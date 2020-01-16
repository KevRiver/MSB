using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.XR;

public class MSB_CharacterDash : CharacterAbility
{
    public override string HelpBoxText() { return "This component allows your character to dash. Here you can define the distance the dash should cover, " +
                "how much force to apply during the dash (which impacts its duration), whether forces should be reset on dash exit (otherwise inertia will apply)." +
                "Then you can define how to pick the dash's direction, whether or not the character should be automatically flipped to match the dash's direction, and " +
                "whether or not you want to correct the trajectory to prevent grounded characters to not dash if the input was slightly wrong." +
                "And finally you can tweak the cooldown between the end of a dash and the start of the next one."; }

		[Header("Dash")]
		/// the duration of dash (in seconds)
		public float DashDistance = 3f;
		/// the force of the dash
		public float DashForce = 40f;
        /// if this is true, forces will be reset on dash exit (killing inertia)
        public bool ResetForcesOnExit = false;

        [Header("Direction")]
        /// the dash's aim properties
        public MMAim Aim;
        /// the minimum amount of input required to apply a direction to the dash
        public float MinimumInputThreshold = 0.1f;
        /// if this is true, the character will flip when dashing and facing the dash's opposite direction
        public bool FlipCharacterIfNeeded = true;
        /// if this is true, will prevent the character from dashing into the ground when already grounded
        public bool AutoCorrectTrajectory = true;

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 dashes (in seconds)
		public float DashCooldown = 1f;	

		protected float _cooldownTimeStamp = 0;
		protected float _startTime ;
		protected Vector3 _initialPosition ;
        protected Vector2 _dashDirection;
		protected float _distanceTraveled = 0f;
		protected bool _shouldKeepDashing = true;
		protected float _slopeAngleSave = 0f;
		protected bool _dashEndedNaturally = true;
        protected IEnumerator _dashCoroutine;
        protected CharacterDive _characterDive;

        // animation parameters
        protected const string _dashingAnimationParameterName = "Dashing";
        protected int _dashingAnimationParameter;

        /// <summary>
        /// Initializes our aim instance
        /// </summary>
        protected override void Initialization()
        {
	        _character = GetComponent<MSB_Character>();
	        _controller = GetComponent<CorgiController>();
	        _characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
	        _characterGravity = GetComponent<CharacterGravity> ();
	        _spriteRenderer = GetComponent<SpriteRenderer>();
	        _health = GetComponent<Health> ();
	        BindAnimator();
	        _sceneCamera = _character.SceneCamera;
	        _inputManager = _character.LinkedInputManager;
	        _state = _character.CharacterState;
	        _movement = _character.MovementState;
	        _condition = _character.ConditionState;
	        _abilityInitialized = true;
            Aim.Initialization();
            _characterDive = this.gameObject.GetComponent<CharacterDive>();
        }

