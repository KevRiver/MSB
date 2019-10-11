using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to look up
	/// Animator parameters : LookingUp
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Look Up")] 
	public class CharacterLookUp : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to look up when pressing up while grounded. How much the camera will move in this situation is defined on the CameraController's inspector. And here you can specify how much horizontal movement is too much to prevent looking up."; }
        [Header("Input")]
		public float HorizontalInputThreshold = 0.5f;

        [Header("Cinemachine")]
        public bool MoveCameraTarget = true;
        public Vector3 CameraTargetOffset = new Vector3(0f, 6f, 0f);

        protected bool _lookingUp = false;

        // animation parameters
        protected const string _lookingUpAnimationParameterName = "LookingUp";
        protected int _lookingUpAnimationParameter;
        
        /// <summary>
        /// Every frame, we check the input to 
        /// </summary>
        protected override void HandleInput()
		{
			if (_verticalInput > _inputManager.Threshold.y) 				
			{
				LookUp();
			}
		}

		/// <summary>
		/// Sets the character in looking up state and asks the camera to look up
		/// </summary>
		protected virtual void LookUp()
		{
            if (!AbilityPermitted // if the ability is not permitted
				|| (!_controller.State.IsGrounded) // or if we're not grounded
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) // or if we're not in a normal condition
                || (_movement.CurrentState == CharacterStates.MovementStates.Jumping) // or if we're jumping
				|| (_movement.CurrentState == CharacterStates.MovementStates.WallJumping) // or if we're wall jumping
				|| (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing) // or if we're wall jumping
				|| (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
				|| (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
				|| (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
				|| (Mathf.Abs(_horizontalInput) > HorizontalInputThreshold)) // or if we're moving horizontally
			{
				return;
			}

            if (_movement.CurrentState != CharacterStates.MovementStates.LookingUp)
            {
                PlayAbilityStartFeedbacks();
            }

            // we set our current state to LookingUp
            _movement.ChangeState(CharacterStates.MovementStates.LookingUp) ;

			// if we have a camera, we make it look up
			if (_sceneCamera!=null)
			{	
				_sceneCamera.LookUp();
			}
            if (MoveCameraTarget)
            {
                _character.SetCameraTargetOffset(CameraTargetOffset);
            }            

            _lookingUp = true;
        }

		/// <summary>
		/// Every frame, we check to see if we should exit the lookup state
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			ExitLookUp();
		}

		/// <summary>
		/// Checks to see if we should exit the LookUp state
		/// </summary>
		protected virtual void ExitLookUp()
		{
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                return;
            }

            // if we're in the Lookup state
            if ( (_movement.CurrentState == CharacterStates.MovementStates.LookingUp)	
            || ((_movement.CurrentState != CharacterStates.MovementStates.LookingUp) && _lookingUp)
            )
			{
				// if we're not pressing up anymore, or if we're not grounded anymore
				if ( (_verticalInput <= _inputManager.Threshold.y) 
					|| (!_controller.State.IsGrounded) 
					|| (Mathf.Abs(_horizontalInput) > HorizontalInputThreshold))
				{					
					// we reset the camera's position
					if (_sceneCamera!=null)
					{
						_sceneCamera.ResetLookUpDown();
                    }
                    if (MoveCameraTarget)
                    {
                        _character.SetCameraTargetOffset(Vector3.zero);
                    }

                    StopStartFeedbacks();
                    PlayAbilityStopFeedbacks();

                    // we restore the previous state
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);

                    _lookingUp = false;
                }
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_lookingUpAnimationParameterName, AnimatorControllerParameterType.Bool, out _lookingUpAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our current LookingUp status to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _lookingUpAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.LookingUp), _character._animatorParameters);
		}
	}
}