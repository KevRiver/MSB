using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This Action will make the Character patrols until it (optionally) hits a wall or a hole.
    /// </summary>
    public class AIActionPatrol : AIAction
    {
        [Header("Obstacle Detection")]
        /// If set to true, the agent will change direction when hitting a wall
        public bool ChangeDirectionOnWall = true;
        /// If set to true, the agent will try and avoid falling
        public bool AvoidFalling = false;
        /// The offset the hole detection should take into account
        public Vector3 HoleDetectionOffset = new Vector3(0, 0, 0);
        /// the length of the ray cast to detect holes
        public float HoleDetectionRaycastLength = 1f;

        // private stuff
        protected CorgiController _controller;
        protected Character _character;
        protected Health _health;
        protected CharacterHorizontalMovement _characterHorizontalMovement;
        protected Vector2 _direction;
        protected Vector2 _startPosition;
        protected Vector2 _initialDirection;
        protected Vector3 _initialScale;
        protected float _distanceToTarget;

        /// <summary>
        /// On init we grab all the components we'll need
        /// </summary>
        protected override void Initialization()
        {
            // we get the CorgiController2D component
            _controller = GetComponent<CorgiController>();
            _character = GetComponent<Character>();
            _characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
            _health = GetComponent<Health>();
            // initialize the start position
            _startPosition = transform.position;
            // initialize the direction
            _direction = _character.IsFacingRight ? Vector2.right : Vector2.left;

            _initialDirection = _direction;
            _initialScale = transform.localScale;
        }

        /// <summary>
        /// On PerformAction we patrol
        /// </summary>
        public override void PerformAction()
        {
            Patrol();
        }

        /// <summary>
        /// This method initiates all the required checks and moves the character
        /// </summary>
        protected virtual void Patrol()
        {
            if (_character == null)
            {
                return;
            }
            if ((_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
                || (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
            {
                return;
            }
            // moves the agent in its current direction
            CheckForWalls();
            CheckForHoles();
            _characterHorizontalMovement.SetHorizontalMove(_direction.x);
        }

        /// <summary>
        /// When exiting the state we reset our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            _characterHorizontalMovement.SetHorizontalMove(0f);
        }

        /// <summary>
	    /// Checks for a wall and changes direction if it meets one
	    /// </summary>
	    protected virtual void CheckForWalls()
        {
            if (!ChangeDirectionOnWall)
            {
                return;
            }

            // if the agent is colliding with something, make it turn around
            if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            {
                ChangeDirection();
            }
        }

        /// <summary>
        /// Checks for holes 
        /// </summary>
        protected virtual void CheckForHoles()
        {
            // if we're not grounded or if we're not supposed to check for holes, we do nothing and exit
            if (!AvoidFalling || !_controller.State.IsGrounded)
            {
                return;
            }

            // we send a raycast at the extremity of the character in the direction it's facing, and modified by the offset you can set in the inspector.
            Vector2 raycastOrigin = new Vector2(transform.position.x + _direction.x * (HoleDetectionOffset.x + Mathf.Abs(GetComponent<BoxCollider2D>().bounds.size.x) / 2), transform.position.y + HoleDetectionOffset.y - (transform.localScale.y / 2));
            RaycastHit2D raycast = MMDebug.RayCast(raycastOrigin, -transform.up, HoleDetectionRaycastLength, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask, Color.gray, true);
            // if the raycast doesn't hit anything
            if (!raycast)
            {
                // we change direction
                ChangeDirection();
            }
        }

        /// <summary>
        /// Changes the current movement direction
        /// </summary>
        protected virtual void ChangeDirection()
        {
            _direction = -_direction;
        }

        /// <summary>
        /// When reviving we make sure our directions are properly setup
        /// </summary>
        protected virtual void OnRevive()
        {
            _direction = _character.IsFacingRight ? Vector2.right : Vector2.left;
            transform.localScale = _initialScale;
            transform.position = _startPosition;
        }

        /// <summary>
        /// On enable we start listening for OnRevive events
        /// </summary>
        protected virtual void OnEnable()
        {
            if (_health == null)
            {
                _health = this.gameObject.GetComponent<Health>();
            }

            if (_health != null)
            {
                _health.OnRevive += OnRevive;
            }
        }

        /// <summary>
        /// On disable we stop listening for OnRevive events
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_health != null)
            {
                _health.OnRevive -= OnRevive;
            }
        }
    }
}