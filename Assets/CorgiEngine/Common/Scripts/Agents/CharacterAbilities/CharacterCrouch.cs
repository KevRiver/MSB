using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to crouch and crawl
	/// Animator parameters : Crouching, Crawling
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Crouch")] 
	public class CharacterCrouch : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component handles crouch and crawl behaviours. Here you can determine the crouch speed, and whether or not the collider should resize when crouched (to crawl into tunnels for example). If it should, please setup its new size here."; }

		[Header("Crawl")]
		/// if this is set to false, the character won't be able to crawl, just to crouch
		public bool CrawlAuthorized = true;
		/// the speed of the character when it's crouching
		public float CrawlSpeed = 4f;
				
		[Space(10)]	
		[Header("Crouching")]
		/// if this is true, the collider will be resized when crouched
		public bool ResizeColliderWhenCrouched = false;
		/// the size to apply to the collider when crouched (if ResizeColliderWhenCrouched is true, otherwise this will be ignored)
		public Vector2 CrouchedBoxColliderSize = new Vector2(1,1);
		/// if this is true, the character is crouched and has an obstacle over its head that prevents it from getting back up again
		[ReadOnly]
		public bool InATunnel;

        [Header("Cinemachine")]
        public bool MoveCameraTarget = true;
        public Vector3 CameraTargetOffset = new Vector3(0f, -3f, 0f);

        // animation parameters
        protected const string _crouchingAnimationParameterName = "Crouching";
        protected const string _crawlingAnimationParameterName = "Crawling";
        protected int _crouchingAnimationParameter;
        protected int _crawlingAnimationParameter;
        protected bool _wasInATunnelLastFrame;

        /// <summary>
        /// On Start(), we set our tunnel flag to false
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization();
			InATunnel = false;
		}

		/// <summary>
		/// Every frame, we check if we're crouched and if we still should be
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			DetermineState ();
			CheckExitCrouch();
		}

		/// <summary>
		/// At the start of the ability's cycle, we check if we're pressing down. If yes, we call Crouch()
		/// </summary>
		protected override void HandleInput()
		{
			// Crouch Detection : if the player is pressing "down" and if the character is grounded and the crouch action is enabled
			if (_verticalInput < -_inputManager.Threshold.y) 				
			{
				Crouch();
			}
		}

		/// <summary>
		/// If we're pressing down, we check if we can crouch or crawl, and change states accordingly
		/// </summary>
		protected virtual void Crouch()
		{
			if ( !AbilityPermitted // if the ability is not permitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) // or if we're not in our normal stance
				|| (!_controller.State.IsGrounded) // or if we're grounded
				|| (_movement.CurrentState == CharacterStates.MovementStates.Gripping) ) // or if we're gripping
			{
				// we do nothing and exit
				return;
			}

			// if this is the first time we're here, we trigger our sounds
			if ((_movement.CurrentState != CharacterStates.MovementStates.Crouching) && (_movement.CurrentState != CharacterStates.MovementStates.Crawling))
			{
                // we play the crouch start sound 
                PlayAbilityStartFeedbacks();
			}

			// we set the character's state to Crouching and if it's also moving we set it to Crawling
			_movement.ChangeState(CharacterStates.MovementStates.Crouching);
			if ( (Mathf.Abs(_horizontalInput) > 0) && (CrawlAuthorized) )
			{
				_movement.ChangeState(CharacterStates.MovementStates.Crawling);
			}

			// we resize our collider to match the new shape of our character (it's usually smaller when crouched)
			if (ResizeColliderWhenCrouched)
			{
				_controller.ResizeCollider(CrouchedBoxColliderSize);
				Invoke ("RecalculateRays",Time.deltaTime*10);			
			}

			// we change our character's speed
			if (_characterHorizontalMovement != null)
			{
				_characterHorizontalMovement.MovementSpeed = CrawlSpeed;
			}

			// we prevent movement if we can't crawl
			if (!CrawlAuthorized)
			{
				_characterHorizontalMovement.MovementSpeed = 0f;
			}

			// we make our camera look down
			if (_sceneCamera!=null)
			{
				_sceneCamera.LookDown();
            }
            if (MoveCameraTarget)
            {
                _character.SetCameraTargetOffset(CameraTargetOffset);
            }
        }

		/// <summary>
		/// Runs every frame to check if we should switch from crouching to crawling or the other way around
		/// </summary>
		protected virtual void DetermineState()
		{
			if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching) || (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
			{
				if ( (Mathf.Abs(_horizontalInput) > 0) && (CrawlAuthorized) )
				{
					_movement.ChangeState(CharacterStates.MovementStates.Crawling);
				}
				else
				{
					_movement.ChangeState(CharacterStates.MovementStates.Crouching);
				}
			}
		}

		/// <summary>
		/// Every frame, we check to see if we should exit the Crouching (or Crawling) state
		/// </summary>
		protected virtual void CheckExitCrouch()
		{				
            if (_inputManager == null)
            {
                ExitCrouch();
            }

            if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching)
                || (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
            {
                // but we're not pressing down anymore, or we're not grounded anymore
                if ((!_controller.State.IsGrounded) || (_verticalInput >= -_inputManager.Threshold.y))
                {
                    ExitCrouch();
                }
            }

            // if we're currently grounded
            if (_wasInATunnelLastFrame && (_movement.CurrentState == CharacterStates.MovementStates.Pushing))
            {
                if ((!_controller.State.IsGrounded) || (_verticalInput >= -_inputManager.Threshold.y))
                {
                    ExitCrouch();
                }
            }			
		}

        /// <summary>
        /// Exits the crouched state
        /// </summary>
        protected virtual void ExitCrouch()
        {
            // we cast a raycast above to see if we have room enough to go back to normal size
            InATunnel = !_controller.CanGoBackToOriginalSize();
            _wasInATunnelLastFrame = InATunnel;
            // if the character is not in a tunnel, we can go back to normal size
            if (!InATunnel)
            {
                // we return to normal walking speed
                if (_characterHorizontalMovement != null)
                {
                    _characterHorizontalMovement.ResetHorizontalSpeed();
                }

                if (_sceneCamera != null)
                {
                    _sceneCamera.ResetLookUpDown();
                }
                if (MoveCameraTarget)
                {
                    _character.SetCameraTargetOffset(Vector3.zero);
                }

                // we play our exit feedback
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();

                // we go back to Idle state and reset our collider's size
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                _controller.ResetColliderSize();
                Invoke("RecalculateRays", Time.deltaTime * 10);
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("Crouching", AnimatorControllerParameterType.Bool, out _crouchingAnimationParameter);
			RegisterAnimatorParameter ("Crawling", AnimatorControllerParameterType.Bool, out _crawlingAnimationParameter);
		}

		/// <summary>
		/// At the end of the ability's cycle, we send our current crouching and crawling states to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crouchingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Crouching), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crawlingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Crawling), _character._animatorParameters);
		}

		/// <summary>
		/// Recalculates the raycast's origin points.
		/// </summary>
		protected virtual void RecalculateRays()
		{
			_character.RecalculateRays();
		}
	}
}