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
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Handle Secondary Weapon")] 
	public class CharacterHandleSecondaryWeapon : CharacterHandleWeapon 
	{
		/// <summary>
		/// Gets input and triggers methods based on what's been pressed
		/// </summary>
		protected override void HandleInput ()
		{			

			if ((_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) || (ContinuousPress && (CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto) && (_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)))
			{
				ShootStart();
			}

			if ((_inputManager.SecondaryShootAxis == MMInput.ButtonStates.ButtonDown) || (ContinuousPress && (CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto) && (_inputManager.SecondaryShootAxis == MMInput.ButtonStates.ButtonPressed)))
			{
				ShootStart();
			}

			if (_inputManager.ReloadButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				Reload();
            }

            if ((_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) || (_inputManager.SecondaryShootAxis == MMInput.ButtonStates.ButtonUp))
            {
                ShootStop();
            }

            if (CurrentWeapon != null)
            {
                if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses)
                && ((_inputManager.SecondaryShootAxis == MMInput.ButtonStates.Off) && (_inputManager.SecondaryShootButton.State.CurrentState == MMInput.ButtonStates.Off)))
                {
                    ShootStop();
                }
            }            
        }				
    }
}