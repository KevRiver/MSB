using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This Action will make the Character fly until it hits a wall or a hole while following a path.
    /// </summary>
    [RequireComponent(typeof(MMPath))]
    public class AIActionFlyPatrol : AIAction
    {        
        [Header("Obstacle Detection")]
        /// If set to true, the agent will change direction when hitting an obstacle
        public bool ChangeDirectionOnObstacle = true;

        // private stuff
        protected CorgiController _controller;
        protected Character _character;
        protected CharacterFly _characterFly;
        protected Health _health;
        protected Vector2 _direction;
        protected Vector2 _startPosition;
        protected Vector2 _initialDirection;
        protected Vector3 _initialScale;
        protected float _distanceToTarget;
        protected Vector3 _initialPosition;
        protected MMPath _mmPath;

        /// <summary>
        /// On init we grab all the components we'll need
        /// </summary>
        protected override void Initialization()
        {
            // we get the CorgiController2D component
            _controller = this.gameObject.GetComponent<CorgiController>();
            _character = this.gameObject.GetComponent<Character>();
            _characterFly = this.gameObject.GetComponent<CharacterFly>();
            _health = this.gameObject.GetComponent<Health>();
            _mmPath = this.gameObject.GetComponent<MMPath>();
            // initialize the start position
            _startPosition = transform.position;
            // initialize the direction
            _direction = _character.IsFacingRight ? Vector2.right : Vector2.left;

            _initialPosition = this.transform.position;
            _initialDirection = _direction;
            _initialScale = transform.localScale;

            
        }


        /// <summary>
        /// On PerformAction we patrol
        /// </summary>
        public override void PerformAction()
        {
            FlyPatrol();
        }

        /// <summary>
        /// This method initiates all the required checks and moves the character
        /// </summary>
        protected virtual void FlyPatrol()
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
            CheckForObstacles();

            _direction = _mmPath.CurrentPoint() - this.transform.position;
            _direction = _direction.normalized;

            _characterFly.SetHorizontalMove(_direction.x);
            _characterFly.SetVerticalMove(_direction.y);
        }

        /// <summary>
        /// Draws bounds gizmos
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (_mmPath == null)
            {
                return;
            }
            Gizmos.color = MoreMountains.Tools.Colors.IndianRed;
            Gizmos.DrawLine(this.transform.position, _mmPath.CurrentPoint());
        }

        /// <summary>
        /// When exiting the state we reset our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            _characterFly.SetHorizontalMove(0f);
            _characterFly.SetVerticalMove(0f);
        }

        /// <summary>
	    /// Checks for a wall and changes direction if it meets one
	    /// </summary>
	    protected virtual void CheckForObstacles()
        {
            if (!ChangeDirectionOnObstacle)
            {
                return;
            }
            
            // if the agent is colliding with something, make it turn around
            if (
                (_direction.x < 0 && _controller.State.IsCollidingLeft) 
                || (_direction.x > 0 && _controller.State.IsCollidingRight)
                || (_direction.y < 0 && _controller.State.IsCollidingBelow)
                || (_direction.y > 0 && _controller.State.IsCollidingAbove)
                )
            {
                ChangeDirection();
            }
        }
        
        /// <summary>
        /// Changes the current movement direction
        /// </summary>
        protected virtual void ChangeDirection()
        {
            _direction = -_direction;
            _mmPath.ChangeDirection();
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