        /// <summary>
        /// At the start of each cycle, we check if we're pressing the dash button. If we
        /// </summary>
        protected override void HandleInput()
		{
			if (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				StartDash();
			}
		}

		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			// If the character is dashing, we cancel the gravity
			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing) 
			{
				_controller.GravityActive(false);
			}

			// we reset our slope tolerance if dash didn't end naturally
			if ((!_dashEndedNaturally) && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
			{
				_dashEndedNaturally = true;
				_controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
			}
		}

		/// <summary>
		/// Causes the character to dash or dive (depending on the vertical movement at the start of the dash)
		/// </summary>
		public virtual void StartDash()
		{
			// if the Dash action is enabled in the permissions, we continue, if not we do nothing
			if (!AbilityPermitted 
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
				return;		
			
			// If the user presses the dash button and is not aiming down
            if (_characterDive != null)
            {
                if (_verticalInput < -_inputManager.Threshold.y)
                {
                    return;
                }
            }

            // if the character is allowed to dash
            if (_cooldownTimeStamp <= Time.time)
            {
                InitiateDash();
            }
        }

        /// <summary>
        /// initializes all parameters prior to a dash and triggers the pre dash feedbacks
        /// </summary>
		public virtual void InitiateDash()
		{
			// we set its dashing state to true
			_movement.ChangeState(CharacterStates.MovementStates.Dashing);

            // we start our sounds
            PlayAbilityStartFeedbacks();

            // we initialize our various counters and checks
            _startTime = Time.time;
            _dashEndedNaturally = false;
            _initialPosition = this.transform.position;
            _distanceTraveled = 0;
            _shouldKeepDashing = true;
            _cooldownTimeStamp = Time.time + DashCooldown;

            // we prevent our character from going through slopes
            _slopeAngleSave = _controller.Parameters.MaximumSlopeAngle;
            _controller.Parameters.MaximumSlopeAngle = 0;

            ComputeDashDirection();
            CheckFlipCharacter();

            // we launch the boost corountine with the right parameters
            _dashCoroutine = Dash();
            StartCoroutine(_dashCoroutine);
		}

        /// <summary>
        /// Computes the dash direction based on the selected options
        /// </summary>
        protected virtual void ComputeDashDirection()
        {
	        // we compute our direction
            Aim.PrimaryMovement = _character.LinkedInputManager.PrimaryMovement;
            Aim.SecondaryMovement = _character.LinkedInputManager.SecondaryMovement;
            Aim.CurrentPosition = this.transform.position;
            _dashDirection = Aim.GetCurrentAim();

            CheckAutoCorrectTrajectory();
            
            if (_dashDirection.magnitude < MinimumInputThreshold)
            {
                _dashDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
            }
            else
            {
                _dashDirection = _dashDirection.normalized;
            }
        }

        /// <summary>
        /// Prevents the character from dashing into the ground when already grounded and if AutoCorrectTrajectory is checked
        /// </summary>
        protected virtual void CheckAutoCorrectTrajectory()
        {
            if (AutoCorrectTrajectory && _controller.State.IsCollidingBelow && (_dashDirection.y < 0f))
            {
                _dashDirection.y = 0f;
            }
        }

        /// <summary>
        /// Checks whether or not a character flip is required, and flips the character if needed
        /// </summary>
        protected virtual void CheckFlipCharacter()
        {
            // we flip the character if needed
            if (FlipCharacterIfNeeded && (Mathf.Abs(_dashDirection.x) > 0.05f))
            {
                if (_character.IsFacingRight != (_dashDirection.x > 0f))
                {
                    _character.Flip();
                }
            }
        }

		/// <summary>
		/// Coroutine used to move the player in a direction over time
		/// </summary>
		protected virtual IEnumerator Dash()
        {
            // if the character is not in a position where it can move freely, we do nothing.
            if ( !AbilityPermitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				yield break;
            }            

			// we keep dashing until we've reached our target distance or until we get interrupted
			while (_distanceTraveled < DashDistance 
                && _shouldKeepDashing 
                && _movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                _distanceTraveled = Vector3.Distance(_initialPosition,this.transform.position);

				// if we collide with something on our left or right (wall, slope), we stop dashing, otherwise we apply horizontal force
				if (_controller.State.IsCollidingLeft 
                    || _controller.State.IsCollidingRight 
                    || _controller.State.IsCollidingAbove 
                    || (_controller.State.IsCollidingBelow && _dashDirection.y < 0f))
				{
					_shouldKeepDashing = false;
					_controller.SetForce (Vector2.zero);
				}
				else
				{
                    _controller.GravityActive(false);
                    _controller.SetForce(_dashDirection * DashForce);
				}
				yield return null;
			}
            StopDash();				
		}

        /// <summary>
        /// Stops the dash coroutine and resets all necessary parts of the character
        /// </summary>
        public virtual void StopDash()
        {
            StopCoroutine(_dashCoroutine);

            // once our dash is complete, we reset our various states
            _controller.DefaultParameters.MaximumSlopeAngle = _slopeAngleSave;
            _controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
            _controller.GravityActive(true);
            _dashEndedNaturally = true;

            // we reset our forces
            if (ResetForcesOnExit)
            {
                _controller.SetForce(Vector2.zero);
            }

            // we play our exit sound
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();

            // once the boost is complete, if we were dashing, we make it stop and start the dash cooldown
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                _movement.RestorePreviousState();
            }
        }

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_dashingAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashingAnimationParameter);
		}

		/// <summary>
		/// At the end of the cycle, we update our animator's Dashing state 
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Dashing), _character._animatorParameters);
		}

}
