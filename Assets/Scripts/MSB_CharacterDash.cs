using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
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
        /// the minimum amount of input required to apply a direction to the dash
        public float MinimumInputThreshold = 0.1f;
        /// if this is true, the character will flip when dashing and facing the dash's opposite direction
        public bool FlipCharacterIfNeeded = true;
        /// if this is true, will prevent the character from dashing into the ground when already grounded
        public bool AutoCorrectTrajectory = true;

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 dashes (in seconds)
		public float DashCooldown = 1f;

        [Header("Dash Setting")]
        public float DashThresholdX = 0.05f;
        public GameObject DashTrail;

        [Header("Damage Setting")] 
        public float InitialDelay;
        //public float ActiveDuration;
        public Vector3 AreaOffset;
        public int CausedDamage;
        public Vector2 KnockbackForce;
        public LayerMask TargetLayerMask;
        public DamageOnTouch.KnockbackDirections KnockbackDirection;
        public DamageOnTouch.KnockbackStyles KnockbackStyle;
        public CausedCCType ccType;
        public float ccDuration;
        public float radius = 1;
        private CircleCollider2D _circleCollider2D;
        private MSB_DamageOnTouch _damageOnTouch;
        private GameObject _damageArea;
        

		protected float _cooldownTimeStamp = 0;
		protected float _startTime ;
		protected Vector3 _initialPosition ;
        protected Vector2 _dashDirection;
		protected float _distanceTraveled = 0f;
		protected bool _shouldKeepDashing = true;
		protected float _slopeAngleSave = 0f;
		protected bool _dashEndedNaturally = true;
        protected IEnumerator _dashCoroutine;
        private Transform _characterModel;
        private RCReciever _rcReciever;


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
	        _characterModel = transform.GetChild(0);
	        Debug.LogWarning("_characterModel : " + _characterModel.gameObject.name);
	        _rcReciever = GetComponent<RCReciever>();
	        if(DashTrail.activeInHierarchy)
		        DashTrail.SetActive(false);
	        CreateDamageArea();
	        DisableDamageArea();
        }

        private void CreateDamageArea()
        {
	        _damageArea = new GameObject();
	        _damageArea.name = this.name + "DamageArea";
	        _damageArea.transform.position = this.transform.position;
	        _damageArea.transform.rotation = this.transform.rotation;
	        _damageArea.transform.SetParent(this.transform);
	        
	        _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
	        _circleCollider2D.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
	        _circleCollider2D.radius = radius;
	        _circleCollider2D.isTrigger = true;
	        
	        Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
	        rigidBody.isKinematic = true;
	        
	        _damageOnTouch = _damageArea.AddComponent<MSB_DamageOnTouch>();
	        _damageOnTouch.TargetLayerMask = TargetLayerMask;
	        _damageOnTouch.CCType = ccType;
	        _damageOnTouch.Owner = gameObject;
	        _damageOnTouch._ownerCharacter = gameObject.GetComponent<MSB_Character>();
	        if (_damageOnTouch._ownerCharacter != null)
	        {
		        foreach (var player in MSB_LevelManager.Instance.Players)
		        {
			        if (player.team == _damageOnTouch._ownerCharacter.team)
				        _damageOnTouch.IgnoreGameObject(player.gameObject);
		        }
	        }

	        _damageOnTouch.DamageCaused = CausedDamage;
	        _damageOnTouch.DamageCausedKnockbackType = KnockbackStyle;
	        _damageOnTouch.DamageCausedKnockbackDirection = KnockbackDirection;
	        _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
	        _damageOnTouch.stunDuration = ccDuration;
        }

        private void EnableDamageArea()
        {
	        _circleCollider2D.enabled = true;
        }

        private void DisableDamageArea()
        {
	        _circleCollider2D.enabled = false;
        }

        /// <summary>
        /// At the start of each cycle, we check if we're pressing the dash button. If we
        /// </summary>
        protected override void HandleInput()
		{
			// 공격 버튼을 누르면 대시 시작
			if (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				StartDash();
			}
		}

        public override void StartAbility()
        {
	        StartDash();
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
			

            // if the character is allowed to dash
            if (_cooldownTimeStamp <= Time.time)
            {
	            if(_character != null && !(((MSB_Character)_character).IsRemote))
		            RCSender.Instance.RequestUserSync();
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
            DashTrail.SetActive(true);
            PlayAbilityStartFeedbacks();

            // we initialize our various counters and checks
            _startTime = Time.time;
            _dashEndedNaturally = false;
            _initialPosition = transform.position;
            _distanceTraveled = 0;
            _shouldKeepDashing = true;
            _cooldownTimeStamp = Time.time + DashCooldown;

            // we prevent our character from going through slopes
            _slopeAngleSave = _controller.Parameters.MaximumSlopeAngle;
            _controller.Parameters.MaximumSlopeAngle = 0;

            ComputeDashDirection();
            //CheckFlipCharacter();

            // we launch the boost corountine with the right parameters
            _dashCoroutine = Dash();
            StartCoroutine(_dashCoroutine);
		}

        /// <summary>
        /// Computes the dash direction based on the selected options
        /// </summary>
        protected virtual void ComputeDashDirection()
        {
	        // 현재 캐릭터가 바라보고 있는 방향 계산
	        _dashDirection = Deg2Vec2(_characterModel.eulerAngles.z, _characterModel.localScale.x);

	        CheckAutoCorrectTrajectory();
            
            if (_dashDirection.magnitude < MinimumInputThreshold)
            {
	            Debug.LogWarning("_dashDirection : " + _dashDirection);
                _dashDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
            }
            else
            {
                _dashDirection = _dashDirection.normalized;
            }
        }

        /// <summary>
        /// 캐릭터 모델이 바라보고 있는 방향의 Vector2 반환
        /// </summary>
        /// <param name="z"> _characterModel's eulerAngles.z</param>
        /// <param name="sx">_characterModel's localScale.x </param>
        /// <returns></returns>
        private Vector2 Deg2Vec2(float z, float sx)
        {
	        //Debug.LogWarning("Deg2Vec2 z : " + z + " sx : " + sx);
	        var rad = z * Mathf.Deg2Rad;
	        var cz = Mathf.Cos(rad);
	        var sz = Mathf.Sin(rad);
	        sx = sx < 0 ? -1 : 1;
	        cz *= sx;
	        sz *= sx;
	        //Debug.LogWarning("Dash Direction : " + "( " + cz*sx + ", " + sz + ")");
	        return new Vector2(cz,sz);
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
			EnableDamageArea();
			// if characteris remote don't move character
			if (_rcReciever != null)
				yield return null;
			
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
                    || _controller.State.IsCollidingAbove)
				{
					_shouldKeepDashing = false;
					_controller.SetForce (Vector2.zero);
				}
				else if (_controller.State.IsCollidingBelow && _dashDirection.y < 0f)
				{
					//땅에 닿고 dashDirection.y가 0보다 작을 때
					// dashDirection.x 유지 ,dashDirection.y = 0 
					if (_dashDirection.x <= DashThresholdX)
					{
						// 대시 방향의 x 값이 문턱을 넘지 않을경우 캐릭터가 보고 있는 방향으로 방향 설정
						float xDirection = _character.IsFacingRight ? 1 : -1;
						_dashDirection = new Vector2(xDirection, 0);
					}
					else
					{
						_dashDirection = Vector2.Scale(_dashDirection, Vector2.right);
					}
					
				}
				else
				{
                    _controller.GravityActive(false);
                    _controller.SetForce(_dashDirection * DashForce);
				}
				yield return null;
			}
            StopDash();
            DisableDamageArea();
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
            DashTrail.SetActive(false);
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

		private void DrawGizmo()
		{
			
		}
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.transform.position + AreaOffset, radius);
		}
}
