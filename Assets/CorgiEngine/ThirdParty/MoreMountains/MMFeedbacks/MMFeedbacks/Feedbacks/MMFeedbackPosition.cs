using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// this feedback will let you animate the position of 
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will animate the target object's position over time, for the specified duration, from the chosen initial position to the chosen destination. These can either be relative Vector3 offsets from the Feedback's position, or Transforms. If you specify transforms, the Vector3 values will be ignored.")]
    [FeedbackPath("GameObject/Position")]
    public class MMFeedbackPosition : MMFeedback
    {
        public enum Spaces { World, Local, RectTransform }
        public enum Modes { AtoB, AlongCurve }
        public enum TimeScales { Scaled, Unscaled }

        [Header("Position Target")]
        /// the object this feedback will animate the position for
        public GameObject AnimatePositionTarget;

        [Header("Animation")]
        /// the mode this animation should follow (either going from A to B, or moving along a curve)
        public Modes Mode = Modes.AtoB;
        /// whether this feedback should play in scaled or unscaled time
        public TimeScales TimeScale = TimeScales.Scaled;
        /// the space in which to move the position in
        public Spaces Space = Spaces.World;
        /// the duration of the animation on play
        public float AnimatePositionDuration = 0.2f;
        /// the acceleration of the movement
        [MMFEnumCondition("Mode", (int)Modes.AtoB)]
        public AnimationCurve AnimatePositionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.1f, 0.05f), new Keyframe(0.9f, 0.95f), new Keyframe(1, 1));
        /// the multiplier to apply to the curve
        [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        public float CurveMultiplier = 1f;
        /// if this is true, the x position will be animated
        [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        public bool AnimateX;
        /// the acceleration of the movement
        [MMFCondition("AnimateX", true)]
        public AnimationCurve AnimatePositionCurveX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1, 0f));
        /// if this is true, the y position will be animated
        [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        public bool AnimateY;
        /// the acceleration of the movement
        [MMFCondition("AnimateY", true)]
        public AnimationCurve AnimatePositionCurveY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1, 0f));
        /// if this is true, the z position will be animated
        [MMFEnumCondition("Mode", (int)Modes.AlongCurve)]
        public bool AnimateZ;
        /// the acceleration of the movement
        [MMFCondition("AnimateZ", true)]
        public AnimationCurve AnimatePositionCurveZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1, 0f));

        [Header("Positions")]
        /// if this is true, the initial position won't be added to init and destination
        public bool RelativePosition = true;
        /// the initial position
        public Vector3 InitialPosition;
        /// the destination position
        [MMFEnumCondition("Mode", (int)Modes.AtoB)]
        public Vector3 DestinationPosition;
        /// the initial transform - if set, takes precedence over the Vector3 above
        public Transform InitialPositionTransform;
        /// the destination transform - if set, takes precedence over the Vector3 above
        [MMFEnumCondition("Mode", (int)Modes.AtoB)]
        public Transform DestinationPositionTransform;

        protected Vector3 _newPosition;
        protected RectTransform _rectTransform;

        /// <summary>
        /// On init, we set our initial and destination positions (transform will take precedence over vector3s)
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (Active)
            {
                if (AnimatePositionTarget == null)
                {
                    Debug.LogWarning("The animate position target for " + this + " is null, you have to define it in the inspector");
                    return;
                }

                if (Space == Spaces.RectTransform)
                {
                    _rectTransform = AnimatePositionTarget.GetComponent<RectTransform>();
                }

                if (InitialPositionTransform != null) 
                {
                    InitialPosition = GetPosition(InitialPositionTransform);                    
                }
                else
                {
                    InitialPosition = RelativePosition ? GetPosition(AnimatePositionTarget.transform) + InitialPosition : InitialPosition;
                }

                if (DestinationPositionTransform != null)
                {
                    DestinationPosition = GetPosition(DestinationPositionTransform);
                }
                else
                {
                    DestinationPosition = RelativePosition ? GetPosition(AnimatePositionTarget.transform) + DestinationPosition : DestinationPosition;
                }
            }
        }

        /// <summary>
        /// On Play, we move our object from A to B
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (AnimatePositionTarget != null))
            {
                if (isActiveAndEnabled)
                {
                    switch(Mode)
                    {
                        case Modes.AtoB:
                            StartCoroutine(MoveFromTo(AnimatePositionTarget, InitialPosition, DestinationPosition, AnimatePositionDuration, AnimatePositionCurve));
                            break;
                        case Modes.AlongCurve:
                            StartCoroutine(MoveAlongCurve(AnimatePositionTarget, InitialPosition, AnimatePositionDuration));
                            break;
                    }
                    
                }
            }
        }

        /// <summary>
        /// Moves the object along a curve
        /// </summary>
        /// <param name="movingObject"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="duration"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        protected virtual IEnumerator MoveAlongCurve(GameObject movingObject, Vector3 initialPosition, float duration)
        {
            float journey = 0f;

            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);

                ComputeNewCurvePosition(movingObject, initialPosition, percent);

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }
            ComputeNewCurvePosition(movingObject, initialPosition, 1f);
            yield break;
        }

        protected virtual void ComputeNewCurvePosition(GameObject movingObject, Vector3 initialPosition, float percent)
        {
            float newValueX = CurveMultiplier * AnimatePositionCurveX.Evaluate(percent);
            float newValueY = CurveMultiplier * AnimatePositionCurveY.Evaluate(percent);
            float newValueZ = CurveMultiplier * AnimatePositionCurveZ.Evaluate(percent);

            _newPosition = initialPosition;

            if (RelativePosition)
            {
                _newPosition.x = AnimateX ? initialPosition.x + newValueX : initialPosition.x;
                _newPosition.y = AnimateY ? initialPosition.y + newValueY : initialPosition.y;
                _newPosition.z = AnimateZ ? initialPosition.z + newValueZ : initialPosition.z;

            }
            else
            {
                _newPosition.x = AnimateX ? newValueX : initialPosition.x;
                _newPosition.y = AnimateY ? newValueY : initialPosition.y;
                _newPosition.z = AnimateZ ? newValueZ : initialPosition.z;
            }

            SetPosition(movingObject.transform, _newPosition);
        }

        /// <summary>
		/// Moves an object from point A to point B in a given time
		/// </summary>
		/// <param name="movingObject">Moving object.</param>
		/// <param name="pointA">Point a.</param>
		/// <param name="pointB">Point b.</param>
		/// <param name="duration">Time.</param>
		protected virtual IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, float duration, AnimationCurve curve = null)
        {
            float journey = 0f;

            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);

                _newPosition = Vector3.Lerp(pointA, pointB, curve.Evaluate(percent));

                SetPosition(movingObject.transform, _newPosition);

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            // set final position
            SetPosition(movingObject.transform, pointB);

            yield break;
        }

        protected virtual Vector3 GetPosition(Transform target)
        {
            switch (Space)
            {
                case Spaces.World:
                    return target.position;
                case Spaces.Local:
                    return target.localPosition;
                case Spaces.RectTransform:
                    return target.gameObject.GetComponent<RectTransform>().anchoredPosition;
            }
            return Vector3.zero;
        }

        protected virtual void SetPosition(Transform target, Vector3 newPosition)
        {
            switch (Space)
            {
                case Spaces.World:
                    target.position = newPosition;
                    break;
                case Spaces.Local:
                    target.localPosition = newPosition;
                    break;
                case Spaces.RectTransform:
                    _rectTransform.anchoredPosition = newPosition;
                    break;
            }

        }
    }
}
