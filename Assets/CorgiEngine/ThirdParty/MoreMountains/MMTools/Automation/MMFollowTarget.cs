using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Add this component to an object and it'll get moved towards the target at update, with or without interpolation based on your settings
    /// </summary>
    public class MMFollowTarget : MonoBehaviour
    {
        [Header("Activity")]
        /// whether or not the object is currently following its target
        public bool FollowPosition = true;
        public bool FollowRotation = true;

        [Header("Target")]
        /// the target to follow
        public Transform Target;
        /// the offset to apply to the followed target
        public Vector3 Offset;
        ///
        public bool AddInitialDistanceXToXOffset = false;
        public bool AddInitialDistanceYToYOffset = false;
        public bool AddInitialDistanceZToZOffset = false;

        [Header("Interpolation")]
        /// whether or not we need to interpolate the movement
        public bool InterpolatePosition = true;
        /// whether or not we need to interpolate the movement
        public bool InterpolateRotation = true;
        /// the speed at which to interpolate the follower's movement
        public float FollowPositionSpeed = 10f;
        /// the speed at which to interpolate the follower's rotation
        public float FollowRotationSpeed = 10f;

        /// the possible update modes
        public enum Modes { Update, FixedUpdate, LateUpdate }
        [Header("Mode")]
        /// the update at which the movement happens
        public Modes UpdateMode = Modes.Update;

        [Header("Axis")]
        /// whether this object should follow its target on the X axis
        public bool FollowPositionX = true;
        /// whether this object should follow its target on the Y axis
        public bool FollowPositionY = true;
        /// whether this object should follow its target on the Z axis
        public bool FollowPositionZ = true;
        
        protected Vector3 _newTargetPosition;        
        protected Vector3 _initialPosition;

        protected Quaternion _newTargetRotation;
        protected Quaternion _initialRotation;

        /// <summary>
        /// On start we store our initial position
        /// </summary>
        protected virtual void Start()
        {
            SetInitialPosition();
            SetOffset();
        }

        /// <summary>
        /// Prevents the object from following the target anymore
        /// </summary>
        public virtual void StopFollowing()
        {
            FollowPosition = false;
        }

        /// <summary>
        /// Makes the object follow the target
        /// </summary>
        public virtual void StartFollowing()
        {
            FollowPosition = true;
            SetInitialPosition();
        }

        /// <summary>
        /// Stores the initial position
        /// </summary>
        protected virtual void SetInitialPosition()
        {
            _initialPosition = this.transform.position;
            _initialRotation = this.transform.rotation;
        }

        protected virtual void SetOffset()
        {
            Vector3 difference = this.transform.position - Target.transform.position;
            Offset.x = AddInitialDistanceXToXOffset ? difference.x : Offset.x;
            Offset.y = AddInitialDistanceYToYOffset ? difference.y : Offset.y;
            Offset.z = AddInitialDistanceZToZOffset ? difference.z : Offset.z;
        }

        /// <summary>
        /// At update we follow our target 
        /// </summary>
        protected virtual void Update()
        {
            if (UpdateMode == Modes.Update)
            {
                FollowTargetPosition();
                FollowTargetRotation();
            }
        }

        /// <summary>
        /// At fixed update we follow our target 
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (UpdateMode == Modes.FixedUpdate)
            {
                FollowTargetPosition();
            }
        }

        /// <summary>
        /// At late update we follow our target 
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (UpdateMode == Modes.LateUpdate)
            {
                FollowTargetPosition();
            }
        }

        /// <summary>
        /// Follows the target, lerping the position or not based on what's been defined in the inspector
        /// </summary>
        protected virtual void FollowTargetPosition()
        {
            if (Target == null)
            {
                return;
            }

            if (!FollowPosition)
            {
                return;
            }

            _newTargetPosition = Target.position + Offset;
            if (!FollowPositionX) { _newTargetPosition.x = _initialPosition.x; }
            if (!FollowPositionY) { _newTargetPosition.y = _initialPosition.y; }
            if (!FollowPositionZ) { _newTargetPosition.z = _initialPosition.z; }

            if (InterpolatePosition)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, _newTargetPosition, Time.deltaTime * FollowPositionSpeed);
            }
            else
            {
                this.transform.position = _newTargetPosition;
            }
        }

        protected virtual void FollowTargetRotation()
        {
            if (Target == null)
            {
                return;
            }

            if (!FollowPosition)
            {
                return;
            }

            _newTargetRotation = Target.rotation;

            if (InterpolateRotation)
            {
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, _newTargetRotation, Time.deltaTime * FollowRotationSpeed);
            }
            else
            {
                this.transform.rotation = _newTargetRotation;
            }
        }
    }
}