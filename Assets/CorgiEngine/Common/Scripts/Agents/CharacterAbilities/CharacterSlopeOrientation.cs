using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a Character and it'll rotate according to the current slope angle.
	/// Animator parameters : none
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Slope Orientation")] 
	public class CharacterSlopeOrientation : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component will orient the character's model to it is perpendicular to the slope it's walking on. Note that this only works if your model is not on the top level of your character, but instead nested under it."; }
        /// The object to rotate when walking on slopes. A good hierarchy is like so :
        /// - top level : Corgi Controller, collider, character, abilities, etc
        /// - - slope object to rotate
        /// - - - model 
        public GameObject ObjectToRotate;

		[Header("Rotation")]
		[Information("Here you can define the speed at which the character should rotate to be perpendicular to the slope. 0 means instant rotation, low value is slow, high value is fast, 10 is the default. You can also specify minimum and maximum angles at which your character's rotation will be clamped.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
        /// the rotation at which to rotate the object
        public float CharacterRotationSpeed = 10f;
        /// the minimum angle the rotation will be clamped at
		public float MinimumAllowedAngle = -90f;
        /// the maximum angle the rotation will be clamped at
		public float MaximumAllowedAngle = 90f;
        /// should the rotation be reset when the character jumps
		public bool ResetAngleInTheAir = true;
        ///should the weapon rotate as well
		public bool RotateWeapon = true;

		protected GameObject _model;
		protected Quaternion _newRotation;
		protected float _currentAngle;
		protected CharacterHandleWeapon _handleWeapon;
		protected WeaponAim _weaponAim;

		/// <summary>
		/// On Start(), we set our tunnel flag to false
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();

            if (ObjectToRotate != null)
            {
                _model = ObjectToRotate;
            }
            else
            {
                _model = _character.CharacterModel;
            }			

			_handleWeapon = GetComponent<CharacterHandleWeapon> ();
			if (_handleWeapon != null)
			{
				if (_handleWeapon.CurrentWeapon != null)
				{
					_weaponAim = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim> ();
				}
			}
		}

		/// <summary>
		/// Every frame, we check if we're crouched and if we still should be
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			// if we don't have a model, we do nothing and exit
			if (_model == null)
			{
				return;
			}

			// we get the current angle between the character and the slope it's on from the controller
			_currentAngle = _controller.State.BelowSlopeAngle;
			// if we're in the air and if we should be resetting the angle, we reset it
			if ((!_controller.State.IsGrounded) && ResetAngleInTheAir)
			{
				_currentAngle = 0;
			}

			// we clamp our angle
			_currentAngle = Mathf.Clamp(_currentAngle, MinimumAllowedAngle, MaximumAllowedAngle);

			if (_characterGravity != null)
			{
				_currentAngle += _characterGravity.GravityAngle;
			}

			// we determine the new rotation
			_newRotation = Quaternion.Euler (_currentAngle * Vector3.forward);

			// if we want instant rotation, we apply it directly
			if (CharacterRotationSpeed == 0)
			{
				_model.transform.rotation = _newRotation;	
			}
			// otherwise we lerp the rotation
			else
			{				
				_model.transform.rotation = Quaternion.Lerp (_model.transform.rotation, _newRotation, CharacterRotationSpeed * Time.deltaTime);
			}

			// if we're supposed to also rotate the weapon
			if (RotateWeapon && (_weaponAim != null))
			{
				if (_characterGravity != null)
				{
					_currentAngle -= _characterGravity.GravityAngle;
				}
				_weaponAim.AddAdditionalAngle (_currentAngle);
			}
		}
	}
}