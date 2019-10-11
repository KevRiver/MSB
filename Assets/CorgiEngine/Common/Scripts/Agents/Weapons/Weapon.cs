using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// This base class, meant to be extended (see ProjectileWeapon.cs for an example of that) handles rate of fire (rate of use actually), and ammo reloading
	/// </summary>
	[SelectionBase]
	public class Weapon : MonoBehaviour 
	{
		/// the possible use modes for the trigger
		public enum TriggerModes { SemiAuto, Auto }
		/// the possible states the weapon can be in
		public enum WeaponStates { WeaponIdle, WeaponStart, WeaponDelayBeforeUse, WeaponUse, WeaponDelayBetweenUses, WeaponStop, WeaponReloadNeeded, WeaponReloadStart, WeaponReload, WeaponReloadStop, WeaponInterrupted }

		[Header("Use")]
		/// is this weapon on semi or full auto ?
		public TriggerModes TriggerMode = TriggerModes.Auto;
		/// the delay before use, that will be applied for every shot
		public float DelayBeforeUse = 0f;
        /// whether or not the delay before used can be interrupted by releasing the shoot button (if true, releasing the button will cancel the delayed shot)
        public bool DelayBeforeUseReleaseInterruption = true;
		/// the time (in seconds) between two shots		
		public float TimeBetweenUses = 1f;
        /// whether or not the time between uses can be interrupted by releasing the shoot button (if true, releasing the button will cancel the time between uses)
        public bool TimeBetweenUsesReleaseInterruption = true;
        
        [Header("Magazine")]
		/// whether or not the weapon is magazine based. If it's not, it'll just take its ammo inside a global pool
		public bool MagazineBased = false;
		/// the size of the magazine
		public int MagazineSize = 30;
		/// if this is true, pressing the fire button when a reload is needed will reload the weapon. Otherwise you'll need to press the reload button
		public bool AutoReload;
		/// the time it takes to reload the weapon
		public float ReloadTime = 2f;
		/// the amount of ammo consumed everytime the weapon fires
		public int AmmoConsumedPerShot = 1;
		/// if this is set to true, the weapon will auto destroy when there's no ammo left
		public bool AutoDestroyWhenEmpty;
		/// the delay (in seconds) before weapon destruction if empty
		public float AutoDestroyWhenEmptyDelay = 1f;
		[ReadOnly]
		/// the current amount of ammo loaded inside the weapon
		public int CurrentAmmoLoaded = 0;

		[Header("Position")]
		/// an offset that will be applied to the weapon once attached to the center of the WeaponAttachment transform.
		public Vector3 WeaponAttachmentOffset = Vector3.zero;
		/// should that weapon be flipped when the character flips ?
		public bool FlipWeaponOnCharacterFlip = true;
		/// the FlipValue will be used to multiply the model's transform's localscale on flip. Usually it's -1,1,1, but feel free to change it to suit your model's specs
		public Vector3 FlipValue = new Vector3(-1,1,1);
        
        [Header("Hands Position")]
		/// the transform to which the character's left hand should be attached to
		public Transform LeftHandHandle;
		/// the transform to which the character's right hand should be attached to
		public Transform RightHandHandle;
        
		[Header("Movement")]
        /// if this is true, a multiplier will be applied to movement while the weapon is equipped
        public bool ModifyMovementWhileEquipped = false;
        /// the multiplier to apply to movement while equipped
        [Condition("ModifyMovementWhileEquipped", true)]
        public float PermanentMovementMultiplier = 0f;
        /// if this is true, a multiplier will be applied to movement while the weapon is active
		public bool ModifyMovementWhileAttacking = false;
        /// the multiplier to apply to movement while attacking
        [Condition("ModifyMovementWhileAttacking", true)]
        public float MovementMultiplier = 0f;
        /// if this is true all movement will be prevented (even flip) while the weapon is active
        public bool PreventAllMovementWhileInUse = false;        

        [Header("Animation")]
		/// the other animators (other than the Character's) that you want to update every time this weapon gets used
		public List<Animator> Animators;

        [Header("Animation Parameters Names")]
        /// the name of the parameter to send to true as long as this weapon is equipped, used or not. While all the other parameters defined here are updated by the Weapon class itself, and
        /// passed to the weapon and character, this one will be updated by CharacterHandleWeapon only.
        public string EquippedAnimationParameter;
		/// the name of the weapon's idle animation parameter : this will be true all the time except when the weapon is being used
		public string IdleAnimationParameter;
		/// the name of the weapon's start animation parameter : true at the frame where the weapon starts being used
		public string StartAnimationParameter;
		/// the name of the weapon's delay before use animation parameter : true when the weapon has been activated but hasn't been used yet
		public string DelayBeforeUseAnimationParameter;
		/// the name of the weapon's single use animation parameter : true at each frame the weapon activates (shoots)
		public string SingleUseAnimationParameter;
		/// the name of the weapon's in use animation parameter : true at each frame the weapon has started firing but hasn't stopped yet
		public string UseAnimationParameter;
		/// the name of the weapon's delay between each use animation parameter : true when the weapon is in use
		public string DelayBetweenUsesAnimationParameter;
		/// the name of the weapon stop animation parameter : true after a shot and before the next one or the weapon's stop 
		public string StopAnimationParameter;
		/// the name of the weapon reload start animation parameter
		public string ReloadStartAnimationParameter;
		/// the name of the weapon reload animation parameter
		public string ReloadAnimationParameter;
		/// the name of the weapon reload end animation parameter
		public string ReloadStopAnimationParameter;
		/// the name of the weapon's angle animation parameter
		public string WeaponAngleAnimationParameter;
		/// the name of the weapon's angle animation parameter, adjusted so it's always relative to the direction the character is currently facing
		public string WeaponAngleRelativeAnimationParameter;
        
        [Header("Feedbacks")]
        /// the feedback to play when the weapon starts being used
        public MMFeedbacks WeaponStartMMFeedback;
        /// the feedback to play while the weapon is in use
        public MMFeedbacks WeaponUsedMMFeedback;
        /// the feedback to play when the weapon stops being used
        public MMFeedbacks WeaponStopMMFeedback;
        /// the feedback to play when the weapon gets reloaded
        public MMFeedbacks WeaponReloadMMFeedback;
        /// the feedback to play when the weapon gets reloaded
        public MMFeedbacks WeaponReloadNeededMMFeedback;
        
        [Header("Settings")]
        /// If this is true, the weapon will initialize itself on start, otherwise it'll have to be init manually, usually by the CharacterHandleWeapon class
		public bool InitializeOnStart = false;
        /// whether or not this weapon can be interrupted 
        public bool Interruptable = false;

        /// the name of the inventory item corresponding to this weapon. Automatically set (if needed) by InventoryEngineWeapon
        public string WeaponID { get; set; }
		/// the weapon's owner
		public Character Owner { get; protected set; }
		/// the weapon's owner's CharacterHandleWeapon component
		public CharacterHandleWeapon CharacterHandleWeapon {get; set;}
        /// if true, the weapon is flipped
        public bool Flipped { get; set; }
        /// the WeaponAmmo component optionnally associated to this weapon
        public WeaponAmmo WeaponAmmo { get; protected set; }
		/// the weapon's state machine
		public MMStateMachine<WeaponStates> WeaponState;
		protected SpriteRenderer _spriteRenderer;
		protected CharacterGravity _characterGravity;
        protected CorgiController _controller;
		protected CharacterHorizontalMovement _characterHorizontalMovement;
        protected WeaponAim _aimableWeapon;
        protected float _permanentMovementMultiplierStorage = 1f;
        protected float _movementMultiplierStorage = 1f;
        protected Animator _ownerAnimator;

		protected float _delayBeforeUseCounter = 0f;
		protected float _delayBetweenUsesCounter = 0f;
		protected float _reloadingCounter = 0f;
		protected bool _triggerReleased = false;
		protected bool _reloading = false;
        protected ComboWeapon _comboWeapon;

	    protected Vector3 _weaponOffset;
		protected Vector3 _weaponAttachmentOffset;
        protected Transform _weaponAttachment;
        protected List<List<int>> _animatorParameters;
        protected List<int> _ownerAnimatorParameters;
        protected bool _initialized = false;

        // animation parameter
        protected const string _aliveAnimationParameterName = "Alive";
        protected int _equippedAnimationParameter;
        protected int _idleAnimationParameter;
        protected int _startAnimationParameter;
        protected int _delayBeforeUseAnimationParameter;
        protected int _singleUseAnimationParameter;
        protected int _useAnimationParameter;
        protected int _delayBetweenUsesAnimationParameter;
        protected int _stopAnimationParameter;
        protected int _reloadStartAnimationParameter;
        protected int _reloadAnimationParameter;
        protected int _reloadStopAnimationParameter;
        protected int _weaponAngleAnimationParameter;
        protected int _weaponAngleRelativeAnimationParameter;
        protected int _aliveAnimationParameter;
        protected int _comboInProgressAnimationParameter;

        /// <summary>
        /// Initialize this weapon.
        /// </summary>
        public virtual void Initialization()
		{
            if (!_initialized)
            {
                Flipped = false;
                _spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
                _comboWeapon = this.gameObject.GetComponent<ComboWeapon>();
                WeaponState = new MMStateMachine<WeaponStates>(gameObject, true);
                _aimableWeapon = GetComponent<WeaponAim>();
                WeaponAmmo = GetComponent<WeaponAmmo>();
                _animatorParameters = new List<List<int>>();
                InitializeAnimatorParameters();
                InitializeFeedbacks();
                _initialized = true;
            }			

			WeaponState.ChangeState(WeaponStates.WeaponIdle);
            if (WeaponAmmo == null)
			{
				CurrentAmmoLoaded = MagazineSize;
			}

            if (_characterHorizontalMovement != null)
            {
                if (_characterHorizontalMovement.MovementSpeedMultiplier == 0f)
                {
                    _characterHorizontalMovement.MovementSpeedMultiplier = 1f;
                }
                _permanentMovementMultiplierStorage = _characterHorizontalMovement.MovementSpeedMultiplier;
            }
        }

        protected virtual void InitializeFeedbacks()
        {
            WeaponStartMMFeedback?.Initialization(this.gameObject);
            WeaponUsedMMFeedback?.Initialization(this.gameObject);
            WeaponStopMMFeedback?.Initialization(this.gameObject);
            WeaponReloadNeededMMFeedback?.Initialization(this.gameObject);
            WeaponReloadMMFeedback?.Initialization(this.gameObject);
        }

        public virtual void InitializeComboWeapons()
        {
            if (_comboWeapon != null)
            {
                _comboWeapon.Initialization();
            }
        }

		/// <summary>
		/// Sets the weapon's owner
		/// </summary>
		/// <param name="newOwner">New owner.</param>
		public virtual void SetOwner(Character newOwner, CharacterHandleWeapon handleWeapon)
		{
			Owner = newOwner;
            if (Owner != null)
            {
                CharacterHandleWeapon = handleWeapon;
                _characterGravity = Owner.GetComponent<CharacterGravity>();
                _characterHorizontalMovement = Owner.GetComponent<CharacterHorizontalMovement>();
                _controller = Owner.GetComponent<CorgiController>();

                if (CharacterHandleWeapon.AutomaticallyBindAnimator)
                {
                    if (CharacterHandleWeapon.CharacterAnimator != null)
                    {
                        _ownerAnimator = CharacterHandleWeapon.CharacterAnimator;
                    }
                }
            }			
        }

		/// <summary>
		/// Called by input, turns the weapon on
		/// </summary>
		public virtual void WeaponInputStart()
		{
			if (_reloading)
			{
				return;
			}
			if (WeaponState.CurrentState == WeaponStates.WeaponIdle)
			{
				_triggerReleased = false;
				TurnWeaponOn ();
			}
		}

		/// <summary>
		/// Describes what happens when the weapon starts
		/// </summary>
		protected virtual void TurnWeaponOn()
        {
            TriggerWeaponStartFeedback();
            
            WeaponState.ChangeState(WeaponStates.WeaponStart);
			if ((_characterHorizontalMovement != null) && (ModifyMovementWhileAttacking))
			{
				_movementMultiplierStorage = _characterHorizontalMovement.MovementSpeedMultiplier;                
                _characterHorizontalMovement.MovementSpeedMultiplier = MovementMultiplier;
            }
            if (_comboWeapon != null)
            {
                _comboWeapon.WeaponStarted(this);
            }
            if (PreventAllMovementWhileInUse && (_characterHorizontalMovement != null) && (_controller != null))
            {
                _controller.SetForce(Vector2.zero);
                _characterHorizontalMovement.SetHorizontalMove(0f);
                _characterHorizontalMovement.MovementForbidden = true;
            }
        }

		/// <summary>
		/// On Update, we check if the weapon is or should be used
		/// </summary>
		protected virtual void Update()
		{
			ApplyOffset ();
            UpdateAnimator();
		}

		/// <summary>
		/// On LateUpdate, processes the weapon state
		/// </summary>
		protected virtual void LateUpdate()
        {
            ProcessWeaponState();
		}

		/// <summary>
		/// Called every lastUpdate, processes the weapon's state machine
		/// </summary>
		protected virtual void ProcessWeaponState()
		{
			if (WeaponState == null) { return; }
                        
			switch (WeaponState.CurrentState)
			{
				case WeaponStates.WeaponIdle:
                    CaseWeaponIdle();
                    break;

				case WeaponStates.WeaponStart:
                    CaseWeaponStart();
                    break;	

				case WeaponStates.WeaponDelayBeforeUse:
                    CaseWeaponDelayBeforeUse();
                    break;

				case WeaponStates.WeaponUse:
                    CaseWeaponUse();
                    break;

				case WeaponStates.WeaponDelayBetweenUses:
                    CaseWeaponDelayBetweenUses();
                    break;

				case WeaponStates.WeaponStop:
                    CaseWeaponStop();
                    break;

				case WeaponStates.WeaponReloadNeeded:
                    CaseWeaponReloadNeeded();
                    break;

				case WeaponStates.WeaponReloadStart:
                    CaseWeaponReloadStart();
                    break;

				case WeaponStates.WeaponReload:
                    CaseWeaponReload();
                    break;

				case WeaponStates.WeaponReloadStop:
                    CaseWeaponReloadStop();
                    break;

                case WeaponStates.WeaponInterrupted:
                    CaseWeaponInterrupted();
                    break;
			}
		}

        protected virtual void CaseWeaponIdle()
        {
            ResetMovementMultiplier();
        }

        protected virtual void CaseWeaponStart()
        {
            if (DelayBeforeUse > 0)
            {
                _delayBeforeUseCounter = DelayBeforeUse;
                WeaponState.ChangeState(WeaponStates.WeaponDelayBeforeUse);
            }
            else
            {
                ShootRequest();
            }
        }

        protected virtual void CaseWeaponDelayBeforeUse()
        {
            _delayBeforeUseCounter -= Time.deltaTime;
            if (_delayBeforeUseCounter <= 0)
            {
                ShootRequest();
            }
        }

        protected virtual void CaseWeaponUse()
        {
            WeaponUse();
            _delayBetweenUsesCounter = TimeBetweenUses;
            WeaponState.ChangeState(WeaponStates.WeaponDelayBetweenUses);
        }

        protected virtual void CaseWeaponDelayBetweenUses()
        {
            _delayBetweenUsesCounter -= Time.deltaTime;
            if (_delayBetweenUsesCounter <= 0)
            {
                if ((TriggerMode == TriggerModes.Auto) && !_triggerReleased)
                {
                    ShootRequest();
                }
                else
                {
                    TurnWeaponOff();
                }
            }
        }

        protected virtual void CaseWeaponStop()
        {
            WeaponState.ChangeState(WeaponStates.WeaponIdle);
        }

        protected virtual void CaseWeaponReloadNeeded()
        {
            ReloadNeeded();
            ResetMovementMultiplier();
            WeaponState.ChangeState(WeaponStates.WeaponIdle);
        }

        protected virtual void CaseWeaponReloadStart()
        {
            ReloadWeapon();
            _reloadingCounter = ReloadTime;
            WeaponState.ChangeState(WeaponStates.WeaponReload);
        }

        protected virtual void CaseWeaponReload()
        {
            ResetMovementMultiplier();
            _reloadingCounter -= Time.deltaTime;
            if (_reloadingCounter <= 0)
            {
                WeaponState.ChangeState(WeaponStates.WeaponReloadStop);
            }
        }

        protected virtual void CaseWeaponReloadStop()
        {
            _reloading = false;
            WeaponState.ChangeState(WeaponStates.WeaponIdle);
            if (WeaponAmmo == null)
            {
                CurrentAmmoLoaded = MagazineSize;
            }
        }

        protected virtual void CaseWeaponInterrupted()
        {
            TurnWeaponOff();
            ResetMovementMultiplier();
            WeaponState.ChangeState(WeaponStates.WeaponIdle);
        }

        /// <summary>
        /// Call this method to interrupt the weapon
        /// </summary>
        public virtual void Interrupt()
        {
            if (Interruptable)
            {
                WeaponState.ChangeState(WeaponStates.WeaponInterrupted);
            }
        }

		/// <summary>
		/// Determines whether or not the weapon can fire
		/// </summary>
		protected virtual void ShootRequest()
		{
			// if we have a weapon ammo component, we determine if we have enough ammunition to shoot
			if (_reloading)
			{
				return;
			}

			if (MagazineBased)
			{
				if (WeaponAmmo != null)
				{
					if (WeaponAmmo.EnoughAmmoToFire())
					{
						WeaponState.ChangeState(WeaponStates.WeaponUse);	
					}
					else
					{
						if (AutoReload && MagazineBased)
						{
							InitiateReloadWeapon ();
						}
						else
						{
							WeaponState.ChangeState(WeaponStates.WeaponReloadNeeded);	
						}
					}
				}
				else
				{
					if (CurrentAmmoLoaded > 0)
					{
						WeaponState.ChangeState(WeaponStates.WeaponUse);	
						CurrentAmmoLoaded -= AmmoConsumedPerShot;
					}
					else
					{
						if (AutoReload)
						{
							InitiateReloadWeapon ();
						}
						else
						{
							WeaponState.ChangeState(WeaponStates.WeaponReloadNeeded);	
						}
					}
				}
			}
			else
			{
				if (WeaponAmmo != null)
				{
					if (WeaponAmmo.EnoughAmmoToFire())
					{
						WeaponState.ChangeState(WeaponStates.WeaponUse);	
					}
					else
					{
						WeaponState.ChangeState(WeaponStates.WeaponReloadNeeded);	
					}	
				}
				else
				{
					WeaponState.ChangeState(WeaponStates.WeaponUse);						
				}					
			}
		}

		/// <summary>
		/// When the weapon is used, plays the corresponding sound
		/// </summary>
		protected virtual void WeaponUse()
		{	            
			TriggerWeaponUsedFeedback();
		}

		/// <summary>
		/// Called by input, turns the weapon off if in auto mode
		/// </summary>
		public virtual void WeaponInputStop()
		{
			if (_reloading)
			{
				return;
			}
			_triggerReleased = true	;		
		}

		/// <summary>
		/// Turns the weapon off.
		/// </summary>
		public virtual void TurnWeaponOff()
        {
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeedMultiplier = _permanentMovementMultiplierStorage;
            }            

            if ((WeaponState.CurrentState == WeaponStates.WeaponIdle || WeaponState.CurrentState == WeaponStates.WeaponStop))
            {
                return;
            }
			_triggerReleased = true;
			TriggerWeaponStopFeedback();
			WeaponState.ChangeState(WeaponStates.WeaponStop);
            if (_comboWeapon != null)
            {
                _comboWeapon.WeaponStopped(this);
            }
            if (PreventAllMovementWhileInUse && (_characterHorizontalMovement != null))
            {
                _characterHorizontalMovement.MovementForbidden = false;
            }
        }

		protected virtual void ResetMovementMultiplier ()
		{
			if (_characterHorizontalMovement != null)
            { 
                if (ModifyMovementWhileAttacking)
			    {
				    _characterHorizontalMovement.MovementSpeedMultiplier = _movementMultiplierStorage;
			    }

                if (ModifyMovementWhileEquipped)
                {
                    _characterHorizontalMovement.MovementSpeedMultiplier = PermanentMovementMultiplier;
                }
                else
                {
                    _characterHorizontalMovement.MovementSpeedMultiplier = _permanentMovementMultiplierStorage;
                }
            }
        }

		/// <summary>
		/// Describes what happens when the weapon needs a reload
		/// </summary>
		protected virtual void ReloadNeeded()
		{
			TriggerWeaponReloadNeededFeedback ();
		}

		public virtual void InitiateReloadWeapon()
		{
			// if we're already reloading, we do nothing and exit
			if (_reloading)
			{
				return;
			}
			WeaponState.ChangeState(WeaponStates.WeaponReloadStart);
			_reloading = true;
		}

		/// <summary>
		/// Reloads the weapon
		/// </summary>
		/// <param name="ammo">Ammo.</param>
		protected virtual void ReloadWeapon()
		{
			if (MagazineBased)
			{
				TriggerWeaponReloadFeedback();	
			}
		}

		/// <summary>
		/// Flips the weapon.
		/// </summary>
		public virtual void FlipWeapon()
        {
            Flipped = !Flipped;

            if (_comboWeapon != null)
            {
                _comboWeapon.FlipUnusedWeapons();
            }
        }

		/// <summary>
		/// Flips the weapon model.
		/// </summary>
		public virtual void FlipWeaponModel()
		{	
			if (_spriteRenderer != null)
			{
				_spriteRenderer.flipX = !_spriteRenderer.flipX;
			} 
			else
			{
				transform.localScale = Vector3.Scale (transform.localScale, FlipValue);		
			}	
		}

		/// <summary>
		/// Destroys the weapon
		/// </summary>
		/// <returns>The destruction.</returns>
		public virtual IEnumerator WeaponDestruction()
		{
            yield return new WaitForSeconds (AutoDestroyWhenEmptyDelay);
            // if we don't have ammo anymore, and need to destroy our weapon, we do it
            TurnWeaponOff();
            Destroy (this.gameObject);
            
            if (WeaponID != null)
			{
				// we remove it from the inventory
				List<int> weaponList = Owner.gameObject.MMGetComponentNoAlloc<CharacterInventory> ().WeaponInventory.InventoryContains(WeaponID);
				if (weaponList.Count > 0)
				{
					Owner.gameObject.MMGetComponentNoAlloc<CharacterInventory> ().WeaponInventory.DestroyItem (weaponList [0]);
				}	
			}
		}

		/// <summary>
		/// Applies the offset specified in the inspector
		/// </summary>
		protected virtual void ApplyOffset()
		{
			_weaponAttachmentOffset = WeaponAttachmentOffset;

			if (Flipped)
			{
				_weaponAttachmentOffset.x = -WeaponAttachmentOffset.x;
			}

			if (_characterGravity != null)
			{
				_weaponAttachmentOffset = MMMaths.RotateVector2 (_weaponAttachmentOffset,_characterGravity.GravityAngle);
			}
			// we apply the offset
			if (transform.parent != null)
			{
				_weaponOffset = transform.parent.position + _weaponAttachmentOffset;
				transform.position = _weaponOffset;
			}	
		}

		/// <summary>
		/// Plays the weapon's start sound
		/// </summary>
		protected virtual void TriggerWeaponStartFeedback()
		{
            WeaponStartMMFeedback?.PlayFeedbacks(this.transform.position);
        }	

		/// <summary>
		/// Plays the weapon's used sound
		/// </summary>
		protected virtual void TriggerWeaponUsedFeedback()
		{
            WeaponUsedMMFeedback?.PlayFeedbacks(this.transform.position);
        }	

		/// <summary>
		/// Plays the weapon's stop sound
		/// </summary>
		protected virtual void TriggerWeaponStopFeedback()
		{
            WeaponStopMMFeedback?.PlayFeedbacks(this.transform.position);
        }	

		/// <summary>
		/// Plays the weapon's reload needed sound
		/// </summary>
		protected virtual void TriggerWeaponReloadNeededFeedback()
        {
            WeaponReloadNeededMMFeedback?.PlayFeedbacks(this.transform.position);
        }	

		/// <summary>
		/// Plays the weapon's reload sound
		/// </summary>
		protected virtual void TriggerWeaponReloadFeedback()
        {
            WeaponReloadMMFeedback?.PlayFeedbacks(this.transform.position);
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        public virtual void InitializeAnimatorParameters()
        {
            for (int i = 0; i < Animators.Count; i++)
            {
                _animatorParameters.Add(new List<int>());
                AddParametersToAnimator(Animators[i], _animatorParameters[i]);
            }

            if (_ownerAnimator != null)
            {
                _ownerAnimatorParameters = new List<int>();
                AddParametersToAnimator(_ownerAnimator, _ownerAnimatorParameters);
            }            
        }

        protected virtual void AddParametersToAnimator(Animator animator, List<int> list)
        {
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, EquippedAnimationParameter, out _equippedAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, WeaponAngleAnimationParameter, out _weaponAngleAnimationParameter, AnimatorControllerParameterType.Float, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, WeaponAngleRelativeAnimationParameter, out _weaponAngleRelativeAnimationParameter, AnimatorControllerParameterType.Float, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, IdleAnimationParameter, out _idleAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, StartAnimationParameter, out _startAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, DelayBeforeUseAnimationParameter, out _delayBeforeUseAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, DelayBetweenUsesAnimationParameter, out _delayBetweenUsesAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, StopAnimationParameter, out _stopAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, ReloadStartAnimationParameter, out _reloadStartAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, ReloadStopAnimationParameter, out _reloadStopAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, ReloadAnimationParameter, out _reloadAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, SingleUseAnimationParameter, out _singleUseAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, UseAnimationParameter, out _useAnimationParameter, AnimatorControllerParameterType.Bool, list);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, _aliveAnimationParameterName, out _aliveAnimationParameter, AnimatorControllerParameterType.Bool, list);

            if (_comboWeapon != null)
            {
                MMAnimatorExtensions.AddAnimatorParameterIfExists(animator, _comboWeapon.ComboInProgressAnimationParameter, out _comboInProgressAnimationParameter, AnimatorControllerParameterType.Bool, list);
            }
        }

        /// <summary>
		/// Override this to send parameters to the character's animator. This is called once per cycle, by the Character
		/// class, after Early, normal and Late process().
		/// </summary>
		public virtual void UpdateAnimator()
        {
            for (int i = 0; i < Animators.Count; i++)
            {
                UpdateAnimator(Animators[i], _animatorParameters[i]);
            }

            if ((_ownerAnimator != null) && (WeaponState != null) && (_ownerAnimatorParameters != null))
            {
                UpdateAnimator(_ownerAnimator, _ownerAnimatorParameters);
            }
        }

        protected virtual void UpdateAnimator(Animator animator, List<int> list)
        {
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _equippedAnimationParameter, true, list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _idleAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _startAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponStart), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _delayBeforeUseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _useAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse || WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse || WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _singleUseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _delayBetweenUsesAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _stopAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponStop), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _reloadStartAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStart), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _reloadAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReload), list);
            MMAnimatorExtensions.UpdateAnimatorBool(animator, _reloadStopAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStop), list);

            if (Owner != null)
            {
                MMAnimatorExtensions.UpdateAnimatorBool(animator, _aliveAnimationParameter, (Owner.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead), list);
            }

            if (_aimableWeapon != null)
            {
                MMAnimatorExtensions.UpdateAnimatorFloat(animator, _weaponAngleAnimationParameter, _aimableWeapon.CurrentAngle, list);
                MMAnimatorExtensions.UpdateAnimatorFloat(animator, _weaponAngleRelativeAnimationParameter, _aimableWeapon.CurrentAngleRelative, list);
            }
            else
            {
                MMAnimatorExtensions.UpdateAnimatorFloat(animator, _weaponAngleAnimationParameter, 0f, list);
                MMAnimatorExtensions.UpdateAnimatorFloat(animator, _weaponAngleRelativeAnimationParameter, 0f, list);
            }

            if (_comboWeapon != null)
            {
                MMAnimatorExtensions.UpdateAnimatorBool(animator, _comboInProgressAnimationParameter, _comboWeapon.ComboInProgress, list);
            }
        }
    }
}