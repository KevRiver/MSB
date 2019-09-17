using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this class to a trigger and it will allow you to modify the time scale when entering it, for the specified duration and settings
    /// </summary>
    [AddComponentMenu("Corgi Engine/Environment/Time Zone")]
    public class TimeZone : ButtonActivated
    {
        /// the possible modes for this zone
        public enum Modes { DurationBased, ExitBased }

        [Header("Time Zone")]
        /// whether this zone will modify time on entry for a certain duration, or until it is exited
        public Modes Mode = Modes.DurationBased;

        /// the new timescale to apply
        public float TimeScale = 0.5f;
        /// the duration to apply the new timescale for
        public float Duration = 1f;
        /// whether or not the timescale should be lerped
        public bool LerpTimeScale = true;
        /// the speed at which to lerp the timescale
        public float LerpSpeed = 5f;

        /// <summary>
        /// When the button is pressed we start modifying the timescale
        /// </summary>
        public override void TriggerButtonAction()
        {
            if (!CheckNumberOfUses())
            {
                return;
            }
            base.TriggerButtonAction();
            ControlTime();
            ActivateZone();
        }

        /// <summary>
        /// When exiting, and if needed, we reset the time scale
        /// </summary>
        /// <param name="collider"></param>
        protected override void TriggerExit(GameObject collider)
        {
            base.TriggerExit(collider);
            if (Mode == Modes.ExitBased)
            {
                Character character = collider.gameObject.GetComponentNoAlloc<Character>();
                CharacterButtonActivation characterButtonActivation = collider.gameObject.GetComponentNoAlloc<CharacterButtonActivation>();

                if (!CheckConditions(character, characterButtonActivation))
                {
                    return;
                }

                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
            }
        }

        /// <summary>
        /// Modifies the timescale
        /// </summary>
        public virtual void ControlTime()
        {
            if (Mode == Modes.ExitBased)
            {
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScale, Duration, LerpTimeScale, LerpSpeed, true);
            }
            else
            {
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScale, Duration, LerpTimeScale, LerpSpeed, false);
            }
        }
    }
}