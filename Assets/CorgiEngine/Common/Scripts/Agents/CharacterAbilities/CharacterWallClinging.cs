using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a Character and it'll be able to cling to walls when being in the air, 
	// facing a wall, and moving in its direction
	/// Animator parameters : WallClinging (bool)
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Wallclinging")] 
	public class CharacterWallClinging : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "Add this component to your character and it'll be able to cling to walls, slowing down its fall. Here you can define the slow factor (close to 0 : super slow, 1 : normal fall) and the tolerance (to account for tiny holes in the wall."; }

		[Header("Wall Clinging")]
		[Range(0.01f,1)]
		/// the slow factor when wall clinging
		public float WallClingingSlowFactor=0.6f;
		/// the tolerance applied to compensate for tiny irregularities in the wall (slightly misplaced tiles for example)
		public float WallClingingTolerance = 0.3f;
        
		/// <summary>
		/// Checks the input to see if we should enter the WallClinging state
		/// </summary>
		protected override void HandleInput()
		{
			// if we're pressing left or right
			if ( (_horizontalInput <= -_inputManager.Threshold.x) || (_horizontalInput >= _inputManager.Threshold.x) ) 
			{
				WallClinging();
			}
		}

		/// <summary>
		/// Every frame, checks if the wallclinging state should be exited
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			ExitWallClinging();
			WallClingingLastFrame ();
		}

		/// <summary>
	    /// Makes the player stick to a wall when jumping
	    /// </summary>
	    protected virtual void WallClinging()
		{
			if (!AbilityPermitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
				|| (_controller.State.IsGrounded)
                || (_controller.State.ColliderResized)
                || (_controller.Speed.y >= 0) )
			{
				return;
			}

			if ( ( (_controller.State.IsCollidingLeft) && (_horizontalInput <= -_inputManager.Threshold.x) ) 
				|| ( (_controller.State.IsCollidingRight) && (_horizontalInput >= _inputManager.Threshold.x)))
			{
				// we check for an override
				WallClingingOverride wallClingingOverride = _controller.CurrentWallCollider.GetComponent<WallClingingOverride> ();
				if (wallClingingOverride != null)
				{
					// if we can't wallcling to this wall, we do nothing and exit
					if (!wallClingingOverride.CanWallClingToThis)
					{
						return;
					}
					_controller.SlowFall (wallClingingOverride.WallClingingSlowFactor);	
				}
				else
				{
					// we slow the controller's fall speed
					_controller.SlowFall (WallClingingSlowFactor);	
				}

				// if we weren't wallclinging before this frame, we start our sounds
				if (_movement.CurrentState != CharacterStates.MovementStates.WallClinging)
				{
					// we start our sounds
					PlayAbilityStartSfx();
					PlayAbilityUsedSfx();
				}

				_movement.ChangeState (CharacterStates.MovementStates.WallClinging);							
			}
		}

		/// <summary>
		/// If the character is currently wallclinging, checks if we should exit the state
		/// </summary>
		protected virtual void ExitWallClinging()
		{
			if (_movement.CurrentState == CharacterStates.MovementStates.WallClinging)
			{
				// we prepare a boolean to store our exit condition value
				bool shouldExit = false;
				if ((_controller.State.IsGrounded) // if the character is grounded
					|| (_controller.Speed.y >= 0))  // or if it's moving up
				{
					// then we should exit
					shouldExit = true;
				}

				// we then cast a ray to the direction's the character is facing, in a down diagonal.
				// we could use the controller's IsCollidingLeft/Right for that, but this technique 
				// compensates for walls that have small holes or are not perfectly flat
				Vector3 raycastOrigin=transform.position;
				Vector3 raycastDirection;
				if (_character.IsFacingRight) 
				{ 
					raycastOrigin = raycastOrigin + transform.right * _controller.Width()/2;
					raycastDirection = transform.right - transform.up; 
				}
				else
				{
					raycastOrigin = raycastOrigin - transform.right * _controller.Width()/2;
					raycastDirection = - transform.right - transform.up;
				}

				// we cast our ray 
				RaycastHit2D hit = MMDebug.RayCast (raycastOrigin,raycastDirection,WallClingingTolerance,_controller.PlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask,Color.black,_controller.Parameters.DrawRaycastsGizmos);			

				// we check if the ray hit anything. If it didn't, or if we're not moving in the direction of the wall, we exit
				if (_character.IsFacingRight)
				{
					if ((!hit) || (_horizontalInput <= _inputManager.Threshold.x))
					{
						shouldExit = true;
					}
				}
				else
				{
					if ((!hit) || (_horizontalInput >= -_inputManager.Threshold.x))
					{
						shouldExit = true;
					}
				}

				if (shouldExit)
				{
					// if we're not wallclinging anymore, we reset the slowFall factor, and reset our state.
					_controller.SlowFall (0f);				
					// we reset the state
					_movement.ChangeState(CharacterStates.MovementStates.Idle);

					// we play our exit sound
					StopAbilityUsedSfx();
					PlayAbilityStopSfx();
				}
			}
		}

		/// <summary>
		/// This methods tests if we were wallcling previously, and if so, resets the slowfall factor and stops the wallclinging sound
		/// </summary>
		protected virtual void WallClingingLastFrame()
		{
			if ((_movement.PreviousState == CharacterStates.MovementStates.WallClinging) && (_movement.CurrentState != CharacterStates.MovementStates.WallClinging))
			{
				_controller.SlowFall (0f);	
				StopAbilityUsedSfx();
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("WallClinging", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// Updates the animator with the current wallclinging state
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"WallClinging",(_movement.CurrentState == CharacterStates.MovementStates.WallClinging), _character._animatorParameters);
		}
		
	}
}