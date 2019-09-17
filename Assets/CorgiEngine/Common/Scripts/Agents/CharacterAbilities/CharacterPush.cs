using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// Add this class to a Character and it'll be able to push objects around. It's an optional class as you can push objects without it,
	/// but it'll allow you to have a "pushing" animation. For the animation to work, your pushable objects will need to have a 
	/// Pushable component.
	/// Animator parameters : Pushing (bool)
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Push")] 
	public class CharacterPush : CharacterAbility 
	{
		public override string HelpBoxText() { return "This component allows your character to push blocks. This is not a mandatory component, it will just override CorgiController push settings, and allow you to have a dedicated push animation."; }
		/// If this is set to true, the Character will be able to push blocks
		public bool CanPush = true;
		/// the (x) force applied to the pushed object
		public float PushForce = 2f;
		/// if this is true, the Character will only be able to push objects while grounded
		public bool PushWhenGroundedOnly = true;
		/// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
		public float DetectionRaycastLength = 0.2f;
		/// the minimum horizontal speed below which we don't consider the character pushing anymore
		public float MinimumPushSpeed = 0.05f;


		protected bool _collidingWithPushable = false;
		protected Vector3 _raycastDirection;
		protected Vector3 _raycastOrigin;

		/// <summary>
		/// On Start(), we initialize our various flags
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_controller.Parameters.Physics2DInteraction = CanPush;
			_controller.Parameters.Physics2DPushForce = PushForce;
		}

		/// <summary>
		/// Every frame we override parameters if needed and cast a ray to see if we're actually pushing anything
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if (!CanPush || !AbilityPermitted)
			{
				return;
			}
			// if we can only push when grounded and we're not grounded we turn our push force off
			if (PushWhenGroundedOnly && !_controller.State.IsGrounded)
			{
				_controller.Parameters.Physics2DPushForce = 0f;
				return;
			}
			else
			{
				_controller.Parameters.Physics2DPushForce = PushForce;
			}

			// we set our flag to false
			_collidingWithPushable = false;

			// we cast a ray in front of us to see if we're colliding with a pushable object
			_raycastDirection = _character.IsFacingRight ? transform.right : -transform.right;
			_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width()/2 );


			// we cast our ray to see if we're hitting something
			RaycastHit2D hit = MMDebug.RayCast (_raycastOrigin,_raycastDirection,DetectionRaycastLength,_controller.PlatformMask,Color.green,_controller.Parameters.DrawRaycastsGizmos);			
			if (hit)
			{
				if (hit.collider.GetComponent<Pushable>() != null)
				{
					_collidingWithPushable = true;
				}
			}

			if (_controller.State.IsGrounded && _collidingWithPushable && Mathf.Abs(_controller.Speed.x) > MinimumPushSpeed && _movement.CurrentState != CharacterStates.MovementStates.Pushing && _movement.CurrentState != CharacterStates.MovementStates.Jumping)
			{
				PlayAbilityStartSfx ();
				PlayAbilityUsedSfx ();
				_movement.ChangeState (CharacterStates.MovementStates.Pushing);
			}

			if ((!_collidingWithPushable && _movement.CurrentState == CharacterStates.MovementStates.Pushing)
				|| (_collidingWithPushable && Mathf.Abs(_controller.Speed.x) <= MinimumPushSpeed && _movement.CurrentState == CharacterStates.MovementStates.Pushing))
			{
				// we reset the state
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				PlayAbilityStopSfx ();
				StopAbilityUsedSfx();
			}

			if (_movement.CurrentState != CharacterStates.MovementStates.Pushing && _abilityInProgressSfx != null)
			{
				PlayAbilityStopSfx ();
				StopAbilityUsedSfx();
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("Pushing", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// Sends the current state of the Diving state to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"Pushing",(_movement.CurrentState == CharacterStates.MovementStates.Pushing),_character._animatorParameters);
		}
	}
}