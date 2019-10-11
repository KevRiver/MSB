using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{	
	[RequireComponent(typeof(Weapon))]
	/// <summary>
	/// Add this component to a Weapon and you'll be able to aim it (meaning you'll rotate it)
	/// Supported control modes are mouse, primary movement (you aim wherever you direct your character) and secondary movement (using a secondary axis, separate from the movement).
	/// </summary>
	[AddComponentMenu("Corgi Engine/Weapons/Weapon Aim")]
	public class WeaponAim : MonoBehaviour 
	{
		/// the list of possible control modes
		public enum AimControls { Off, PrimaryMovement, SecondaryMovement, Mouse, Script }
		/// the list of possible rotation modes
		public enum RotationModes { Free, Strict4Directions, Strict8Directions }

		[Header("Control Mode")]
		[Information("Add this component to a Weapon and you'll be able to aim (rotate) it. It supports three different control modes : mouse (the weapon aims towards the pointer), primary movement (you'll aim towards the current input direction), or secondary movement (aims towards a second input axis, think twin stick shooters).",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the aim control mode
		public AimControls AimControl = AimControls.SecondaryMovement;

		[Header("Weapon Rotation")]
		[Information("Here you can define whether the rotation is free, strict in 4 directions (top, bottom, left, right), or 8 directions (same + diagonals). You can also define a rotation speed, and a min and max angle. For example, if you don't want your character to be able to aim in its back, set min angle to -90 and max angle to 90.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the rotation mode
		public RotationModes RotationMode = RotationModes.Free;
		/// the the speed at which the weapon reaches its new position. Set it to zero if you want movement to directly follow input
		public float WeaponRotationSpeed = 1f;
		[Range(-180,180)]
		/// the minimum angle at which the weapon's rotation will be clamped
		public float MinimumAngle = -180f;
		[Range(-180,180)]
		/// the maximum angle at which the weapon's rotation will be clamped
		public float MaximumAngle = 180f;

		[Header("Reticle")]
		[Information("You can also display a reticle on screen to check where you're aiming at. Leave it blank if you don't want to use one. If you set the reticle distance to 0, it'll follow the cursor, otherwise it'll be on a circle centered on the weapon. You can also ask it to follow the mouse, even replace the mouse pointer. You can also decide if the pointer should rotate to reflect aim angle or remain stable.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the gameobject to display as the aim's reticle/crosshair. Leave it blank if you don't want a reticle
		public GameObject Reticle;
		/// if set to false, the reticle won't be added and displayed
		public bool DisplayReticle = true;
		/// the distance at which the reticle will be from the weapon
		public float ReticleDistance;
		/// if set to true, the reticle will be placed at the mouse's position (like a pointer)
		public bool ReticleAtMousePosition;
		/// if set to true, the reticle will rotate on itself to reflect the weapon's rotation. If not it'll remain stable.
		public bool RotateReticle = false;
		/// if set to true, the reticle will replace the mouse pointer
		public bool ReplaceMousePointer = true;
        /// whether or not the reticle should be hidden when the character is dead
        public bool DisableReticleOnDeath = true;
		/// the weapon's current rotation
		public Quaternion CurrentRotation { get { return transform.rotation; } }
		/// the current angle the weapon is aiming at
		public float CurrentAngle { get; protected set; }
		/// the current angle the weapon is aiming at, adjusted to compensate for the current orientation of the character
		public float CurrentAngleRelative 
		{
			get 
			{  
				if (_weapon != null)
				{
					if (_weapon.Owner != null)
					{
						if (_weapon.Owner.IsFacingRight)
						{
							return CurrentAngle;
						}
						else
						{
							return -CurrentAngle;
						}
					}
				}
				return 0;
			}
		}

		protected Weapon _weapon;
		protected Vector3 _currentAim = Vector3.zero;
		protected Quaternion _lookRotation;
		protected Vector3 _direction;
		protected float[] _possibleAngleValues;
		protected Vector3 _mousePosition;
		protected float _additionalAngle;
		protected Quaternion _initialRotation;
        protected Camera _mainCamera;
		protected CharacterGravity _characterGravity;

		protected GameObject _reticle;

		/// <summary>
		/// On Start(), we trigger the initialization
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// Grabs the weapon component, initializes the angle values
		/// </summary>
		protected virtual void Initialization()
		{
			_weapon = GetComponent<Weapon> ();
			if (_weapon.Owner != null)
			{
				_characterGravity = _weapon.Owner.GetComponent<CharacterGravity> ();	
			}

			if (RotationMode == RotationModes.Strict4Directions)
			{
				_possibleAngleValues = new float[5];
				_possibleAngleValues [0] = -180f;
				_possibleAngleValues [1] = -90f;
				_possibleAngleValues [2] = 0f;
				_possibleAngleValues [3] = 90f;
				_possibleAngleValues [4] = 180f;
			}
			if (RotationMode == RotationModes.Strict8Directions)
			{
				_possibleAngleValues = new float[9];
				_possibleAngleValues [0] = -180f;
				_possibleAngleValues [1] = -135f;
				_possibleAngleValues [2] = -90f;
				_possibleAngleValues [3] = -45f;
				_possibleAngleValues [4] = 0f;
				_possibleAngleValues [5] = 45f;
				_possibleAngleValues [6] = 90f;
				_possibleAngleValues [7] = 135f;
				_possibleAngleValues [8] = 180f;
			}
			_initialRotation = transform.rotation;
			InitializeReticle();
            _mainCamera = Camera.main;
        }

		/// <summary>
		/// Aims the weapon towards a new point
		/// </summary>
		/// <param name="newAim">New aim.</param>
		public virtual void SetCurrentAim(Vector3 newAim)
		{
			_currentAim = newAim;
		}

		/// <summary>
		/// Computes the current aim direction
		/// </summary>
		protected virtual void GetCurrentAim()
		{
			if (_weapon.Owner == null)
			{
				return;
			}

			if ((_weapon.Owner.LinkedInputManager == null) && (_weapon.Owner.CharacterType == Character.CharacterTypes.Player))
			{
				return;
			}

			switch (AimControl)
			{
				case AimControls.Off:
					if (_weapon.Owner == null) { return; }

					_currentAim = Vector2.right;
					_direction = Vector2.right;
					if (_characterGravity != null)
					{
						_currentAim = _characterGravity.transform.right;
						_direction = _characterGravity.transform.right;
					}
					break;

				case AimControls.Script:
					_currentAim = (_weapon.Owner.IsFacingRight) ? _currentAim : -_currentAim;
					_direction = -(transform.position - _currentAim);
					break;

				case AimControls.PrimaryMovement:
					if (_weapon.Owner == null)
					{
						return;
					}
										
					if (_weapon.Owner.IsFacingRight)
					{
						_currentAim = _weapon.Owner.LinkedInputManager.PrimaryMovement;
						_direction = transform.position + _currentAim;
					} else
					{
						_currentAim = -_weapon.Owner.LinkedInputManager.PrimaryMovement;
						_direction = -(transform.position - _currentAim);
					}

					if (_characterGravity != null)
					{
						_currentAim = MMMaths.RotateVector2 (_currentAim,_characterGravity.GravityAngle);	
						if (_characterGravity.ShouldReverseInput())
						{
							_currentAim = -_currentAim;
						}
					}
					break;

				case AimControls.SecondaryMovement:
					if (_weapon.Owner == null) { return; }

					if (_weapon.Owner.IsFacingRight)
					{
						_currentAim = _weapon.Owner.LinkedInputManager.SecondaryMovement;
						_direction = transform.position + _currentAim;
					}
					else
					{
						_currentAim = -_weapon.Owner.LinkedInputManager.SecondaryMovement;
						_direction = -(transform.position - _currentAim);
					}										
					break;

				case AimControls.Mouse:
					if (_weapon.Owner == null)
					{
						return;
					}

					_mousePosition = Input.mousePosition;
					_mousePosition.z = 10;

					_direction = _mainCamera.ScreenToWorldPoint (_mousePosition);
					_direction.z = transform.position.z;

					if (_weapon.Owner.IsFacingRight)
					{
						_currentAim = _direction - transform.position;
					}
					else
					{
						_currentAim = transform.position - _direction;
					}
					break;			
			}
		}

		/// <summary>
		/// Every frame, we compute the aim direction and rotate the weapon accordingly
		/// </summary>
		protected virtual void Update()
		{
			GetCurrentAim ();
			DetermineWeaponRotation ();
			MoveReticle();
			HideReticle ();
		}

		protected virtual void LateUpdate()
		{
			ResetAdditionalAngle ();
		}

		/// <summary>
		/// Determines the weapon rotation based on the current aim direction
		/// </summary>
		protected virtual void DetermineWeaponRotation()
		{
			if (_currentAim != Vector3.zero)
			{
				if (_direction != Vector3.zero)
				{
					CurrentAngle = Mathf.Atan2 (_currentAim.y, _currentAim.x) * Mathf.Rad2Deg;
					if (RotationMode == RotationModes.Strict4Directions || RotationMode == RotationModes.Strict8Directions)
					{
						CurrentAngle = MMMaths.RoundToClosest (CurrentAngle, _possibleAngleValues);
					}

					// we add our additional angle
					CurrentAngle += _additionalAngle;

					// we clamp the angle to the min/max values set in the inspector
					if (_weapon.Owner.IsFacingRight)
					{
						CurrentAngle = Mathf.Clamp (CurrentAngle, MinimumAngle, MaximumAngle);	
					}
					else
					{
						CurrentAngle = Mathf.Clamp (CurrentAngle, -MaximumAngle, -MinimumAngle);	
					}
					_lookRotation = Quaternion.Euler (CurrentAngle * Vector3.forward);
					RotateWeapon(_lookRotation);
				}
			}
			else
			{
				CurrentAngle = 0f;
				if (_characterGravity == null)
				{
					RotateWeapon(_initialRotation);	
				}
				else
				{
					RotateWeapon (_characterGravity.transform.rotation);
				}
			}
			MMDebug.DebugDrawArrow (this.transform.position, _currentAim.normalized, Color.green);
		}

		/// <summary>
		/// Rotates the weapon, optionnally applying a lerp to it.
		/// </summary>
		/// <param name="newRotation">New rotation.</param>
		protected virtual void RotateWeapon(Quaternion newRotation)
		{
			if (GameManager.Instance.Paused)
			{
				return;
			}
			// if the rotation speed is == 0, we have instant rotation
			if (WeaponRotationSpeed == 0)
			{
				transform.rotation = newRotation;	
			}
			// otherwise we lerp the rotation
			else
			{				
				transform.rotation = Quaternion.Lerp (transform.rotation, newRotation, WeaponRotationSpeed * Time.deltaTime);
			}
		}

		/// <summary>
		/// If a reticle has been set, instantiates the reticle and positions it
		/// </summary>
		protected virtual void InitializeReticle()
		{
			if (Reticle == null) { return; }
			if (!DisplayReticle) { return; }

			_reticle = (GameObject)Instantiate(Reticle);
			if (_weapon.Owner != null)
			{
				_reticle.transform.SetParent(_weapon.transform);
			}

			_reticle.transform.localPosition = ReticleDistance * Vector3.right;
		}

		/// <summary>
		/// Every frame, moves the reticle if it's been told to follow the pointer
		/// </summary>
		protected virtual void MoveReticle()
		{		
			if (_reticle == null) { return; }

			// if we're not supposed to rotate the reticle, we force its rotation, otherwise we apply the current look rotation
			if (!RotateReticle)
			{
				_reticle.transform.rotation = Quaternion.identity;
			}
			else
			{
				if (ReticleAtMousePosition)
				{
					_reticle.transform.rotation = _lookRotation;
				}
			}

			// if we're in follow mouse mode and the current control scheme is mouse, we move the reticle to the mouse's position
			if (ReticleAtMousePosition && AimControl == AimControls.Mouse)
			{
				_reticle.transform.position = _mainCamera.ScreenToWorldPoint (_mousePosition);
			}
		}

        /// <summary>
        /// Handles the hiding of the reticle and cursor
        /// </summary>
		protected virtual void HideReticle()
		{
            if (DisableReticleOnDeath && (_reticle != null))
            {
                if (_weapon.Owner != null)
                {
                    if (_weapon.Owner.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
                    {
                        _reticle.gameObject.SetActive(false);
                    }
                    else
                    {
                        _reticle.gameObject.SetActive(true);
                    }
                }
            }

            if (GameManager.Instance.Paused)
			{
				Cursor.visible = true;
				return;
			}
			if (ReplaceMousePointer)
			{
				Cursor.visible = false;
			}
			else
			{
				Cursor.visible = true;
			}
		}

		public virtual void AddAdditionalAngle(float addedAngle)
		{
			_additionalAngle += addedAngle;
		}

		protected virtual void ResetAdditionalAngle()
		{
			_additionalAngle = 0;
		}
	}
}
