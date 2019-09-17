using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// Add this class to a Character and it'll be able to push and/or pull CorgiController equipped objects around. 
    /// Animator parameters : Pushing (bool), Pulling (bool)
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Push")] 
	public class CharacterPushCorgiController : CharacterAbility 
	{
		public override string HelpBoxText() { return "This component allows your character to push blocks. This is not a mandatory component, it will just override CorgiController push settings, and allow you to have a dedicated push animation."; }
        /// if this is true, the user will have to press the Push button to push or pull, otherwise it's automatic on contact
        public bool ButtonBased = false;
		/// If this is set to true, the Character will be able to push blocks
		public bool CanPush = true;
        /// If this is set to true, the Character will be able to pull blocks. Note that this requires ButtonBased to be true.
        public bool CanPull = true; 
		/// if this is true, the Character will only be able to push objects while grounded
		public bool PushWhenGroundedOnly = true;
		/// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
		public float DetectionRaycastLength = 0.2f;
		/// the minimum horizontal speed below which we don't consider the character pushing anymore
		public float MinimumPushSpeed = 0.05f;
        
		protected bool _collidingWithPushable = false;
		protected Vector3 _raycastDirection;
		protected Vector3 _raycastOrigin;
        protected Pushable _pushedObject;
        protected float _movementMultiplierStorage;
        protected bool _pulling = false;
        protected CharacterRun _characterRun;

        /// <summary>
        /// On Start(), we initialize our various flags
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization();
            _characterRun = this.gameObject.GetComponent<CharacterRun>();
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

            CheckForPushEnd();

            if (ButtonBased && (_inputManager.PushButton.State.CurrentState != MMInput.ButtonStates.ButtonPressed))
            {
                return;             
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
				if (hit.collider.gameObject.GetComponentNoAlloc<Pushable>() != null)
                {
                    _collidingWithPushable = true;
				}
            }
            
            if (_controller.State.IsGrounded 
                && _collidingWithPushable 
                && Mathf.Abs(_controller.ExternalForce.x) > MinimumPushSpeed 
                && _movement.CurrentState != CharacterStates.MovementStates.Pushing 
                && _movement.CurrentState != CharacterStates.MovementStates.Jumping)
			{
                if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                {
                    if (_characterRun != null)
                    {
                        _characterRun.RunStop();
                    }
                }
                PlayAbilityStartSfx ();
				PlayAbilityUsedSfx ();
                _movement.ChangeState (CharacterStates.MovementStates.Pushing);
            }

            if (hit && (_movement.CurrentState == CharacterStates.MovementStates.Pushing) && (_pushedObject == null))
            {
                _pushedObject = hit.collider.gameObject.GetComponentNoAlloc<Pushable>();
                _pushedObject.Attach(_controller);
                _character.CanFlip = false;
                _movementMultiplierStorage = _characterHorizontalMovement.PushSpeedMultiplier;
                _characterHorizontalMovement.PushSpeedMultiplier = _pushedObject.PushSpeed;
            }

            if (((_controller.Speed.x > MinimumPushSpeed) 
                && (_movement.CurrentState == CharacterStates.MovementStates.Pushing)
                && (_pushedObject.transform.position.x < this.transform.position.x))
                ||
                ((_controller.Speed.x < -MinimumPushSpeed)
                && (_movement.CurrentState == CharacterStates.MovementStates.Pushing)
                && (_pushedObject.transform.position.x > this.transform.position.x)))
            {
                if (!CanPull)
                {
                    StopPushing();
                }
                else
                {
                    _pulling = true;
                }                
            }
            else
            {
                _pulling = false;
            }
        }

        /// <summary>
        /// Checks whether we should stop pushing and change state
        /// </summary>
        protected virtual void CheckForPushEnd()
        {
            if ((_pushedObject != null) && _inputManager.PushButton.State.CurrentState != MMInput.ButtonStates.ButtonPressed)
            {
                StopPushing();
            }

            if (!_collidingWithPushable && (_movement.CurrentState == CharacterStates.MovementStates.Pushing))
            {
                StopPushing();
            }

            if (((_pushedObject == null) && _movement.CurrentState == CharacterStates.MovementStates.Pushing)
                || ((_pushedObject != null) && Mathf.Abs(_controller.Speed.x) <= MinimumPushSpeed && _movement.CurrentState == CharacterStates.MovementStates.Pushing))
            {
                // we reset the state
                _movement.ChangeState(CharacterStates.MovementStates.Idle);

                PlayAbilityStopSfx();
                StopAbilityUsedSfx();
            }

            if (_movement.CurrentState != CharacterStates.MovementStates.Pushing && _abilityInProgressSfx != null)
            {
                PlayAbilityStopSfx();
                StopAbilityUsedSfx();
            }
        }

        /// <summary>
        /// Stops the character from pushing or pulling
        /// </summary>
        protected virtual void StopPushing()
        {
            if (_pushedObject == null)
            {
                return;
            }
            _pushedObject.Detach(_controller);
            _pushedObject = null;
            _character.CanFlip = true;
            _characterHorizontalMovement.PushSpeedMultiplier = _movementMultiplierStorage;
            _pulling = false;
        }

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter("Pushing", AnimatorControllerParameterType.Bool);
            RegisterAnimatorParameter("Pulling", AnimatorControllerParameterType.Bool);
        }

		/// <summary>
		/// Sends the current state of the Diving state to the character's animator
		/// </summary>
		public override void UpdateAnimator()
        {
            MMAnimator.UpdateAnimatorBool(_animator, "Pushing", (_movement.CurrentState == CharacterStates.MovementStates.Pushing), _character._animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "Pulling", _pulling, _character._animatorParameters);
        }
	}
}