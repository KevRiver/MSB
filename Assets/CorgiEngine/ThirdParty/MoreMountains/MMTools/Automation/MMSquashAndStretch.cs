using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    /// This component will automatically update scale and rotation 
    /// Put it one level below the top, and have the model one level below that
    /// Hierarchy should be as follows :
    /// 
    /// Parent (where the logic (and optionnally rigidbody lies)
    /// - MMSquashAndStretch
    /// - - Model / sprite
    /// 
    /// Make sure this intermediary layer only has one child
    /// If movement feels glitchy make sure your rigidbody is on Interpolate
    /// </summary>
    public class MMSquashAndStretch : MonoBehaviour
    {

        public enum Modes { Rigidbody, Rigidbody2D, Position }

        [Information("This component will apply squash and stretch based on velocity (either position based or computed from a Rigidbody. It has to be put on an intermediary level in the hierarchy, between the logic (top level) and the model (bottom level).", InformationAttribute.InformationType.Info, false)]
        [Header("Velocity Detection")]
        /// the possible ways to get velocity from
        public Modes Mode = Modes.Position;
        
        [Header("Settings")]
        /// the intensity of the squash and stretch
        public float Intensity = 0.02f;
        /// the maximum velocity of your parent object, used to remap the computed one
        public float MaximumVelocity = 1f;
        /// the minimum scale to apply to this object
        public Vector2 MinimumScale = new Vector2(0.5f, 0.5f);
        /// the maximum scale to apply to this object
        public Vector2 MaximumScale = new Vector2(2f, 2f);

        protected Rigidbody2D _rigidbody2D;
        protected Rigidbody _rigidbody;

        protected Transform _childTransform;
        protected Transform _parentTransform;
        protected Vector3 _direction;
        protected Vector3 _previousPosition;
        protected Vector3 _newLocalScale;
        protected Vector3 _initialScale;
        protected Quaternion _newRotation = Quaternion.identity;
        protected Quaternion _deltaRotation;

        [Header("Debug")]
        [ReadOnly]
        /// the current velocity of the parent object
        public Vector3 Velocity;
        [ReadOnly]
        /// the remapped velocity
        public float RemappedVelocity;
        [ReadOnly]
        /// the current velocity magnitude
        public float VelocityMagnitude;

        /// <summary>
        /// On start, we initialize our component
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        /// Stores the initial scale, grabs the rigidbodies (or tries to), as well as the parent and child
        /// </summary>
        protected virtual void Initialization()
        {
            _initialScale = this.transform.localScale;

            _rigidbody = this.transform.parent.GetComponent<Rigidbody>();
            _rigidbody2D = this.transform.parent.GetComponent<Rigidbody2D>();

            _childTransform = this.transform.GetChild(0).transform;
            _parentTransform = this.transform.parent.GetComponent<Transform>();
        }
        
        /// <summary>
        /// On late update, we apply our squash and stretch effect
        /// </summary>
        protected virtual void LateUpdate()
        {
            SquashAndStretch();
        }

        /// <summary>
        /// Computes velocity and applies the effect
        /// </summary>
        protected virtual void SquashAndStretch()
        {
            if (Time.deltaTime <= 0f)
            {
                return;
            }

            ComputeVelocityAndDirection();
            ComputeNewRotation();
            ComputeNewLocalScale();
            StorePreviousPosition();
        }

        /// <summary>
        /// Determines the current velocity and direction of the parent object
        /// </summary>
        protected virtual void ComputeVelocityAndDirection()
        {
            Velocity = Vector3.zero;

            switch (Mode)
            {
                case Modes.Rigidbody:
                    Velocity = _rigidbody.velocity;
                    break;

                case Modes.Rigidbody2D:
                    Velocity = _rigidbody2D.velocity;
                    break;

                case Modes.Position:
                    Velocity = (_previousPosition - _parentTransform.position) / Time.deltaTime;
                    break;
            }

            VelocityMagnitude = Velocity.magnitude;
            RemappedVelocity = MMMaths.Remap(VelocityMagnitude, 0f, MaximumVelocity, 0f, 1f);
            _direction = Vector3.Normalize(Velocity);
        }

        /// <summary>
        /// Computes a new rotation for both this object and the child
        /// </summary>
        protected virtual void ComputeNewRotation()
        {
            if (VelocityMagnitude > 0.01f)
            {
                _newRotation = Quaternion.FromToRotation(Vector3.up, _direction);
            }
            _deltaRotation = _parentTransform.rotation;
            this.transform.rotation = _newRotation;
            _childTransform.rotation = _deltaRotation;
        }

        /// <summary>
        /// Computes a new local scale for this object
        /// </summary>
        protected virtual void ComputeNewLocalScale()
        {
            _newLocalScale.x = Mathf.Clamp01(1f / (RemappedVelocity + 0.001f));
            _newLocalScale.y = RemappedVelocity;
            _newLocalScale.z = Mathf.Clamp01(1f / (RemappedVelocity + 0.001f));

            _newLocalScale = Vector3.Lerp(Vector3.one, _newLocalScale, VelocityMagnitude * Intensity);

            _newLocalScale.x = Mathf.Clamp(_newLocalScale.x, MinimumScale.x, MaximumScale.x);
            _newLocalScale.y = Mathf.Clamp(_newLocalScale.y, MinimumScale.y, MaximumScale.y);

            this.transform.localScale = _newLocalScale;
        }

        /// <summary>
        /// Stores the previous position of the parent to compute velocity
        /// </summary>
        protected virtual void StorePreviousPosition()
        {
            _previousPosition = _parentTransform.position;
        }
    }
}
