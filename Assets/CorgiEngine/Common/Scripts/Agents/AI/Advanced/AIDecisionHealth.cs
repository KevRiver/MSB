using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This decision will return true if the specified Health conditions are met. You can have it be lower, strictly lower, equal, higher or strictly higher than the specified value.
    /// </summary>
    public class AIDecisionHealth : AIDecision
    {
        /// the different comparison modes
        public enum ComparisonModes { StrictlyLowerThan, LowerThan, Equals, GreatherThan, StrictlyGreaterThan }
        /// the comparison mode with which we'll evaluate the HealthValue
        public ComparisonModes TrueIfHealthIs;
        /// the Health value to compare to
        public int HealthValue;
        /// whether we want this comparison to be done only once or not
        public bool OnlyOnce = true;

        protected Health _health;
        protected bool _once = false;

        /// <summary>
        /// On init we grab our Health component
        /// </summary>
        public override void Initialization()
        {
            _health = _brain.gameObject.GetComponent<Health>();
        }

        /// <summary>
        /// On Decide we evaluate our current Health level
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return EvaluateHealth();
        }

        /// <summary>
        /// Compares our health value and returns true if the condition is met
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvaluateHealth()
        {
            bool returnValue = false;

            if (OnlyOnce && _once)
            {
                return false;
            }

            if (_health == null)
            {
                Debug.LogWarning("You've added an AIDecisionHealth to " + this.gameObject.name + "'s AI Brain, but this object doesn't have a Health component.");
                return false;
            }

            if (!_health.isActiveAndEnabled)
            {
                return false;
            }
            
            if (TrueIfHealthIs == ComparisonModes.StrictlyLowerThan)
            {
                returnValue = (_health.CurrentHealth < HealthValue);
            }

            if (TrueIfHealthIs == ComparisonModes.LowerThan)
            {
                returnValue = (_health.CurrentHealth <= HealthValue);
            }

            if (TrueIfHealthIs == ComparisonModes.Equals)
            {
                returnValue = (_health.CurrentHealth == HealthValue);
            }

            if (TrueIfHealthIs == ComparisonModes.GreatherThan)
            {
                returnValue = (_health.CurrentHealth >= HealthValue);
            }

            if (TrueIfHealthIs == ComparisonModes.StrictlyGreaterThan)
            {
                returnValue = (_health.CurrentHealth > HealthValue);
            }

            if (returnValue)
            {
                _once = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
