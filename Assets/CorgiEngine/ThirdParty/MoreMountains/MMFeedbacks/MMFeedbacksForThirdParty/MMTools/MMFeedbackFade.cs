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
    [FeedbackHelp("This feedback lets you trigger a fade event.")]
    [FeedbackPath("Camera/Fade")]
    public class MMFeedbackFade : MMFeedback
    {
        /// the different possible types of fades
        public enum FadeTypes { FadeIn, FadeOut, Custom }

        [Header("Fade")]
        /// the type of fade we want to use when this feedback gets played
        public FadeTypes FadeType;
        /// the ID of the fader(s) to pilot
        public int ID = 0;
        /// the duration (in seconds) of the fade
        public float Duration = 1f;
        /// the curve to use for this fade
        public MMTween.MMTweenCurve Curve = MMTween.MMTweenCurve.EaseOutCubic;
        /// whether or not this fade should ignore timescale
        public bool IgnoreTimeScale = true;

        [Header("Custom")]
        /// the target alpha we're aiming for with this fade
        public float TargetAlpha;
        
        /// <summary>
        /// On play we trigger the selected fade event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                switch (FadeType)
                {
                    case FadeTypes.Custom:
                        MMFadeEvent.Trigger(Duration, TargetAlpha, Curve, ID, IgnoreTimeScale);
                        break;
                    case FadeTypes.FadeIn:
                        MMFadeInEvent.Trigger(Duration, Curve, ID, IgnoreTimeScale);
                        break;
                    case FadeTypes.FadeOut:
                        MMFadeOutEvent.Trigger(Duration, Curve, ID, IgnoreTimeScale);
                        break;
                }
            }
        }
    }
}
