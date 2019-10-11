using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// Turns an object active or inactive at the various stages of the feedback
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to change the state of a behaviour on a target gameobject from active to inactive (or the opposite), on init, play, stop or reset. " +
        "For each of these you can specify if you want to force a state (enabled or disabled), or toggle it (enabled becomes disabled, disabled becomes enabled).")]
    [FeedbackPath("GameObject/Enable Behaviour")]
    public class MMFeedbackEnable : MMFeedback
    {
        /// the possible effects the feedback can have on the target object's status 
        public enum PossibleStates { Enabled, Disabled, Toggle }

        [Header("Set Active")]
        /// the gameobject we want to change the active state of
        public Behaviour TargetBehaviour;

        /// whether or not we should alter the state of the target object on init
        public bool SetStateOnInit = false;
        [MMFCondition("SetStateOnInit", true)]
        /// how to change the state on init
        public PossibleStates StateOnInit = PossibleStates.Disabled;
        /// whether or not we should alter the state of the target object on play
        public bool SetStateOnPlay = false;
        [MMFCondition("SetStateOnPlay", true)]
        /// how to change the state on play
        public PossibleStates StateOnPlay = PossibleStates.Disabled;
        /// whether or not we should alter the state of the target object on stop
        public bool SetStateOnStop = false;
        [MMFCondition("SetStateOnStop", true)]
        /// how to change the state on stop
        public PossibleStates StateOnStop = PossibleStates.Disabled;
        /// whether or not we should alter the state of the target object on reset
        public bool SetStateOnReset = false;
        [MMFCondition("SetStateOnReset", true)]
        /// how to change the state on reset
        public PossibleStates StateOnReset = PossibleStates.Disabled;

        /// <summary>
        /// On init we change the state of our Behaviour if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (Active && (TargetBehaviour != null))
            {
                if (SetStateOnInit)
                {
                    SetStatus(StateOnInit);
                }
            }
        }

        /// <summary>
        /// On Play we change the state of our Behaviour if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (TargetBehaviour != null))
            {
                if (SetStateOnPlay)
                {
                    SetStatus(StateOnPlay);
                }
            }
        }

        /// <summary>
        /// On Stop we change the state of our Behaviour if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1)
        {
            base.CustomStopFeedback(position, attenuation);

            if (Active && (TargetBehaviour != null))
            {
                if (SetStateOnStop)
                {
                    SetStatus(StateOnStop);
                }
            }
        }

        /// <summary>
        /// On Reset we change the state of our Behaviour if needed
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();

            if (Active && (TargetBehaviour != null))
            {
                if (SetStateOnReset)
                {
                    SetStatus(StateOnReset);
                }
            }
        }

        /// <summary>
        /// Changes the status of the Behaviour
        /// </summary>
        /// <param name="state"></param>
        protected virtual void SetStatus(PossibleStates state)
        {
            switch (state)
            {
                case PossibleStates.Enabled:
                    TargetBehaviour.enabled = true;
                    break;
                case PossibleStates.Disabled:
                    TargetBehaviour.enabled = false;
                    break;
                case PossibleStates.Toggle:
                    TargetBehaviour.enabled = !TargetBehaviour.enabled;
                    break;
            }
        }
    }
}
