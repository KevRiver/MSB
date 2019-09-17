using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// Add this class to a Character and it'll be able to dive by pressing down + dash button, pounding the ground in the process.
	/// Animator parameters : Diving (bool)
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Dive")] 
	public class CharacterDive : CharacterAbility
	{	
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to dive (by pressing the dash button + the down direction while in the air). Here you can define how much the camera should shake on impact, and how fast the dive should be."; }

		/// Shake parameters : intensity, duration (in seconds) and decay
		public Vector3 ShakeParameters = new Vector3(1.5f,0.5f,1f);
		/// the vertical acceleration applied when diving
		public float DiveAcceleration = 2f;

		/// <summary>
		/// Every frame, we check input to see if we should dive
		/// </summary>
		protected override void HandleInput()
		{
			if ((_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
				&& (_verticalInput < -_inputManager.Threshold.y))
			{
				// we start the dive coroutine
				InitiateDive();
			}
		}

		protected virtual void InitiateDive()
		{
			if ( !AbilityPermitted // if the ability is not permitted
				|| (_controller.State.IsGrounded) // or if the character is grounded
				|| (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if it's gripping
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not under normal conditions
			{
				// we do nothing and exit
				return;
			}
			// we start the dive coroutine
			StartCoroutine(Dive());
		}

		/// <summary>
	    /// Coroutine used to make the player dive vertically
	    /// </summary>
	    protected virtual IEnumerator Dive()
		{	
			// we start our sounds
			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();

			// we make sure collisions are on
			_controller.CollisionsOn();
			// we set our current state to Diving
			_movement.ChangeState(CharacterStates.MovementStates.Diving);

			// while the player is not grounded, we force it to go down fast
			while (!_controller.State.IsGrounded)
			{
				_controller.SetVerticalForce(-Mathf.Abs(_controller.Parameters.Gravity)*DiveAcceleration);
				yield return null; //go to next frame
			}
			
			// once the player is grounded, we shake the camera, and restore the diving state to false
			if (_sceneCamera != null)
			{
				_sceneCamera.Shake(ShakeParameters);	
			}

			// we play our exit sound
			StopAbilityUsedSfx();
			PlayAbilityStopSfx();

			_movement.ChangeState(CharacterStates.MovementStates.Idle);
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("Diving", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// Sends the current state of the Diving state to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"Diving",(_movement.CurrentState == CharacterStates.MovementStates.Diving),_character._animatorParameters);
		}
	}
}
