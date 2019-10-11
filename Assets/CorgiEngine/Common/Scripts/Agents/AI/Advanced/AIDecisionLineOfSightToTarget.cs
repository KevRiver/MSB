using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Use this decision to make sure there is an unobstructed line of sight between this AI and its current target
    /// </summary>
    public class AIDecisionLineOfSightToTarget : AIDecision
    {
        public LayerMask ObstacleLayerMask;
        /// the offset to apply (from the collider's center)
        public Vector3 LineOfSightOffset = new Vector3(0, 0, 0);

        protected Vector2 _directionToTarget;
        protected Collider2D _collider;
        protected Vector3 _raycastOrigin;

        /// <summary>
        /// On init we grab our collider component
        /// </summary>
        public override void Initialization()
        {
            _collider = this.gameObject.GetComponent<Collider2D>();
        }

        /// <summary>
        /// On Decide we check whether we've got a line of sight or not
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return CheckLineOfSight();
        }

        /// <summary>
        /// Casts a ray towards the target to see if there's an obstacle in between or not
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckLineOfSight()
        {
            if (_brain.Target == null)
            {
                return false;
            }

            _raycastOrigin = _collider.bounds.center + LineOfSightOffset / 2;
            _directionToTarget = _brain.Target.transform.position - _raycastOrigin;
                        
            RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _directionToTarget.normalized, _directionToTarget.magnitude, ObstacleLayerMask, Color.yellow, true);

            if (hit.collider == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
