using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Inherits from AIActionPatrol but also lets you define left and right bounds that the character can't exceed.
    /// </summary>
    public class AIActionPatrolWithinBounds : AIActionPatrol
    {
        /// <summary>
        /// BoundsMethods determine whether the bounds will be defined based on the starting position of the Character, or the position it was at when entering the state
        /// </summary>
        public enum BoundsMethods { BasedOnOriginPosition, BasedOnStateEnterPosition }

        [Header("Bounds")]
        /// the chosen BoundsMethod
        public BoundsMethods BoundsMethod = BoundsMethods.BasedOnOriginPosition;
        /// the max distance the character can patrol to the left
        public float BoundsExtentsLeft;
        /// the max distance the character can patrol to the right
        public float BoundsExtentsRight;

        protected Vector3 _initialPosition;
        protected bool _init = false;
        protected Vector3 _boundsLeft;
        protected Vector3 _boundsRight;

        /// <summary>
        /// On init we store our initial position and define our bounds
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _init = true;
            _initialPosition = this.transform.position;
            EstablishBounds();
        }

        /// <summary>
        /// When patrolling we make sure we don't exceed our bounds
        /// </summary>
        protected override void Patrol()
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
            CheckForDistance();
            _characterHorizontalMovement.SetHorizontalMove(_direction.x);
        }

        /// <summary>
        /// Determines whether or not bounds have been exceeded and turns around if needed
        /// </summary>
        protected virtual void CheckForDistance()
        {
            if (this.transform.position.x < _boundsLeft.x)
            {
                _direction = Vector2.right;
            }
            if (this.transform.position.x > _boundsRight.x)
            {
                _direction = Vector2.left;
            }
        }

        /// <summary>
        /// Determines the position of the bounds
        /// </summary>
        protected virtual void EstablishBounds()
        {
            _boundsLeft = this.transform.position + Vector3.left * BoundsExtentsLeft;
            _boundsRight = this.transform.position + Vector3.right * BoundsExtentsRight;
        }

        /// <summary>
        /// On Enter state we determine our bounds if required
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            if (BoundsMethod == BoundsMethods.BasedOnStateEnterPosition)
            {
                _initialPosition = this.transform.position;
                EstablishBounds();
            }            
        }

        /// <summary>
        /// On exit state we stop moving
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            _characterHorizontalMovement.SetHorizontalMove(0f);
        }

        /// <summary>
        /// Draws bounds gizmos
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (!_init)
            {
                EstablishBounds();
            }
            Gizmos.color = MoreMountains.Tools.Colors.IndianRed;
            Gizmos.DrawLine(_boundsLeft + Vector3.down * 1f, _boundsLeft + Vector3.up * 1f);
            Gizmos.color = MoreMountains.Tools.Colors.Salmon;
            Gizmos.DrawLine(_boundsRight + Vector3.down * 1f, _boundsRight + Vector3.up * 1f);
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(_boundsLeft + Vector3.up * 1f, _boundsRight + Vector3.up * 1f);
        }
    }
}
