using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This decision will return true after the specified duration (in seconds) has passed since the level was loaded.
    /// </summary>
    public class AIDecisionTimeSinceStart : AIDecision
    {
        /// The duration (in seconds) after which to return true
        public float AfterTime;

        protected float _startTime;

        /// <summary>
        /// On init we store our current time
        /// </summary>
        public override void Initialization()
        {
            _startTime = Time.time;
        }

        /// <summary>
        /// On Decide we evaluate our time since the level has started
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return EvaluateTime();
        }

        /// <summary>
        /// Returns true if the time since the level has started has exceeded our requirements
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvaluateTime()
        {
            return (Time.time - _startTime >= AfterTime);
        }
    }
}
