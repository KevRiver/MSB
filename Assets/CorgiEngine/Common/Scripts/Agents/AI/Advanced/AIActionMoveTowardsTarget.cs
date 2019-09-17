using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This action directs the CharacterHorizontalMovement ability to move in the direction of the target.
    /// </summary>
    public class AIActionMoveTowardsTarget : AIAction
    {
        /// The minimum distance to the target that this Character can reach
        public float MinimumDistance = 1f;

        protected CharacterHorizontalMovement _characterHorizontalMovement;
        
        /// <summary>
        /// On init we grab our CharacterHorizontalMovement ability
        /// </summary>
        protected override void Initialization()
        {
            _characterHorizontalMovement = this.gameObject.GetComponent<CharacterHorizontalMovement>();
        }

        /// <summary>
        /// On PerformAction we move
        /// </summary>
        public override void PerformAction()
        {
            Move();
        }

        /// <summary>
        /// Moves the character in the decided direction
        /// </summary>
        protected virtual void Move()
        {
            if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) < MinimumDistance)
            {
                _characterHorizontalMovement.SetHorizontalMove(0f);
                return;
            }

            if (this.transform.position.x < _brain.Target.position.x)
            {
                _characterHorizontalMovement.SetHorizontalMove(1f);
            }            
            else
            {
                _characterHorizontalMovement.SetHorizontalMove(-1f);
            }
        }

        /// <summary>
        /// When entering the state we reset our movement.
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            _characterHorizontalMovement.SetHorizontalMove(0f);
        }

        /// <summary>
        /// When exiting the state we reset our movement.
        /// </summary>
        public override void OnExitState()
        {
            base.OnEnterState();
            _characterHorizontalMovement.SetHorizontalMove(0f);
        }
    }
}
