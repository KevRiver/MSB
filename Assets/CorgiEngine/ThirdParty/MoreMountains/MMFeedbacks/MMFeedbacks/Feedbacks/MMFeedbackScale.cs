using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will animate the scale of the target object over time when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("GameObject/Scale")]
    [FeedbackHelp("This feedback will animate the target's scale on the 3 specified animation curves, for the specified duration (in seconds). You can apply a multiplier, that will multiply each animation curve value.")]
    public class MMFeedbackScale : MMFeedback
    {
        public enum TimeScales { Scaled, Unscaled }

        [Header("Scale")]
        /// whether this feedback should play in scaled or unscaled time
        public TimeScales TimeScale = TimeScales.Scaled;
        /// the object to animate
        public Transform AnimateScaleTarget;
        /// the duration of the animation
        public float AnimateScaleDuration = 0.2f;
        /// how much each curve should be multiplied
        public float Multiplier = 1f;
        /// how much should be added to the curve
        public float Offset = 0f;
        /// if this is true, should animate the X scale value
        public bool AnimateX = true;
        /// the x scale animation definition
        [MMFCondition("AnimateX", true)]
        public AnimationCurve AnimateScaleX = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        /// if this is true, should animate the Y scale value
        public bool AnimateY = true;
        /// the y scale animation definition
        [MMFCondition("AnimateY", true)]
        public AnimationCurve AnimateScaleY = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        /// if this is true, should animate the z scale value
        public bool AnimateZ = true;
        /// the z scale animation definition
        [MMFCondition("AnimateZ", true)]
        public AnimationCurve AnimateScaleZ = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));

        /// <summary>
        /// On Play, triggers the scale animation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (AnimateScaleTarget != null))
            {
                if (isActiveAndEnabled)
                {
                    StartCoroutine(AnimateScale(AnimateScaleTarget, Vector3.zero, AnimateScaleDuration, AnimateScaleX, AnimateScaleY, AnimateScaleZ, Multiplier));
                }
            }
        }

        protected virtual IEnumerator AnimateScale(Transform targetTransform, Vector3 vector, float duration, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float multiplier = 1f)
        {
            if (targetTransform == null)
            {
                yield break;
            }

            if ((curveX == null) || (curveY == null) || (curveZ == null))
            {
                yield break;
            }

            if (duration == 0f)
            {
                yield break;
            }

            float journey = 0f;

            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);

                vector.x = AnimateX ? curveX.Evaluate(percent) : targetTransform.localScale.x;
                vector.y = AnimateY ? curveY.Evaluate(percent) : targetTransform.localScale.y;
                vector.z = AnimateZ ? curveZ.Evaluate(percent) : targetTransform.localScale.z;
                targetTransform.localScale = multiplier * vector + Offset * Vector3.one;

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            vector.x = AnimateX ? curveX.Evaluate(1f) : targetTransform.localScale.x;
            vector.y = AnimateY ? curveY.Evaluate(1f) : targetTransform.localScale.y;
            vector.z = AnimateZ ? curveZ.Evaluate(1f) : targetTransform.localScale.z;
            targetTransform.localScale = multiplier * vector + Offset * Vector3.one;

            yield return null;
        }
    }
}
