using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback animates the rotation of the specified object when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will animate the target's rotation on the 3 specified animation curves (one per axis), for the specified duration (in seconds).")]
    [FeedbackPath("GameObject/Rotation")]
    public class MMFeedbackRotation : MMFeedback
    {
        public enum TimeScales { Scaled, Unscaled }

        [Header("Rotation Target")]
        /// the object whose rotation you want to animate
        public Transform AnimateRotationTarget;
        
        [Header("Animation")]
        /// whether this feedback should play in scaled or unscaled time
        public TimeScales TimeScale = TimeScales.Scaled;
        /// the duration of the transition
        public float AnimateRotationDuration = 0.2f;
        /// how much each curve should be multiplied by
        public float Multiplier = 360f;
        /// if this is true, should animate the X rotation
        public bool AnimateX = true;
        /// how the x part of the rotation should animate over time, in degrees
        [MMFCondition("AnimateX", true)]
        public AnimationCurve AnimateRotationX = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        /// if this is true, should animate the X rotation
        public bool AnimateY = true;
        /// how the y part of the rotation should animate over time, in degrees
        [MMFCondition("AnimateY", true)]
        public AnimationCurve AnimateRotationY = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        /// if this is true, should animate the X rotation
        public bool AnimateZ = true;
        /// how the z part of the rotation should animate over time, in degrees
        [MMFCondition("AnimateZ", true)]
        public AnimationCurve AnimateRotationZ = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));

        /// <summary>
        /// On play, we trigger our rotation animation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (AnimateRotationTarget != null))
            {
                if (isActiveAndEnabled)
                {
                    StartCoroutine(AnimateRotation(AnimateRotationTarget, Vector3.zero, AnimateRotationDuration, AnimateRotationX, AnimateRotationY, AnimateRotationZ, Multiplier));
                }
            }
        }

        protected virtual IEnumerator AnimateRotation(Transform targetTransform,
                                                    Vector3 vector,
                                                    float duration,
                                                    AnimationCurve curveX,
                                                    AnimationCurve curveY,
                                                    AnimationCurve curveZ,
                                                    float multiplier)
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

                vector.x = AnimateX ? curveX.Evaluate(percent) * multiplier : targetTransform.localEulerAngles.x;
                vector.y = AnimateY ? curveY.Evaluate(percent) * multiplier : targetTransform.localEulerAngles.y;
                vector.z = AnimateZ ? curveZ.Evaluate(percent) * multiplier : targetTransform.localEulerAngles.z;
                targetTransform.localEulerAngles = vector;

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            vector.x = AnimateX ? curveX.Evaluate(1f) * multiplier : targetTransform.localEulerAngles.x;
            vector.y = AnimateY ? curveY.Evaluate(1f) * multiplier : targetTransform.localEulerAngles.y;
            vector.z = AnimateZ ? curveZ.Evaluate(1f) * multiplier : targetTransform.localEulerAngles.z;
            targetTransform.localEulerAngles = vector;

            yield break;
        }
    }
}
