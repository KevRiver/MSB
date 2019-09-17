using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a character and it'll be able to hang from ledges and climb up
    /// Animator parameters : LedgeHanging, LedgeClimbing
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Ledge Hang")]
    public class CharacterLedgeHang : CharacterAbility, MMEventListener<LedgeEvent>
    {                
		public override string HelpBoxText() { return "This component allows you to hang from objects with a Ledge component on them. From the inspector here you'll be able to specify the name of the Idle animation to return to after the climb, the duration of the climb animation (in seconds), and a minimum buffer delay for the hang time (0.2s is a safe value)."; }
        [Header("Animation")]
        /// the name of the animation to play after the climb animation is complete (usually your Idle animation)
        public string IdleAnimationName = "Idle";
        /// the duration of your climbing animation, after this it'll transition to IdleAnimationName automatically
        public float ClimbingAnimationDuration = 0.5f;
        [Header("Settings")]
        /// the minimum time the Character must have been LedgeHanging before it can LedgeClimb. 0.2s (or more) will prevent any glitches and unwanted input conflicts
        public float MinimumHangingTime = 0.2f;

        protected Ledge _ledge = null;
        protected CharacterJump _characterJump;
        protected WaitForSeconds _climbingAnimationDelay;
        protected float _ledgeHangingStartedTimestamp;

        /// <summary>
        /// On Start() we grab a few components for storage
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _characterJump = this.gameObject.GetComponent<CharacterJump>();
            _climbingAnimationDelay = new WaitForSeconds(ClimbingAnimationDuration);
        }

        /// <summary>
        /// Every frame, we check the input for a up input, in case we're hanging
        /// </summary>
        protected override void HandleInput()
        {
            if (_movement.CurrentState != CharacterStates.MovementStates.LedgeHanging)
            {
                return;
            }

            if (Time.time - _ledgeHangingStartedTimestamp < MinimumHangingTime)
            {
                return;
            }

            if (_verticalInput > _inputManager.Threshold.y)
            {
                StartCoroutine(Climb());
            }
        }

        /// <summary>
        /// Every frame we make sure we don't have to detach from the ledge
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            HandleLedge();

            if ((_movement.CurrentState != CharacterStates.MovementStates.LedgeHanging)
                && (_movement.CurrentState != CharacterStates.MovementStates.LedgeClimbing)
                && (_movement.PreviousState == CharacterStates.MovementStates.LedgeHanging))
            {
                DetachFromLedge();
            }
        }

        /// <summary>
        /// When getting a ledge event, we make sure it's this Character, and if it is, we grab the ledge
        /// </summary>
        /// <param name="ledgeEvent"></param>
        public virtual void OnMMEvent(LedgeEvent ledgeEvent)
        {
            if (ledgeEvent.CharacterCollider.gameObject != this.gameObject)
            {
                return;
            }
            StartGrabbingLedge(ledgeEvent.LedgeGrabbed);
        }

        /// <summary>
        /// Grabs the ledge if possible
        /// </summary>
        /// <param name="ledge"></param>
        public virtual void StartGrabbingLedge(Ledge ledge)
        {
            // we make sure we're facing the right direction
            if ( (_character.IsFacingRight && (ledge.LedgeGrabDirection == Ledge.LedgeGrabDirections.Left))
                || (!_character.IsFacingRight && (ledge.LedgeGrabDirection == Ledge.LedgeGrabDirections.Right)))
            {
                return;
            }

            // we make sure we can grab the ledge
            if (!AbilityPermitted
                || (_movement.CurrentState == CharacterStates.MovementStates.Jetpacking))
            {
                return;
            }

            // we start hanging from the ledge
            _ledgeHangingStartedTimestamp = Time.time;
            _ledge = ledge;
            _controller.CollisionsOff();
            _movement.ChangeState(CharacterStates.MovementStates.LedgeHanging);

        }

        /// <summary>
        /// Every frame, if we're hanging from a ledge, we prevent any force from moving our character, prevent flip and force our position to the ledge's offset
        /// </summary>
        protected virtual void HandleLedge()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
            {
                _controller.SetForce(Vector2.zero);
                _controller.GravityActive(false);
                if (_characterJump != null)
                {
                    _characterJump.ResetNumberOfJumps();
                }
                _characterHorizontalMovement.AbilityPermitted = false;
                _character.CanFlip = false;
                _controller.transform.position = _ledge.transform.position + _ledge.HangOffset;
            }
        }

        /// <summary>
        /// This coroutine handles the climb sequence
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Climb()
        {
            // we start to climb
            _movement.ChangeState(CharacterStates.MovementStates.LedgeClimbing);
            MMAnimator.UpdateAnimatorBool(_animator, "LedgeClimbing", true, _character._animatorParameters);
            // we prevent all other input
            _inputManager.InputDetectionActive = false;

            // we wait until the climb animation is complete
            yield return _climbingAnimationDelay;

            // we restore input and go to idle
            _inputManager.InputDetectionActive = true;
            MMAnimator.UpdateAnimatorBool(_animator, "LedgeClimbing", false, _character._animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "Idle", true, _character._animatorParameters);
            _animator.Play(IdleAnimationName);
            
            // we teleport our character to its new position (this offset is specified on the Ledge object)
            this.transform.position = _ledge.transform.position + _ledge.ClimbOffset;
            
            // we go back to idle and detach from the ledge
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _controller.GravityActive(true);
            DetachFromLedge();
        }

        /// <summary>
        /// Detaches the Character from the ledge, losing any reference to it, and restoring permissions
        /// </summary>
        protected virtual void DetachFromLedge()
        {
            _ledge = null;
            _character.CanFlip = true;
            _characterHorizontalMovement.AbilityPermitted = true;
            _controller.CollisionsOn();
        }

        /// <summary>
        /// Initializes the LedgeHanging and LedgeClimbing animator parameters
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter("LedgeHanging", AnimatorControllerParameterType.Bool);
            RegisterAnimatorParameter("LedgeClimbing", AnimatorControllerParameterType.Bool);
        }

        /// <summary>
        /// At the end of each cycle, we send our current LookingUp status to the animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimator.UpdateAnimatorBool(_animator, "LedgeHanging", (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging), _character._animatorParameters);
        }

        /// <summary>
        /// On enable, we start listening for LedgeEvents
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<LedgeEvent>();
        }

        /// <summary>
        /// On disable, we stop listening for ledge events
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            this.MMEventStopListening<LedgeEvent>();
        }
    }
}
