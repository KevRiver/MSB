using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// A class meant to be overridden that handles a character's ability. 
	/// </summary>
	[RequireComponent(typeof(Character))]
	public class CharacterAbility : MonoBehaviour 
	{
        //MSB Custom
        public enum ActionType { NULL, BasicAttack, ActiveSkill }

		/// the sound fx to play when the ability starts
		public AudioClip AbilityStartSfx;
		/// the sound fx to play while the ability is running
		public AudioClip AbilityInProgressSfx;
		/// the sound fx to play when the ability stops
		public AudioClip AbilityStopSfx;
        /// if true, this ability can perform as usual, if not, it'll be ignored. You can use this to unlock abilities over time for example
		public bool AbilityPermitted = true;
        /// true if the ability has already been initialized
		public bool AbilityInitialized { get { return _abilityInitialized; } }

		protected Character _character;
        //MSB Custom
        protected MSB_Character _MSB_character;
        public bool isLocalUser;       

		protected Health _health;
		protected CharacterHorizontalMovement _characterHorizontalMovement;
		protected CorgiController _controller;
		protected InputManager _inputManager;
		protected CameraController _sceneCamera;
		protected Animator _animator;
		protected CharacterStates _state;
		protected SpriteRenderer _spriteRenderer;
		protected MMStateMachine<CharacterStates.MovementStates> _movement;
		protected MMStateMachine<CharacterStates.CharacterConditions> _condition;
		protected AudioSource _abilityInProgressSfx;
		protected bool _abilityInitialized = false;
		protected CharacterGravity _characterGravity;
		protected float _verticalInput;
		protected float _horizontalInput;
        
        //MSB Custom Request된 버튼 ID OnGameUserSync로 0.1초마다 동기화
        public string RecievedButtonID;
        //MSB Custom Request된 버튼 ID의 상태 OnGameUserSync로 0.1초마다 동기화
        public int RecievedButtonState;

		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public virtual string HelpBoxText() { return ""; }

		/// <summary>
		/// On Start(), we call the ability's intialization
		/// </summary>
		protected virtual void Start () 
		{
			Initialization();
		}

		/// <summary>
		/// Gets and stores components for further use
		/// </summary>
		protected virtual void Initialization()
		{
			_character = GetComponent<Character>();
            isLocalUser = false;
            if (GetComponent<MSB_Character>() != null)
            {
                _MSB_character = GetComponent<MSB_Character>();
                isLocalUser = _MSB_character.isLocalUser;
            }
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
		}

        /// <summary>
        /// Sets a new input manager for this ability to get input from
        /// </summary>
        /// <param name="inputManager"></param>
        public virtual void SetInputManager(InputManager inputManager)
        {
            _inputManager = inputManager;
        }

        /// <summary>
        /// Binds the animator from the character and initializes the animator parameters
        /// </summary>
        protected virtual void BindAnimator()
        {
            _animator = _character._animator;
            if (_animator != null)
            {
                InitializeAnimatorParameters();
            }
        }

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected virtual void InitializeAnimatorParameters()
		{

		}

		/// <summary>
		/// Internal method to check if an input manager is present or not
		/// </summary>
		protected virtual void InternalHandleInput()
		{
			if (_inputManager == null) { return; }

            if (!_inputManager.inputPermitted)
                return;

			_verticalInput = _inputManager.PrimaryMovement.y;
			_horizontalInput = _inputManager.PrimaryMovement.x;

			if (_characterGravity != null)
			{
				if (_characterGravity.ShouldReverseInput())
				{
					if (_characterGravity.ReverseVerticalInputWhenUpsideDown)
					{
						_verticalInput = -_verticalInput;
					}
					if (_characterGravity.ReverseHorizontalInputWhenUpsideDown)
					{
						_horizontalInput = -_horizontalInput;
					}	
				}
			}
			HandleInput();
		}

		/// <summary>
		/// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls methods if conditions are met
		/// </summary>
		protected virtual void HandleInput()
		{

		}

        /// <summary>
        /// MSB Custom 원격으로 LocalPlayer가 아닌 플레이어는 HandleInput 대신 MSBRemoteControl 로 조작된다
        /// </summary>
        protected virtual void MSBRemoteControl(ActionType _actionType, bool _actionAimable, float _actionDirX, float _actionDirY)
        {
            if (_actionType == ActionType.NULL)
                return;
            //Debug.LogWarning("MSBRemoteControl Params : " + _actionType + " " + _state);
        }

        protected virtual void ResetRecievedData()
        {

        }

        /// <summary>
        /// Resets all input for this ability. Can be overridden for ability specific directives
        /// </summary>
        public virtual void ResetInput()
        {
            _horizontalInput = 0f;
            _verticalInput = 0f;
        }

		/// <summary>
		/// The first of the 3 passes you can have in your ability. Think of it as EarlyUpdate() if it existed
		/// </summary>
		public virtual void EarlyProcessAbility()
		{
            InternalHandleInput();
            MSBRemoteControl(_MSB_character.RecievedActionType, _MSB_character.RecievedActionAimable,_MSB_character.RecievedActionDirX,_MSB_character.RecievedActionDirY);
		}

		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// </summary>
		public virtual void ProcessAbility()
		{
			
		}

		/// <summary>
		/// The last of the 3 passes you can have in your ability. Think of it as LateUpdate()
		/// </summary>
		public virtual void LateProcessAbility()
		{
			
		}

		/// <summary>
		/// Override this to send parameters to the character's animator. This is called once per cycle, by the Character class, after Early, normal and Late process().
		/// </summary>
		public virtual void UpdateAnimator()
		{

		}

		/// <summary>
		/// Changes the status of the ability's permission
		/// </summary>
		/// <param name="abilityPermitted">If set to <c>true</c> ability permitted.</param>
		public virtual void PermitAbility(bool abilityPermitted)
		{
			AbilityPermitted = abilityPermitted;
		}

		/// <summary>
		/// Override this to specify what should happen in this ability when the character flips
		/// </summary>
		public virtual void Flip()
		{
			
		}

		/// <summary>
		/// Override this to reset this ability's parameters. It'll be automatically called when the character gets killed, in anticipation for its respawn.
		/// </summary>
		public virtual void Reset()
		{
			
		}

		/// <summary>
		/// Plays the ability start sound effect
		/// </summary>
		protected virtual void PlayAbilityStartSfx()
		{
			if (AbilityStartSfx!=null) 
			{	
				SoundManager.Instance.PlaySound(AbilityStartSfx,transform.position);	
			}
		}	

		/// <summary>
		/// Plays the ability used sound effect
		/// </summary>
		protected virtual void PlayAbilityUsedSfx()
		{
			if (AbilityInProgressSfx!=null) 
			{	
				if (_abilityInProgressSfx == null)
				{
					_abilityInProgressSfx = SoundManager.Instance.PlaySound(AbilityInProgressSfx, transform.position, true);	
				}
			}
		}	

		/// <summary>
		/// Stops the ability used sound effect
		/// </summary>
		protected virtual void StopAbilityUsedSfx()
		{
			if (AbilityInProgressSfx!=null) 
			{	
				SoundManager.Instance.StopLoopingSound(_abilityInProgressSfx);	
				_abilityInProgressSfx = null;
			}
		}	

		/// <summary>
		/// Plays the ability stop sound effect
		/// </summary>
		protected virtual void PlayAbilityStopSfx()
		{
			if (AbilityStopSfx!=null) 
			{	
				SoundManager.Instance.PlaySound(AbilityStopSfx,transform.position);	
			}
		}

		/// <summary>
		/// Registers a new animator parameter to the list
		/// </summary>
		/// <param name="parameterName">Parameter name.</param>
		/// <param name="parameterType">Parameter type.</param>
		protected virtual void RegisterAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
		{
			if (_animator == null) 
			{
				return;
			}
			if (_animator.HasParameterOfType(parameterName, parameterType))
			{
				_character._animatorParameters.Add(parameterName);
			}
		}

		/// <summary>
		/// Override this to describe what should happen to this ability when the character respawns
		/// </summary>
		protected virtual void OnRespawn()
		{
		}

		/// <summary>
		/// Override this to describe what should happen to this ability when the character respawns
		/// </summary>
		protected virtual void OnDeath()
		{
			StopAbilityUsedSfx ();
		}

        /// <summary>
        /// Override this to describe what should happen to this ability when the character takes a hit
        /// </summary>
        protected virtual void OnHit()
        {

        }

		/// <summary>
		/// On enable, we bind our respawn delegate
		/// </summary>
		protected virtual void OnEnable()
		{
			if (_health == null)
			{
				_health = GetComponent<Health> ();
			}

			if (_health != null)
			{
				_health.OnRevive += OnRespawn;
				_health.OnDeath += OnDeath;
                _health.OnHit += OnHit;
			}
		}

		/// <summary>
		/// On disable, we unbind our respawn delegate
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_health != null)
			{
				_health.OnRevive -= OnRespawn;
				_health.OnDeath -= OnDeath;
                _health.OnHit -= OnHit;
            }			
		}
	}
}