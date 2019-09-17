using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Probably more suited for an AI character than a Player character, this ability allows the Character to follow a path, as defined in the linked MMPathMovement component
    /// </summary>
    [RequireComponent(typeof(MMPathMovement))]
    public class CharacterFollowPath : CharacterAbility
    {
        /// the speed at which the character follows the path
        public float FollowPathSpeed = 6f;
        /// a multiplier you can target to increase/reduce the follow speed
        public float MovementSpeedMultiplier { get; set; }
        /// whether or not the Character is always following the path, in which case it'll start immune to gravity 
        public bool AlwaysFollowingPath = false;

        protected MMPathMovement _mmPathMovement;
        protected bool _followingPath;

        /// <summary>
        /// On Start, we initialize our path follow if needed
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            _mmPathMovement = this.gameObject.GetComponent<MMPathMovement>();
            MovementSpeedMultiplier = 1f;

            if (AlwaysFollowingPath)
            {
                StartFollowingPath();
            }
        }
        
        /// <summary>
        /// Starts the follow sequence
        /// </summary>
        public virtual void StartFollowingPath()
        {
            if ((!AbilityPermitted) // if the ability is not permitted
                || (_movement.CurrentState == CharacterStates.MovementStates.Dashing) // or if we're dashing
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if we're in the gripping state
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
            {
                return;
            }

            // if this is the first time we're here, we trigger our sounds
            if (_movement.CurrentState != CharacterStates.MovementStates.FollowingPath)
            {
                // we play the jetpack start sound 
                PlayAbilityStartSfx();
                PlayAbilityUsedSfx();
                _followingPath = true;
            }

            // we set the various states
            _movement.ChangeState(CharacterStates.MovementStates.FollowingPath);

            _mmPathMovement.enabled = true;
            MovementSpeedMultiplier = 1f;
            _controller.GravityActive(false);
        }

        /// <summary>
        /// Stops the path follow
        /// </summary>
        public virtual void StopFollowingPath()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.FollowingPath)
            {
                StopAbilityUsedSfx();
                PlayAbilityStopSfx();
            }
            _mmPathMovement.enabled = false;
            _controller.GravityActive(true);
            _followingPath = false;
            _movement.RestorePreviousState();
        }

        /// <summary>
        /// On Update, checks if we should stop following the path
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (AlwaysFollowingPath)
            {
                if (_movement.CurrentState != CharacterStates.MovementStates.FollowingPath)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.FollowingPath);
                }
                _followingPath = true;
            }

            _mmPathMovement.MovementSpeed = FollowPathSpeed * MovementSpeedMultiplier;

            HandleMovement();

            // if we're not following the path anymore, we stop our following sound
            if (_movement.CurrentState != CharacterStates.MovementStates.FollowingPath && _abilityInProgressSfx != null)
            {
                StopAbilityUsedSfx();
            }

            if (_movement.CurrentState != CharacterStates.MovementStates.FollowingPath && _followingPath)
            {
                StopFollowingPath();
            }

            if (_controller.State.IsCollidingAbove && (_movement.CurrentState != CharacterStates.MovementStates.FollowingPath))
            {
                _controller.SetVerticalForce(0);
            }
        }


        /// <summary>
        /// Makes the character flip direction according to its movement along the path
        /// </summary>
        protected virtual void HandleMovement()
        {
            // if we're not following anymore, we stop our following sound
            if (_movement.CurrentState != CharacterStates.MovementStates.FollowingPath && _abilityInProgressSfx != null)
            {
                StopAbilityUsedSfx();
            }

            if (_movement.CurrentState == CharacterStates.MovementStates.FollowingPath && _abilityInProgressSfx == null)
            {
                PlayAbilityUsedSfx();
            }

            // if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
            if (!AbilityPermitted
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                return;
            }

            // If the value of the horizontal axis is positive, the character must face right.
            if (_mmPathMovement.CurrentSpeed.x > 0.1f)
            {
                if (!_character.IsFacingRight)
                    _character.Flip();
            }
            // If it's negative, then we're facing left
            else if (_mmPathMovement.CurrentSpeed.x < -0.1f)
            {
                if (_character.IsFacingRight)
                    _character.Flip();
            }
        }

        /// <summary>
        /// When the character respawns we reinitialize it
        /// </summary>
        protected virtual void OnRevive()
        {
            Initialization();
        }

        /// <summary>
        /// When the player respawns, we reinstate this agent.
        /// </summary>
        /// <param name="checkpoint">Checkpoint.</param>
        /// <param name="player">Player.</param>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (gameObject.GetComponentNoAlloc<Health>() != null)
            {
                gameObject.GetComponentNoAlloc<Health>().OnRevive += OnRevive;
            }
        }

        /// <summary>
        /// Stops listening for revive events
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_health != null)
            {
                _health.OnRevive -= OnRevive;
            }
        }

        /// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter("FollowingPath", AnimatorControllerParameterType.Bool);
            RegisterAnimatorParameter("FollowingPathSpeed", AnimatorControllerParameterType.Float);
        }

        /// <summary>
        /// At the end of each cycle, we send our character's animator the current following status
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimator.UpdateAnimatorBool(_animator, "FollowingPath", (_movement.CurrentState == CharacterStates.MovementStates.FollowingPath), _character._animatorParameters);
            MMAnimator.UpdateAnimatorFloat(_animator, "FollowingPathSpeed", Mathf.Abs(_mmPathMovement.CurrentSpeed.magnitude), _character._animatorParameters);
        }

    }
}
