using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This Decision will return true if the current Brain's Target is within the specified range, false otherwise.
    /// </summary>
    public class AIDecisionDistanceToTarget : AIDecision
    {
        /// The possible comparison modes
        public enum ComparisonModes { StrictlyLowerThan, LowerThan, Equals, GreatherThan, StrictlyGreaterThan }
        /// the comparison mode
        public ComparisonModes ComparisonMode = ComparisonModes.GreatherThan;
        /// the distance to compare with
        public float Distance;
        
        /// <summary>
        /// On Decide we check our distance to the Target
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return EvaluateDistance();
        }

        /// <summary>
        /// Returns true if the distance conditions are met
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvaluateDistance()
        {
            if (_brain.Target == null)
            {
                return false;
            }

            float distance = Vector3.Distance(this.transform.position, _brain.Target.position);

            if (ComparisonMode == ComparisonModes.StrictlyLowerThan)
            {
                return (distance < Distance);
            }
            if (ComparisonMode == ComparisonModes.LowerThan)
            {
                return (distance <= Distance);
            }
            if (ComparisonMode == ComparisonModes.Equals)
            {
                return (distance == Distance);
            }
            if (ComparisonMode == ComparisonModes.GreatherThan)
            {
                return (distance >= Distance);
            }
            if (ComparisonMode == ComparisonModes.StrictlyGreaterThan)
            {
                return (distance > Distance);
            }
            return false;
        }
    }
}
