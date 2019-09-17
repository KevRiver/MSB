using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MSBNetwork;

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
        /// MSB Custom 무기를 Flip 할때 로컬스케일을 FlipValue 만큼 Flip 한다        
        public bool FlipScale = true;

        [Header("Hands Position")]
		/// the transform to which the character's left hand should be attached to
		public Transform LeftHandHandle;
		/// the transform to which the character's right hand should be attached to
		public Transform RightHandHandle;

		[Header("Effects")]
		/// a list of effects to trigger when the weapon is used
		public List<ParticleSystem> ParticleEffects;

		[Header("Movement")]
        /// if this is true, a multiplier will be applied to movement while the weapon is equipped
        public bool ModifyMovementWhileEquipped = false;
        /// the multiplier to apply to movement while equipped
		public float PermanentMovementMultiplier = 0f;
        /// if this is true, a multiplier will be applied to movement while the weapon is active
		public bool ModifyMovementWhileAttacking = false;
		/// the multiplier to apply to movement while attacking
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

		[Header("Sounds")]
		/// the sound to play when the weapon starts being used
		public AudioClip WeaponStartSfx;
		/// the sound to play while the weapon is in use
		public AudioClip WeaponUsedSfx;
		/// the sound to play when the weapon stops being used
		public AudioClip WeaponStopSfx;
		/// the sound to play when the weapon gets reloaded
		public AudioClip WeaponReloadSfx; 
		/// the sound to play when the weapon gets reloaded
		public AudioClip WeaponReloadNeededSfx;

        [Header("Feedback")]
        /// whether or not the screen should shake when shooting
        public bool ScreenShake = false;
        /// Shake parameters : intensity, duration (in seconds) and decay
        public Vector3 ShakeParameters = new Vector3(1.5f, 0.5f, 1f);
        /// whether or not the screen should flash when shooting
        public bool ScreenFlash = false;

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
        protected List<List<string>> _animatorParameters;
        protected List<string> _ownerAnimatorParameters;
        protected bool _initialized = false;
        public bool isBelongToLocalUser;
        public bool collideOnce;

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
                _animatorParameters = new List<List<string>>();
                InitializeAnimatorParameters();
                _initialized = true;
                //CharacterHandleWeapon의 CurrentWeapon.Initialization 이 끝나고 현재 캐릭터가 isLocalUser 이면 true 값이된다
                isBelongToLocalUser = CharacterHandleWeapon.isLocalUser;
            }			

            SetParticleEffects (false);
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

        public virtual void SetOwner(MSB_Character newOwner, CharacterHandleWeapon handleWeapon)
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
        
		public virtual void TurnWeaponOn()
		{
			SfxPlayWeaponStartSound();
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
            //Debug.LogWarning("CaseWeaponUse");
            WeaponUse();           
            _delayBetweenUsesCounter = TimeBetweenUses;
            WeaponState.ChangeState(WeaponStates.WeaponDelayBetweenUses);
        }

        protected virtual void CaseWeaponDelayBetweenUses()
        {
            _delayBetweenUsesCounter -= Time.deltaTime;
            if (_delayBetweenUsesCounter <= 0)
            {
                if (!Owner.isLocalUser)
                {
                    TurnWeaponOff();
                }
                else if ((TriggerMode == TriggerModes.Auto) && !_triggerReleased)
                {
                    Debug.LogWarning("Case_WDBU : 2nd Condition Enter");
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
                    if (((MSB_Character)Owner).isLocalUser)
                    {
                        RequestBasicAttackActionSync();
                    }
                }					
			}
		}

        public virtual void RequestBasicAttackActionSync()
        {
            bool isAimable = (_aimableWeapon != null) ? true : false;
            Debug.LogWarning("isAimable : " + isAimable);
            float aimDirX = isAimable ? _aimableWeapon._currentAim.x : 0f;
            float aimDirY = isAimable ? _aimableWeapon._currentAim.y : 0f;
            string data =
                           ((MSB_Character)Owner).c_userData.userNumber.ToString() +
                           "," + ((int)(CharacterAbility.ActionType.BasicAttack)).ToString() +
                           "," + isAimable.ToString() +
                           "," + aimDirX.ToString() +
                           "," + aimDirY.ToString();
            NetworkModule.GetInstance().RequestGameUserSync(MSB_GameManager.Instance.roomIndex, data);
        }

		/// <summary>
		/// When the weapon is used, plays the corresponding sound
		/// </summary>
		public virtual void WeaponUse()
		{            
			SetParticleEffects (true);	
            if (ScreenShake)
            {
                if (Owner != null)
                {
                    if (Owner.SceneCamera != null)
                    {
                        Owner.SceneCamera.Shake(ShakeParameters);
                    }
                }                
            }

            if (ScreenFlash)
            {
                MMFlashEvent.Trigger(Color.white, 0.2f, 1f, 1);
            }
            
			SfxPlayWeaponUsedSound();
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
			SfxPlayWeaponStopSound();           
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
		/// Sets the particle effects on or off
		/// </summary>
		/// <param name="status">If set to <c>true</c> status.</param>
		protected virtual void SetParticleEffects(bool status)
		{
			foreach (ParticleSystem system in ParticleEffects)
			{
				if (system == null) { return; }

				if (status)
				{
                    system.Play();
				}
				else
				{
					system.Stop();
				}
			}
		}	

		/// <summary>
		/// Describes what happens when the weapon needs a reload
		/// </summary>
		protected virtual void ReloadNeeded()
		{
			SfxPlayWeaponReloadNeededSound ();
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
				SfxPlayWeaponReloadSound();	
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
            ///MSB Custom
            if (FlipScale || !_spriteRenderer)
            {
                transform.localScale = Vector3.Scale(transform.localScale, FlipValue);
            }
            else
            {
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
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
			Destroy (this.gameObject);

			if (WeaponID != null)
			{
				// we remove it from the inventory
				List<int> weaponList = Owner.gameObject.GetComponentNoAlloc<CharacterInventory> ().WeaponInventory.InventoryContains(WeaponID);
				if (weaponList.Count > 0)
				{
					Owner.gameObject.GetComponentNoAlloc<CharacterInventory> ().WeaponInventory.DestroyItem (weaponList [0]);
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
		protected virtual void SfxPlayWeaponStartSound()
		{
			if (WeaponStartSfx!=null) {	SoundManager.Instance.PlaySound(WeaponStartSfx,transform.position);	}
		}	

		/// <summary>
		/// Plays the weapon's used sound
		/// </summary>
		protected virtual void SfxPlayWeaponUsedSound()
		{
			if (WeaponUsedSfx!=null) {	SoundManager.Instance.PlaySound(WeaponUsedSfx,transform.position);	}
		}	

		/// <summary>
		/// Plays the weapon's stop sound
		/// </summary>
		protected virtual void SfxPlayWeaponStopSound()
		{
			if (WeaponStopSfx!=null) {	SoundManager.Instance.PlaySound(WeaponStopSfx,transform.position);	}
		}	

		/// <summary>
		/// Plays the weapon's reload needed sound
		/// </summary>
		protected virtual void SfxPlayWeaponReloadNeededSound()
		{
			if (WeaponReloadNeededSfx!=null) {	SoundManager.Instance.PlaySound(WeaponReloadNeededSfx,transform.position); }
		}	

		/// <summary>
		/// Plays the weapon's reload sound
		/// </summary>
		protected virtual void SfxPlayWeaponReloadSound()
		{
			if (WeaponReloadSfx!=null) {	SoundManager.Instance.PlaySound(WeaponReloadSfx,transform.position); }
		}

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        public virtual void InitializeAnimatorParameters()
        {
            for (int i = 0; i < Animators.Count; i++)
            {
                _animatorParameters.Add(new List<string>());
            }

            for (int i = 0; i < Animators.Count; i++)
            {
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], WeaponAngleAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], WeaponAngleRelativeAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], IdleAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], StartAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], DelayBeforeUseAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], DelayBetweenUsesAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], StopAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], ReloadStartAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], ReloadStopAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], ReloadAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], SingleUseAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
                MMAnimator.AddAnimatorParamaterIfExists(Animators[i], UseAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters[i]);
            }

            if (_ownerAnimator != null)
            {
                _ownerAnimatorParameters = new List<string>();
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, WeaponAngleAnimationParameter, AnimatorControllerParameterType.Float, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, WeaponAngleRelativeAnimationParameter, AnimatorControllerParameterType.Float, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, IdleAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, StartAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, DelayBeforeUseAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, DelayBetweenUsesAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, StopAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, ReloadStartAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, ReloadStopAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, ReloadAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, SingleUseAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
                MMAnimator.AddAnimatorParamaterIfExists(_ownerAnimator, UseAnimationParameter, AnimatorControllerParameterType.Bool, _ownerAnimatorParameters);
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
                MMAnimator.UpdateAnimatorBool(Animators[i], IdleAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], StartAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponStart), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], DelayBeforeUseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], UseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse || WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse || WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], SingleUseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], DelayBetweenUsesAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], StopAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponStop), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], ReloadStartAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStart), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], ReloadAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReload), _animatorParameters[i]);
                MMAnimator.UpdateAnimatorBool(Animators[i], ReloadStopAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStop), _animatorParameters[i]);

                if (_aimableWeapon != null)
                {
                    MMAnimator.UpdateAnimatorFloat(Animators[i], WeaponAngleAnimationParameter, _aimableWeapon.CurrentAngle, _animatorParameters[i]);
                    MMAnimator.UpdateAnimatorFloat(Animators[i], WeaponAngleRelativeAnimationParameter, _aimableWeapon.CurrentAngleRelative, _animatorParameters[i]);
                }
                else
                {
                    MMAnimator.UpdateAnimatorFloat(Animators[i], WeaponAngleAnimationParameter, 0f, _animatorParameters[i]);
                    MMAnimator.UpdateAnimatorFloat(Animators[i], WeaponAngleRelativeAnimationParameter, 0f, _animatorParameters[i]);
                }
            }

            if ((_ownerAnimator != null) && (WeaponState != null) && (_ownerAnimatorParameters != null))
            {
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, IdleAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, StartAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponStart), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, DelayBeforeUseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, UseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse || WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse || WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, SingleUseAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, DelayBetweenUsesAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, StopAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponStop), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, ReloadStartAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStart), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, ReloadAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReload), _ownerAnimatorParameters);
                MMAnimator.UpdateAnimatorBool(_ownerAnimator, ReloadStopAnimationParameter, (WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStop), _ownerAnimatorParameters);

                if (_aimableWeapon != null)
                {
                    MMAnimator.UpdateAnimatorFloat(_ownerAnimator, WeaponAngleAnimationParameter, _aimableWeapon.CurrentAngle, _ownerAnimatorParameters);
                    MMAnimator.UpdateAnimatorFloat(_ownerAnimator, WeaponAngleRelativeAnimationParameter, _aimableWeapon.CurrentAngleRelative, _ownerAnimatorParameters);
                }
                else
                {
                    MMAnimator.UpdateAnimatorFloat(_ownerAnimator, WeaponAngleAnimationParameter, 0f, _ownerAnimatorParameters);
                    MMAnimator.UpdateAnimatorFloat(_ownerAnimator, WeaponAngleRelativeAnimationParameter, 0f, _ownerAnimatorParameters);
                }
            }
        }
    }
}