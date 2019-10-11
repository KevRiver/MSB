using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This decision will return true after the specified duration, picked randomly between the min and max values (in seconds) has passed since the Brain has been in the state this decision is in. Use it to do something X seconds after having done something else.
    /// </summary>
    public class AIDecisionTimeInState : AIDecision
    {

        /// The minimum duration, in seconds, after which to return true
        public float AfterTimeMin = 2f;
        /// The maximum duration, in seconds, after which to return true
        public float AfterTimeMax = 2f;

        protected float _randomTime;

        /// <summary>
        /// On Decide we evaluate our time
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return EvaluateTime();
        }

        /// <summary>
        /// Returns true if enough time has passed since we entered the current state
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvaluateTime()
        {
            if (_brain == null) { return false; }
            return (_brain.TimeInThisState >= _randomTime);
        }

        /// <summary>
        /// On init we randomize our next delay
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            RandomizeTime();
        }

        /// <summary>
        /// On enter state we randomize our next delay
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            RandomizeTime();
        }

        /// <summary>
        /// On randomize time we randomize our next delay
        /// </summary>
        protected virtual void RandomizeTime()
        {
            _randomTime = Random.Range(AfterTimeMin, AfterTimeMax);
        }
    }
}
