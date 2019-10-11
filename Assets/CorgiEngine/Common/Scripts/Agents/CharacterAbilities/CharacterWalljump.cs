using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a Character and it'll be able to walljump
    /// Animator parameters : WallJumping (bool)
    /// </summary>
    [HiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Walljump")] 
	public class CharacterWalljump : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to perform an extra jump while wall clinging only. Here you can determine the force to apply to that jump."; }

		[Header("Walljump")]
		/// the force of a walljump
		public Vector2 WallJumpForce = new Vector2(10,4);
        /// returns true if a walljump happened this frame
        public bool WallJumpHappenedThisFrame { get; set; }

		protected CharacterJump _characterJump;

        // animation parameters
        protected const string _wallJumpingAnimationParameterName = "WallJumping";
        protected int _wallJumpingAnimationParameter;

        /// <summary>
        /// On start, we store our characterJump component
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization();
			_characterJump = GetComponent<CharacterJump>();
		}

		/// <summary>
		/// Every frame, we chack if we're pressing the jump button
		/// </summary>
		protected override void HandleInput()
		{
            WallJumpHappenedThisFrame = false;

            if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				Walljump();
			}
		}

		/// <summary>
		/// Performs a walljump if the conditions are met
		/// </summary>
		protected virtual void Walljump()
		{
			if (!AbilityPermitted
				|| _condition.CurrentState != CharacterStates.CharacterConditions.Normal)
			{
				return;
			}

			// wall jump
			float wallJumpDirection;

			// if we're here the jump button has been pressed. If we were wallclinging, we walljump
			if (_movement.CurrentState == CharacterStates.MovementStates.WallClinging)
			{
				_movement.ChangeState(CharacterStates.MovementStates.WallJumping);

				// we decrease the number of jumps left
				if (_characterJump != null)
				{
					_characterJump.SetNumberOfJumpsLeft(_characterJump.NumberOfJumpsLeft-1);
					_characterJump.SetJumpFlags();
					// we start our sounds
					PlayAbilityStartFeedbacks();
				}

				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				_controller.GravityActive(true);
				_controller.SlowFall (0f);	

				// If the character is colliding to the right with something (probably the wall)
				if (_character.IsFacingRight)
				{
					wallJumpDirection=-1f;
				}
				else
				{					
					wallJumpDirection=1f;
				}

				Vector2 walljumpVector = new Vector2(
										wallJumpDirection*WallJumpForce.x,
										Mathf.Sqrt( 2f * WallJumpForce.y * Mathf.Abs(_controller.Parameters.Gravity))
				);
				_controller.AddForce(walljumpVector);
                PlayAbilityStartFeedbacks();
                WallJumpHappenedThisFrame = true;

                return;
			}
		}
        
        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_wallJumpingAnimationParameterName, AnimatorControllerParameterType.Bool, out _wallJumpingAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our character's animator the current walljumping status
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _wallJumpingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.WallJumping), _character._animatorParameters);
		}
	}
}
