using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Requires a CharacterFly ability. Makes the character fly up to the specified MinimumDistance in the direction of the target. That's how the RetroGhosts move.
    /// </summary>
    [RequireComponent(typeof(CharacterFly))]
    public class AIActionFlyTowardsTarget : AIAction
    {
        /// the minimum distance from the target this Character can reach.
        public float MinimumDistance = 1f;

        protected CharacterFly _characterFly;
        protected int _numberOfJumps = 0;

        /// <summary>
        /// On init we grab our CharacterFly ability
        /// </summary>
        protected override void Initialization()
        {
            _characterFly = this.gameObject.GetComponent<CharacterFly>();
        }

        /// <summary>
        /// On PerformAction we fly
        /// </summary>
        public override void PerformAction()
        {
            Fly();
        }

        /// <summary>
        /// Moves the character towards the target if needed
        /// </summary>
        protected virtual void Fly()
        {
            if (_brain.Target == null)
            {
                return;
            }
            
            if (this.transform.position.x < _brain.Target.position.x)
            {
                _characterFly.SetHorizontalMove(1f);
            }
            else
            {
                _characterFly.SetHorizontalMove(-1f);
            }

            if (this.transform.position.y < _brain.Target.position.y)
            {
                _characterFly.SetVerticalMove(1f);
            }
            else
            {
                _characterFly.SetVerticalMove(-1f);
            }
            
            if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) < MinimumDistance)
            {
                _characterFly.SetHorizontalMove(0f);
            }

            if (Mathf.Abs(this.transform.position.y - _brain.Target.position.y) < MinimumDistance)
            {
                _characterFly.SetVerticalMove(0f);
            }
        }

        /// <summary>
        /// On exit state we stop our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterFly.SetHorizontalMove(0f);
            _characterFly.SetVerticalMove(0f);
        }
    }
}
