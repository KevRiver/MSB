using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This decision returns true if the Character got hit this frame, or after the specified number of hits has been reached.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class AIDecisionHit : AIDecision
    {
        /// The number of hits required to return true
        public int NumberOfHits = 1;

        protected int _hitCounter;
        protected Health _health;

        /// <summary>
        /// On init we grab our Health component
        /// </summary>
        public override void Initialization()
        {
            _health = _brain.gameObject.GetComponent<Health>();
            _hitCounter = 0;
        }

        /// <summary>
        /// On Decide we check whether we've been hit
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return EvaluateHits();
        }

        /// <summary>
        /// Checks whether we've been hit enough times
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvaluateHits()
        {
            return (_hitCounter >= NumberOfHits);
        }

        /// <summary>
        /// On EnterState, resets the hit counter
        /// </summary>
        public override void OnEnterState()
        {
            _hitCounter = 0;
        }

        /// <summary>
        /// On exit state, resets the hit counter
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            _hitCounter = 0;
        }

        /// <summary>
        /// When we get hit we increase our hit counter
        /// </summary>
        protected virtual void OnHit()
        {
            _hitCounter++;
        }

        /// <summary>
        /// Grabs our health component and starts listening for OnHit events
        /// </summary>
        protected virtual void OnEnable()
        {
            if (_health == null)
            {
                _health = this.gameObject.GetComponent<Health>();
            }

            if (_health != null)
            {
                _health.OnHit += OnHit;
            }
        }

        /// <summary>
        /// Stops listening for OnHit events
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_health != null)
            {
                _health.OnHit -= OnHit;
            }
        }
    }
}
