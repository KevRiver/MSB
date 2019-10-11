using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This decision will return true if an object on its TargetLayer layermask is within its specified radius, false otherwise. It will also set the Brain's Target to that object.
    /// </summary>
    public class AIDecisionDetectTargetRadius : AIDecision
    {
        /// the radius to search our target in
        public float Radius = 3f;
        /// the center of the search circle
        public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);
        /// the layer(s) to search our target on
        public LayerMask TargetLayer;

        protected Vector2 _facingDirection;
        protected Vector2 _raycastOrigin;
        protected Character _character;
        protected Collider2D _detectionCollider = null;
        protected Color _gizmoColor = Color.yellow;
        protected bool _init = false;

        /// <summary>
        /// On init we grab our Character component
        /// </summary>
        public override void Initialization()
        {
            _character = this.gameObject.GetComponent<Character>();
            _gizmoColor.a = 0.25f;
            _init = true;
        }

        /// <summary>
        /// On Decide we check for our target
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return DetectTarget();
        }

        /// <summary>
        /// Returns true if a target is found within the circle
        /// </summary>
        /// <returns></returns>
        protected virtual bool DetectTarget()
        {
            _detectionCollider = null;
            
            _facingDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
            // we cast a ray to the left of the agent to check for a Player
            _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
            _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

            _detectionCollider = Physics2D.OverlapCircle(_raycastOrigin, Radius, TargetLayer);     
            if (_detectionCollider == null)
            {
                return false;
            }
            else
            {
                _brain.Target = _detectionCollider.gameObject.transform;
                return true;
            }
        }

        /// <summary>
        /// Draws gizmos for the detection circle
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
            _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_raycastOrigin, Radius);
            if (_init)
            {
                Gizmos.color = _gizmoColor;
                Gizmos.DrawSphere(_raycastOrigin, Radius);
            }            
        }
    }
}
