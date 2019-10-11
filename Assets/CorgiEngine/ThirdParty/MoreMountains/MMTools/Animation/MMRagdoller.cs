using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoreMountains.Tools
{
    public class MMRagdoller : MonoBehaviour
    {
        public Rigidbody MainRigidbody;

        public enum RagdollStates
        {
            Animated,
            Ragdolling,
            Blending
        }

        public class RagdollBodypart
        {
            public Transform BodyPartTransform;
            public Vector3 StoredPosition;
            public Quaternion StoredRotation;
        }

        public RagdollStates CurrentState = RagdollStates.Animated;
        public float RagdollToMecanimBlendDuration = 0.5f;

        protected float _mecanimToGetUpTransitionTime = 0.05f;
        protected float _ragdollingEndTimestamp = -100f;
        protected Vector3 _ragdolledHipPosition;
        protected Vector3 _ragdolledHeadPosition;
        protected Vector3 _ragdolledFeetPosition;
        protected List<RagdollBodypart> _bodyparts = new List<RagdollBodypart>();
        protected Animator _animator;

        public bool Ragdolling
        {
            get
            {
                return CurrentState != RagdollStates.Animated;
            }
            set
            {
                if (value == true)
                {
                    if (CurrentState == RagdollStates.Animated)
                    {
                        SetIsKinematic(false);
                        _animator.enabled = false;
                        CurrentState = RagdollStates.Ragdolling;
                    }
                }
                else
                {
                    if (CurrentState == RagdollStates.Ragdolling)
                    {
                        SetIsKinematic(true);
                        _ragdollingEndTimestamp = Time.time;
                        _animator.enabled = true;
                        CurrentState = RagdollStates.Blending;

                        foreach (RagdollBodypart bodypart in _bodyparts)
                        {
                            bodypart.StoredRotation = bodypart.BodyPartTransform.rotation;
                            bodypart.StoredPosition = bodypart.BodyPartTransform.position;
                        }

                        _ragdolledFeetPosition = 0.5f * (_animator.GetBoneTransform(HumanBodyBones.LeftToes).position + _animator.GetBoneTransform(HumanBodyBones.RightToes).position);
                        _ragdolledHeadPosition = _animator.GetBoneTransform(HumanBodyBones.Head).position;
                        _ragdolledHipPosition = _animator.GetBoneTransform(HumanBodyBones.Hips).position;

                        if (_animator.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0)
                        {
                            _animator.SetBool("GetUpFromBack", true);
                        }
                        else
                        {
                            _animator.SetBool("GetUpFromBelly", true);
                        }
                    }
                }
            }
        }

        protected virtual void Start()
        {
            SetIsKinematic(true);
            Component[] transforms = GetComponentsInChildren(typeof(Transform));
            foreach (Component component in transforms)
            {
                if (component.transform != this.transform)
                {
                    RagdollBodypart bodyPart = new RagdollBodypart { BodyPartTransform = component as Transform };
                    _bodyparts.Add(bodyPart);
                }
            }
            _animator = GetComponent<Animator>();
        }

        public virtual void SetIsKinematic(bool isKinematic)
        {
            Component[] rigidbodies = GetComponentsInChildren(typeof(Rigidbody));
            foreach (Component rigidbody in rigidbodies)
            {
                if (rigidbody.transform != this.transform)
                {
                    (rigidbody as Rigidbody).isKinematic = isKinematic;
                }
            }
        }

        protected virtual void LateUpdate()
        {
            _animator.SetBool("GetUpFromBelly", false);
            _animator.SetBool("GetUpFromBack", false);

            if (CurrentState == RagdollStates.Blending)
            {
                if (Time.time <= _ragdollingEndTimestamp + _mecanimToGetUpTransitionTime)
                {
                    Vector3 animatedToRagdolling = _ragdolledHipPosition - _animator.GetBoneTransform(HumanBodyBones.Hips).position;
                    Vector3 newRootPosition = transform.position + animatedToRagdolling;

                    RaycastHit[] hits = Physics.RaycastAll(new Ray(newRootPosition, Vector3.down));
                    newRootPosition.y = 0;
                    foreach (RaycastHit hit in hits)
                    {
                        if (!hit.transform.IsChildOf(transform))
                        {
                            newRootPosition.y = Mathf.Max(newRootPosition.y, hit.point.y);
                        }
                    }
                    transform.position = newRootPosition;

                    Vector3 ragdollingDirection = _ragdolledHeadPosition - _ragdolledFeetPosition;
                    ragdollingDirection.y = 0;

                    Vector3 meanFeetPosition = 0.5f * (_animator.GetBoneTransform(HumanBodyBones.LeftFoot).position + _animator.GetBoneTransform(HumanBodyBones.RightFoot).position);
                    Vector3 animatedDirection = _animator.GetBoneTransform(HumanBodyBones.Head).position - meanFeetPosition;
                    animatedDirection.y = 0;

                    transform.rotation *= Quaternion.FromToRotation(animatedDirection.normalized, ragdollingDirection.normalized);
                }
                float ragdollBlendAmount = 1.0f - (Time.time - _ragdollingEndTimestamp - _mecanimToGetUpTransitionTime) / RagdollToMecanimBlendDuration;
                ragdollBlendAmount = Mathf.Clamp01(ragdollBlendAmount);

                foreach (RagdollBodypart bodypart in _bodyparts)
                {
                    if (bodypart.BodyPartTransform != transform)
                    {
                        if (bodypart.BodyPartTransform == _animator.GetBoneTransform(HumanBodyBones.Hips))
                        {
                            bodypart.BodyPartTransform.position = Vector3.Lerp(bodypart.BodyPartTransform.position, bodypart.StoredPosition, ragdollBlendAmount);
                        }
                        bodypart.BodyPartTransform.rotation = Quaternion.Slerp(bodypart.BodyPartTransform.rotation, bodypart.StoredRotation, ragdollBlendAmount);
                    }
                }

                if (ragdollBlendAmount == 0)
                {
                    CurrentState = RagdollStates.Animated;
                    return;
                }
            }
        }
    }
}
