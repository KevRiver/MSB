using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a character and it'll adopt a dangling stance if facing a hole in the ground
	/// Animator parameters : Dangling
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Dangling")] 
	public class CharacterDangling : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "Add this component to a character and it'll adopt a dangling stance if facing a hole in the ground. The detection is done using a raycast, whose origin and length can be setup here."; }

		[Header("Dangling")]
		/// the origin of the raycast used to detect pits. This is relative to the transform.position of our character
		public Vector3 DanglingRaycastOrigin=new Vector3(0.7f,-0.25f,0f);
		/// the length of the raycast used to detect pits
		public float DanglingRaycastLength=2f;

		protected Vector3 _leftOne = new Vector3(-1,1,1);

		/// <summary>
		/// Every frame, we check to see if there's a hole in front of us
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			Dangling();
		}

		/// <summary>
		/// Casts a ray in front of the character and going downwards. If the ray hits nothing, we're close to an edge and start dangling.
		/// </summary>
		protected virtual void Dangling()
		{
			// if we're dangling and not grounded, we change our state to Falling
			if (!_controller.State.IsGrounded && (_movement.CurrentState == CharacterStates.MovementStates.Dangling))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Falling);
			}

			// if dangling is disabled or if we're not grounded, we do nothing and exit
			if (!AbilityPermitted 
			|| (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
			|| (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
			|| (_movement.CurrentState == CharacterStates.MovementStates.LookingUp)
			|| (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
			|| !_controller.State.IsGrounded)
			{
				return;
			}

			// we determine the ray's origin (our character's position + an offset defined in the inspector)
			Vector3 raycastOrigin = Vector3.zero;
			if (_character.IsFacingRight)
			{ 
				raycastOrigin = transform.position + DanglingRaycastOrigin.x * transform.right + DanglingRaycastOrigin.y * transform.up;
			}
			else
			{
				raycastOrigin = transform.position - DanglingRaycastOrigin.x * transform.right + DanglingRaycastOrigin.y * transform.up;
			}

			// we cast our ray downwards
			RaycastHit2D hit = MMDebug.RayCast (raycastOrigin,-transform.up,DanglingRaycastLength,_controller.PlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask,Color.gray,_controller.Parameters.DrawRaycastsGizmos);			

			// if the ray didn't hit something, we're dangling
			if (!hit)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Dangling) ; 			
			}

			// if the ray hit something and we were dangling previously, we go back to Idle
			if (hit && (_movement.CurrentState == CharacterStates.MovementStates.Dangling) )
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("Dangling", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// At the end of each cycle, sends our dangling current state to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"Dangling",(_movement.CurrentState == CharacterStates.MovementStates.Dangling), _character._animatorParameters);
		}
	}
}