using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this ability to a Character to have it handle horizontal movement (walk, and potentially run, crawl, etc)
	/// Animator parameters : Speed (float), Walking (bool)
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Horizontal Movement")] 
	public class CharacterHorizontalMovement : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component handles basic left/right movement, friction, and ground hit detection. Here you can define standard movement speed, walk speed, and what effects to use when the character hits the ground after a jump/fall."; }

		/// the current reference movement speed
		public float MovementSpeed { get; set; }
		[Header("Speed")]
		/// the speed of the character when it's walking
		public float WalkSpeed = 6f;
        /// the multiplier to apply to the horizontal movement
        [ReadOnly]
        public float MovementSpeedMultiplier = 1f;
        [ReadOnly]
        public float PushSpeedMultiplier = 1f;
        /// the current horizontal movement force
		public float HorizontalMovementForce { get { return _horizontalMovementForce; }}
        /// if this is true, movement will be forbidden (as well as flip)
        public bool MovementForbidden { get; set; }

        [Header("Input")]
        /// if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement)
        public bool InstantAcceleration = false;
        /// the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)
        public float InputThreshold = 0.1f;

        [Header("Touching the Ground")]
        /// the MMFeedbacks to play when the character hits the ground
        public MMFeedbacks TouchTheGroundFeedback;
                
		protected float _horizontalMovement;
		protected float _horizontalMovementForce;
	    protected float _normalizedHorizontalSpeed;

        // animation parameters
        protected const string _speedAnimationParameterName = "Speed";
        protected const string _walkingAnimationParameterName = "Walking";
        protected int _speedAnimationParameter;
        protected int _walkingAnimationParameter;

        /// <summary>
        /// On Initialization, we set our movement speed to WalkSpeed.
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization ();
			MovementSpeed = WalkSpeed;
			MovementSpeedMultiplier = 1f;
            MovementForbidden = false;
		}

	    /// <summary>
	    /// The second of the 3 passes you can have in your ability. Think of it as Update()
	    /// </summary>
		public override void ProcessAbility()
	    {
			base.ProcessAbility();

			HandleHorizontalMovement();
	    }

	    /// <summary>
	    /// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls
	    /// methods if conditions are met
	    /// </summary>
	    protected override void HandleInput()
	    {
			_horizontalMovement = _horizontalInput;
	    }

		/// <summary>
		/// Sets the horizontal move value.
		/// </summary>
		/// <param name="value">Horizontal move value, between -1 and 1 - positive : will move to the right, negative : will move left </param>
		public virtual void SetHorizontalMove(float value)
		{
			_horizontalMovement = value;
		}

		/// <summary>
	    /// Called at Update(), handles horizontal movement
	    /// </summary>
	    protected virtual void HandleHorizontalMovement()
		{	
			// if we're not walking anymore, we stop our walking sound
			if ((_movement.CurrentState != CharacterStates.MovementStates.Walking) && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
			}
                        
            // if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
            if ( !AbilityPermitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
			{
				return;
            }

            // check if we just got grounded
            CheckJustGotGrounded();

            if (MovementForbidden)
            {
                _horizontalMovement = _controller.Speed.x * Time.deltaTime;
            }

            // If the value of the horizontal axis is positive, the character must face right.
            if (_horizontalMovement > InputThreshold)
			{
				_normalizedHorizontalSpeed = _horizontalMovement;
				if (!_character.IsFacingRight)
                {
                    _character.Flip();
                }					
			}		
			// If it's negative, then we're facing left
			else if (_horizontalMovement < -InputThreshold)
			{
				_normalizedHorizontalSpeed = _horizontalMovement;
				if (_character.IsFacingRight)
                {
                    _character.Flip();
                }					
			}
			else
			{
				_normalizedHorizontalSpeed = 0;
			}

            /// if we're dashing, we stop there
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                return;
            }

			// if we're grounded and moving, and currently Idle, Running or Dangling, we become Walking
			if ( (_controller.State.IsGrounded)
				&& (_normalizedHorizontalSpeed != 0)
				&& ( (_movement.CurrentState == CharacterStates.MovementStates.Idle)
					|| (_movement.CurrentState == CharacterStates.MovementStates.Dangling) ))
			{				
				_movement.ChangeState(CharacterStates.MovementStates.Walking);
                PlayAbilityStartFeedbacks();	
			}

			// if we're walking and not moving anymore, we go back to the Idle state
			if ((_movement.CurrentState == CharacterStates.MovementStates.Walking) 
				&& (_normalizedHorizontalSpeed == 0))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
                PlayAbilityStopFeedbacks();
			}

			// if the character is not grounded, but currently idle or walking, we change its state to Falling
			if (!_controller.State.IsGrounded
				&& (
					(_movement.CurrentState == CharacterStates.MovementStates.Walking)
					 || (_movement.CurrentState == CharacterStates.MovementStates.Idle)
					))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Falling);
			}

            if (InstantAcceleration)
            {
                if (_normalizedHorizontalSpeed > 0f) { _normalizedHorizontalSpeed = 1f; }
                if (_normalizedHorizontalSpeed < 0f) { _normalizedHorizontalSpeed = -1f; }
            }

			// we pass the horizontal force that needs to be applied to the controller.
			float movementFactor = _controller.State.IsGrounded ? _controller.Parameters.SpeedAccelerationOnGround : _controller.Parameters.SpeedAccelerationInAir;
			float movementSpeed = _normalizedHorizontalSpeed * MovementSpeed * _controller.Parameters.SpeedFactor * MovementSpeedMultiplier * PushSpeedMultiplier;
                        
            if (InstantAcceleration && _controller.State.IsGrounded)
            {
                // if we are in instant acceleration mode, we just apply our movement speed
                _horizontalMovementForce = movementSpeed;

                // and any external forces that may be active right now
                if (Mathf.Abs(_controller.ExternalForce.x) > 0)
                {
                    _horizontalMovementForce += _controller.ExternalForce.x;
                }                
            }
            else
            {
                // if we are not in instant acceleration mode, we lerp towards our movement speed
                _horizontalMovementForce = Mathf.Lerp(_controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);
            }			
						
			// we handle friction
			_horizontalMovementForce = HandleFriction(_horizontalMovementForce);

			// we set our newly computed speed to the controller
            _controller.SetHorizontalForce(_horizontalMovementForce);            			
		}

		/// <summary>
		/// Every frame, checks if we just hit the ground, and if yes, changes the state and triggers a particle effect
		/// </summary>
		protected virtual void CheckJustGotGrounded()
		{
			// if the character just got grounded
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                return;
            }

			if (_controller.State.JustGotGrounded)
			{
                if (_controller.State.ColliderResized)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Crouching);
                }
                else
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
				
				_controller.SlowFall (0f);
                TouchTheGroundFeedback?.PlayFeedbacks();

            }
		}

		/// <summary>
		/// Handles surface friction.
		/// </summary>
		/// <returns>The modified current force.</returns>
		/// <param name="force">the force we want to apply friction to.</param>
		protected virtual float HandleFriction(float force)
		{
			// if we have a friction above 1 (mud, water, stuff like that), we divide our speed by that friction
			if (_controller.Friction>1)
			{
				force = force/_controller.Friction;
			}

			// if we have a low friction (ice, marbles...) we lerp the speed accordingly
			if (_controller.Friction<1 && _controller.Friction > 0)
			{
				force = Mathf.Lerp(_controller.Speed.x, force, Time.deltaTime * _controller.Friction * 10);
			}

			return force;
		}

		/// <summary>
		/// A public method to reset the horizontal speed
		/// </summary>
		public virtual void ResetHorizontalSpeed()
		{
			MovementSpeed = WalkSpeed;
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_speedAnimationParameterName, AnimatorControllerParameterType.Float, out _speedAnimationParameter);
			RegisterAnimatorParameter (_walkingAnimationParameterName, AnimatorControllerParameterType.Bool, out _walkingAnimationParameter);
		}

		/// <summary>
		/// Sends the current speed and the current value of the Walking state to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _speedAnimationParameter, Mathf.Abs(_normalizedHorizontalSpeed), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Walking), _character._animatorParameters);
		}
        
        /// <summary>
        /// When the character gets revived we reinit it again
        /// </summary>
		protected virtual void OnRevive()
		{
			Initialization ();
		}

		/// <summary>
		/// When the player respawns, we reinstate this agent.
		/// </summary>
		/// <param name="checkpoint">Checkpoint.</param>
		/// <param name="player">Player.</param>
		protected override void OnEnable ()
		{
			base.OnEnable ();
			if (gameObject.MMGetComponentNoAlloc<Health>() != null)
			{
				gameObject.MMGetComponentNoAlloc<Health>().OnRevive += OnRevive;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable ();
			if (_health != null)
			{
				_health.OnRevive -= OnRevive;
			}			
		}
	}
}