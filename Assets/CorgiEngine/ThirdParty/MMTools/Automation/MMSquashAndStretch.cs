using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    /// This component will automatically update scale and rotation 
    /// Put it one level below the top, and have the model one level below that
    /// </summary>
    public class MMSquashAndStretch : MonoBehaviour
    {
        public enum Modes { Rigidbody, Rigidbody2D, Position }

        [Header("Velocity Detection")]
        public Modes Mode = Modes.Position;
        
        [Header("Settings")]
        public float Intensity;
        public float MaximumVelocity;

        protected Rigidbody2D _rigidbody2D;
        protected Rigidbody _rigidbody;

        protected Transform _rotatingChildTransform;
        protected Transform _topLevelTransform;
        protected Vector3 _direction;
        protected Vector3 _previousPosition;
        protected Vector3 _newLocalScale;
        protected Vector3 _initialScale;
        protected Quaternion _newRotation = Quaternion.identity;

        [ReadOnly]
        public Vector3 Velocity;
        [ReadOnly]
        public float RemappedVelocity;
        [ReadOnly]
        public float VelocityMagnitude;
        
        protected virtual void Start()
        {
            _initialScale = this.transform.localScale;

            _rigidbody = this.transform.parent.GetComponent<Rigidbody>();
            _rigidbody2D = this.transform.parent.GetComponent<Rigidbody2D>();

            _rotatingChildTransform = this.transform.GetChild(0).transform;
            _topLevelTransform = this.transform.parent.GetComponent<Transform>();
        }
        
        protected virtual void LateUpdate()
        {
            ComputeVelocityAndDirection();
            ComputeNewRotation();
            ComputeNewLocalScale();
            StorePosition();
        }

        protected virtual void ComputeVelocityAndDirection()
        {
            Velocity = Vector3.zero;

            if (Mode == Modes.Rigidbody)
            {
                Velocity = _rigidbody.velocity;
            }
            if (Mode == Modes.Rigidbody2D)
            {
                Velocity = _rigidbody.velocity;
            }
            if (Mode == Modes.Position)
            {
                Velocity = (_previousPosition - this.transform.position);
            }
            VelocityMagnitude = Velocity.magnitude;
            RemappedVelocity = MMMaths.Remap(VelocityMagnitude, 0f, MaximumVelocity, 0f, 1f);
            _direction = Vector3.Normalize(Velocity);
        }

        protected virtual void ComputeNewRotation()
        {
            if (VelocityMagnitude > 0.01f)
            {
                _newRotation = Quaternion.FromToRotation(Vector3.up, _direction);
            }
            Quaternion deltaRotation = _topLevelTransform.rotation;
            this.transform.rotation = _newRotation;
            _rotatingChildTransform.rotation = deltaRotation;
        }

        protected virtual void ComputeNewLocalScale()
        {
            _newLocalScale.x = Mathf.Clamp01(1f / (RemappedVelocity + 0.001f));
            _newLocalScale.y = RemappedVelocity;
            _newLocalScale.z = Mathf.Clamp01(1f / (RemappedVelocity + 0.001f));

            _newLocalScale = Vector3.Lerp(Vector3.one, _newLocalScale, VelocityMagnitude * Intensity);

            this.transform.localScale = _newLocalScale;
        }

        protected virtual void StorePosition()
        {
            _previousPosition = this.transform.position;
        }
    }
}
