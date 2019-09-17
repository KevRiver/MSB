using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This decision will return true if the character is grounded, false otherwise.
    /// </summary>
    public class AIDecisionGrounded : AIDecision
    {
        /// The duration, in seconds, after entering the state this Decision is in during which we'll ignore being grounded
        public float GroundedBufferDelay = 0.2f;

        protected CorgiController _controller;
        protected float _startTime = 0f;

        /// <summary>
        /// On init we grab our CorgiController component
        /// </summary>
        public override void Initialization()
        {
            _controller = this.gameObject.GetComponent<CorgiController>();
        }

        /// <summary>
        /// On Decide we check if we're grounded
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return EvaluateGrounded();
        }

        /// <summary>
        /// Checks whether the character is grounded
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvaluateGrounded()
        {
            if (Time.time - _startTime < GroundedBufferDelay)
            {
                return false;
            }
            return (_controller.State.IsGrounded);
        }

        /// <summary>
        /// On Enter State we reset our start time
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            _startTime = Time.time;
        }
    }
}
