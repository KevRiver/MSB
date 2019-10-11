using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will trigger a one time play on a target FloatController
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you trigger a one time play on a target ShaderController.")]
    [FeedbackPath("GameObject/ShaderController")]
    public class MMFeedbackShaderController : MMFeedback
    {
        [Header("Float Controller")]
        /// the float controller to trigger a one time play on
        public ShaderController TargetShaderController;
        /// the duration of the One Time shake
        public float OneTimeDuration = 1f;
        /// the amplitude of the One Time shake (this will be multiplied by the curve's height)
        public float OneTimeAmplitude = 1f;
        /// the curve to apply to the one time shake
        public AnimationCurve OneTimeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        protected float _oneTimeDurationStorage;
        protected float _oneTimeAmplitudeStorage;
        protected AnimationCurve _oneTimeCurveStorage;

        /// <summary>
        /// On init we grab our initial color and components
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            if (Active && (TargetShaderController != null))
            {
                _oneTimeDurationStorage = TargetShaderController.OneTimeDuration;
                _oneTimeAmplitudeStorage = TargetShaderController.OneTimeAmplitude;
                _oneTimeCurveStorage = TargetShaderController.OneTimeCurve;
            }
        }

        /// <summary>
        /// On play we make our renderer flicker
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (TargetShaderController != null))
            {
                TargetShaderController.OneTimeDuration = OneTimeDuration;
                TargetShaderController.OneTimeAmplitude = OneTimeAmplitude;
                TargetShaderController.OneTimeCurve = OneTimeCurve;
                TargetShaderController.OneTime();
            }
        }

        /// <summary>
        /// On reset we make our renderer stop flickering
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();
            if (Active && (TargetShaderController != null))
            {
                TargetShaderController.OneTimeDuration = _oneTimeDurationStorage;
                TargetShaderController.OneTimeAmplitude = _oneTimeAmplitudeStorage;
                TargetShaderController.OneTimeCurve = _oneTimeCurveStorage;
            }
        }

    }
}
