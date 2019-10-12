using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a character so it can use weapons
	/// Note that this component will trigger animations (if their parameter is present in the Animator), based on 
	/// the current weapon's Animations
	/// Animator parameters : defined from the Weapon's inspector
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Handle Weapon")] 
	public class CharacterHandleWeapon : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component will allow your character to pickup and use weapons. What the weapon will do is defined in the Weapon classes. This just describes the behaviour of the 'hand' holding the weapon, not the weapon itself. Here you can set an initial weapon for your character to start with, allow weapon pickup, and specify a weapon attachment (a transform inside of your character, could be just an empty child gameobject, or a subpart of your model."; }
        [Header("Weapon")]
		/// the initial weapon owned by the character
		public Weapon InitialWeapon;
        /// if this is set to true, the character can pick up PickableWeapons
        public bool CanPickupWeapons = true;
        [Header("Binding")]
        /// the position the weapon will be attached to. If left blank, will be this.transform.
		public Transform WeaponAttachment;
        /// if this is true this animator will be automatically bound to the weapon
        public bool AutomaticallyBindAnimator = true;
        [Header("Input")]
        /// if this is true you won't have to release your fire button to auto reload
		public bool ContinuousPress = false;
        /// whether or not this character getting hit should interrupt its attack (will only work if the weapon is marked as interruptable)
        public bool GettingHitInterruptsAttack = false;
        [Header("Buffering")]
        /// whether or not attack input should be buffered, letting you prepare an attack while another is being performed, making it easier to chain them
        public bool BufferInput;
        [Condition("BufferInput", true)]
        /// if this is true, every new input will prolong the buffer
        public bool NewInputExtendsBuffer;
        [Condition("BufferInput", true)]
        /// the maximum duration for the buffer, in seconds
        public float MaximumBufferDuration = 0.25f;
        /// returns the current equipped weapon
        [Header("Debug")]
        [ReadOnly]
        public Weapon CurrentWeapon;

        public Animator CharacterAnimator { get; set; }

        protected float _fireTimer = 0f;
		protected float _secondaryHorizontalMovement;
		protected float _secondaryVerticalMovement;
		protected WeaponAim _aimableWeapon;
		protected WeaponIK _weaponIK;
		protected Transform _leftHandTarget = null;
		protected Transform _rightHandTarget = null;

        protected float _bufferEndsAt = 0f;
        protected bool _buffering = false;

	    // Initialization
		protected override void Initialization () 
		{
			base.Initialization();
								
			Setup ();
		}

		/// <summary>
		/// Grabs various components and inits stuff
		/// </summary>
		public virtual void Setup()
        {
            _character = gameObject.MMGetComponentNoAlloc<Character>();
            CharacterAnimator = _animator;

            // filler if the WeaponAttachment has not been set
            if (WeaponAttachment==null)
			{
				WeaponAttachment=transform;
			}		
			if (_animator != null)
			{
				_weaponIK = _animator.GetComponent<WeaponIK> ();
			}	
			// we set the initial weapon
			if (InitialWeapon != null)
			{
				ChangeWeapon(InitialWeapon, null);			
			}
        }

		/// <summary>
		/// Every frame we check if it's needed to update the ammo display
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility ();
			UpdateAmmoDisplay ();
            HandleBuffer();
		}

		/// <summary>
		/// Gets input and triggers methods based on what's been pressed
		/// </summary>
		protected override void HandleInput ()
		{			

			if ((_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) || (_inputManager.ShootAxis == MMInput.ButtonStates.ButtonDown))
            {
				ShootStart();
			}

            if (CurrentWeapon != null)
            {
                if (ContinuousPress && (CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto) && (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed))
                {
                    ShootStart();
                }
                if (ContinuousPress && (CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto) && (_inputManager.ShootAxis == MMInput.ButtonStates.ButtonPressed))
                {
                    ShootStart();
                }
            }			

			if (_inputManager.ReloadButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				Reload();
            }

            if ((_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) || (_inputManager.ShootAxis == MMInput.ButtonStates.ButtonUp))
            {
                ShootStop();
            }

            if (CurrentWeapon != null)
            {
                if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses)
                && ((_inputManager.ShootAxis == MMInput.ButtonStates.Off) && (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.Off)))
                {
                    CurrentWeapon.WeaponInputStop();
                }
            }            
        }

        /// <summary>
        /// Triggers an attack if the weapon is idle and an input has been buffered
        /// </summary>
        protected virtual void HandleBuffer()
        {
            if (CurrentWeapon == null)
            {
                return;
            }

            // if we are currently buffering an input and if the weapon is now idle
            if (_buffering && (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle))
            {
                // and if our buffer is still valid, we trigger an attack
                if (Time.time < _bufferEndsAt)
                {
                    ShootStart();
                }
                _buffering = false;
            }
        }
						
		/// <summary>
		/// Causes the character to start shooting
		/// </summary>
		public virtual void ShootStart()
		{
			// if the Shoot action is enabled in the permissions, we continue, if not we do nothing.  If the player is dead we do nothing.
			if ( !AbilityPermitted
				|| (CurrentWeapon == null)
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
				|| (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing))
			{
				return;
			}

            //  if we've decided to buffer input, and if the weapon is in use right now
            if (BufferInput && (CurrentWeapon.WeaponState.CurrentState != Weapon.WeaponStates.WeaponIdle))
            {
                // if we're not already buffering, or if each new input extends the buffer, we turn our buffering state to true
                if (!_buffering || NewInputExtendsBuffer)
                {
                    _buffering = true;
                    _bufferEndsAt = Time.time + MaximumBufferDuration;
                }
            }

            PlayAbilityStartFeedbacks();
            CurrentWeapon.WeaponInputStart();
		}
		
		/// <summary>
		/// Causes the character to stop shooting
		/// </summary>
		public virtual void ShootStop()
		{
			// if the Shoot action is enabled in the permissions, we continue, if not we do nothing
			if (!AbilityPermitted
				|| (CurrentWeapon == null)
                || (_movement == null))
			{
				return;		
			}		

			if (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing && CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle)
			{
				return;
			}

			if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReload)
				|| (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStart)
                || (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStop)
                || (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse))
			{
				return;
			}

            if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse) && (!CurrentWeapon.DelayBeforeUseReleaseInterruption))
            {
                return;
            }

            if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses) && (!CurrentWeapon.TimeBetweenUsesReleaseInterruption))
            {
                return;
            }

            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
            CurrentWeapon.TurnWeaponOff();
		}

		/// <summary>
		/// Reloads the weapon
		/// </summary>
		protected virtual void Reload()
		{
			if (CurrentWeapon != null)
			{
				CurrentWeapon.InitiateReloadWeapon ();
			}
		}
		
		/// <summary>
		/// Changes the character's current weapon to the one passed as a parameter
		/// </summary>
		/// <param name="newWeapon">The new weapon.</param>
		public virtual void ChangeWeapon(Weapon newWeapon, string weaponID, bool combo = false)
		{
            // if the character already has a weapon, we make it stop shooting
            if (CurrentWeapon!=null)
			{
                if (!combo)
                {
                    ShootStop();
                    int equippedAnimationParameter = Animator.StringToHash(CurrentWeapon.EquippedAnimationParameter);
                    if (_character._animatorParameters.Contains(equippedAnimationParameter))
                    {
                        MMAnimatorExtensions.UpdateAnimatorBool(_animator, equippedAnimationParameter, false, _character._animatorParameters);
                    }
                    
                    Destroy(CurrentWeapon.gameObject);
                }				
			}

			if (newWeapon != null)
			{			
                if (!combo)
                {
                    CurrentWeapon = (Weapon)Instantiate(newWeapon, WeaponAttachment.transform.position + newWeapon.WeaponAttachmentOffset, Quaternion.identity);
                }				
				CurrentWeapon.transform.SetParent(WeaponAttachment.transform);
				CurrentWeapon.SetOwner (_character, this);
				CurrentWeapon.WeaponID = weaponID;
				_aimableWeapon = CurrentWeapon.GetComponent<WeaponAim> ();
				// we handle (optional) inverse kinematics (IK) 
				if (_weaponIK != null)
				{
					_weaponIK.SetHandles(CurrentWeapon.LeftHandHandle, CurrentWeapon.RightHandHandle);
				}
				// we turn off the gun's emitters.
				CurrentWeapon.Initialization();
                CurrentWeapon.InitializeComboWeapons();
                CurrentWeapon.InitializeAnimatorParameters();
				InitializeAnimatorParameters();
                if ((_character != null) && !combo)
                {
                    if (!_character.IsFacingRight)
                    {
                        if (CurrentWeapon != null)
                        {
                            CurrentWeapon.FlipWeapon();
                            CurrentWeapon.FlipWeaponModel();
                        }
                    }
                }				
			}
			else
			{
				CurrentWeapon = null;
			}
		}	

		/// <summary>
		/// Flips the current weapon if needed
		/// </summary>
		public override void Flip()
		{
			if (CurrentWeapon != null)
            {
                CurrentWeapon.FlipWeapon();
				if (CurrentWeapon.FlipWeaponOnCharacterFlip)
				{
					CurrentWeapon.FlipWeaponModel();
				}
			}
		}

		/// <summary>
		/// Updates the ammo display bar and text.
		/// </summary>
		public virtual void UpdateAmmoDisplay()
		{
			if ( (GUIManager.Instance != null) && (_character.CharacterType == Character.CharacterTypes.Player) )
			{
				if (CurrentWeapon == null)
				{
					GUIManager.Instance.SetAmmoDisplays (false, _character.PlayerID);
					return;
				}

				if (!CurrentWeapon.MagazineBased && (CurrentWeapon.WeaponAmmo == null))
				{
					GUIManager.Instance.SetAmmoDisplays (false, _character.PlayerID);
					return;
				}

				if (CurrentWeapon.WeaponAmmo == null)
				{					
					GUIManager.Instance.SetAmmoDisplays (true, _character.PlayerID);
					GUIManager.Instance.UpdateAmmoDisplays(CurrentWeapon.MagazineBased, 0, 0, CurrentWeapon.CurrentAmmoLoaded, CurrentWeapon.MagazineSize, _character.PlayerID, false);	
					return;
				}
				else
				{
					GUIManager.Instance.SetAmmoDisplays (true, _character.PlayerID);
					GUIManager.Instance.UpdateAmmoDisplays(CurrentWeapon.MagazineBased, CurrentWeapon.WeaponAmmo.CurrentAmmoAvailable, CurrentWeapon.WeaponAmmo.MaxAmmo, CurrentWeapon.CurrentAmmoLoaded, CurrentWeapon.MagazineSize, _character.PlayerID, true);	
					return;
				}
			}
		}
        
        protected override void OnHit()
        {
            base.OnHit();
            if (GettingHitInterruptsAttack && (CurrentWeapon != null))
            {
                CurrentWeapon.Interrupt();
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            ShootStop();
        }
    }
}