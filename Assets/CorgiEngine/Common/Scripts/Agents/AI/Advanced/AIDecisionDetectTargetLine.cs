using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This Decision will return true if any object on its TargetLayer layermask enters its line of sight. It will also set the Brain's Target to that object. You can choose to have it in ray mode, in which case its line of sight will be an actual line (a raycast), or have it be wider (in which case it'll use a spherecast). You can also specify an offset for the ray's origin, and an obstacle layer mask that will block it.
    /// </summary>
    public class AIDecisionDetectTargetLine : AIDecision
    {
        /// the possible detection methods
        public enum DetectMethods { Ray, WideRay }
        /// the possible detection directions
        public enum DetectionDirections { Front, Back, Both }
        /// the detection method
        public DetectMethods DetectMethod = DetectMethods.Ray;
        /// the detection direction
        public DetectionDirections DetectionDirection = DetectionDirections.Front;
        /// the width of the ray to cast (if we're in WideRay mode only
        public float RayWidth = 1f;
        /// the distance up to which we'll cast our rays
        public float DetectionDistance = 10f;
        /// the offset to apply to the ray(s)
        public Vector3 DetectionOriginOffset = new Vector3(0,0,0);
        /// the layer(s) on which we want to search a target on
        public LayerMask TargetLayer;
        /// the layer(s) on which obstacles are set. Obstacles will block the ray
        public LayerMask ObstaclesLayer;

        protected Vector2 _direction;
        protected Vector2 _facingDirection;
        protected float _distanceToTarget;
        protected Vector2 _raycastOrigin;
        protected Character _character;
        protected bool _drawLeftGizmo = false;
        protected bool _drawRightGizmo = false;
        protected Color _gizmosColor = Color.yellow;
        protected Vector3 _gizmoCenter;
        protected Vector3 _gizmoSize;
        protected bool _init = false;
        protected Vector2 _boxcastSize = Vector2.zero;
        
        /// <summary>
        /// On Init we grab our character
        /// </summary>
        public override void Initialization()
        {
            _character = this.gameObject.GetComponent<Character>();
            _gizmosColor.a = 0.25f;
            _init = true;
        }

        /// <summary>
        /// On Decide we look for a target
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return DetectTarget();
        }

        /// <summary>
        /// Returns true if a target is found by the ray
        /// </summary>
        /// <returns></returns>
        protected virtual bool DetectTarget()
        {
            bool hit = false;
            _distanceToTarget = 0;
            Transform target = null;
            RaycastHit2D raycast;
            _drawLeftGizmo = false;
            _drawRightGizmo = false;

            _boxcastSize.x = DetectionDistance / 5f;
            _boxcastSize.y = RayWidth;

            _facingDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
            // we cast a ray to the left of the agent to check for a Player
            _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
            _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

            // we cast it to the left	
            if ((DetectionDirection == DetectionDirections.Both)
                || ((DetectionDirection == DetectionDirections.Front) && (!_character.IsFacingRight))
                || ((DetectionDirection == DetectionDirections.Back) && (_character.IsFacingRight)))
            {
                if (DetectMethod == DetectMethods.Ray)
                {
                    raycast = MMDebug.RayCast(_raycastOrigin, Vector2.left, DetectionDistance, TargetLayer, MoreMountains.Tools.Colors.Gold, true);
                }
                else
                {
                    raycast = Physics2D.BoxCast(_raycastOrigin + Vector2.right * _boxcastSize.x / 2f, _boxcastSize, 0f, Vector2.left, DetectionDistance, TargetLayer);
                    MMDebug.RayCast(_raycastOrigin + Vector2.up * RayWidth/2f, Vector2.left, DetectionDistance, TargetLayer, MoreMountains.Tools.Colors.Gold, true);
                    MMDebug.RayCast(_raycastOrigin - Vector2.up * RayWidth / 2f, Vector2.left, DetectionDistance, TargetLayer, MoreMountains.Tools.Colors.Gold, true);
                    MMDebug.RayCast(_raycastOrigin - Vector2.up * RayWidth / 2f + Vector2.left * DetectionDistance, Vector2.up, RayWidth, TargetLayer, MoreMountains.Tools.Colors.Gold, true);
                    _drawLeftGizmo = true;
                }
                
                // if we see a player
                if (raycast)
                {
                    hit = true;
                    _direction = Vector2.left;
                    _distanceToTarget = Vector2.Distance(_raycastOrigin, raycast.point);
                    target = raycast.collider.gameObject.transform;
                }
            }

            // we cast a ray to the right of the agent to check for a Player	
            if ((DetectionDirection == DetectionDirections.Both)
               || ((DetectionDirection == DetectionDirections.Front) && (_character.IsFacingRight))
               || ((DetectionDirection == DetectionDirections.Back) && (!_character.IsFacingRight)))
            {
                if (DetectMethod == DetectMethods.Ray)
                {
                    raycast = MMDebug.RayCast(_raycastOrigin, Vector2.right, DetectionDistance, TargetLayer, Colors.DarkOrange, true);
                }
                else
                {
                    raycast = Physics2D.BoxCast(_raycastOrigin - Vector2.right * _boxcastSize.x / 2f, _boxcastSize, 0f, Vector2.right, DetectionDistance, TargetLayer);
                    MMDebug.RayCast(_raycastOrigin + Vector2.up * RayWidth / 2f, Vector2.right, DetectionDistance, TargetLayer, MoreMountains.Tools.Colors.DarkOrange, true);
                    MMDebug.RayCast(_raycastOrigin - Vector2.up * RayWidth / 2f, Vector2.right, DetectionDistance, TargetLayer, MoreMountains.Tools.Colors.DarkOrange, true);
                    MMDebug.RayCast(_raycastOrigin - Vector2.up * RayWidth / 2f + Vector2.right * DetectionDistance, Vector2.up, RayWidth, TargetLayer, MoreMountains.Tools.Colors.DarkOrange, true);
                    _drawLeftGizmo = true;
                }
                
                if (raycast)
                {
                    hit = true;
                    _direction = Vector2.right;
                    _distanceToTarget = Vector2.Distance(_raycastOrigin, raycast.point);
                    target = raycast.collider.gameObject.transform;
                }
            }

            if (hit)
            {
                // we make sure there isn't an obstacle in between
                float distance = Vector2.Distance((Vector2)target.transform.position, _raycastOrigin);
                RaycastHit2D raycastObstacle = MMDebug.RayCast(_raycastOrigin, ((Vector2)target.transform.position - _raycastOrigin).normalized, distance, ObstaclesLayer, Color.gray, true);
                
                if (raycastObstacle && _distanceToTarget > raycastObstacle.distance)
                {
                    _brain.Target = null;
                    return false;
                }
                else
                {
                    // if there's no obstacle, we store our target and return true
                    _brain.Target = target;
                    return true;
                }
            }
            _brain.Target = null;
            return false;           
        }
        
        /// <summary>
        /// Draws ray gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if ((DetectMethod != DetectMethods.WideRay) || !_init)
            {
                return;
            }

            Gizmos.color = _gizmosColor;

            _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
            _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

            if ((DetectionDirection == DetectionDirections.Both)
                || ((DetectionDirection == DetectionDirections.Front) && (!_character.IsFacingRight))
                || ((DetectionDirection == DetectionDirections.Back) && (_character.IsFacingRight)))
            {
                _gizmoCenter = (Vector3)_raycastOrigin + Vector3.left * DetectionDistance / 2f;
                _gizmoSize.x = DetectionDistance;
                _gizmoSize.y = RayWidth;
                _gizmoSize.z = 1f;
                Gizmos.DrawCube(_gizmoCenter, _gizmoSize);
            }

            if ((DetectionDirection == DetectionDirections.Both)
               || ((DetectionDirection == DetectionDirections.Front) && (_character.IsFacingRight))
               || ((DetectionDirection == DetectionDirections.Back) && (!_character.IsFacingRight)))
            {
                _gizmoCenter = (Vector3)_raycastOrigin + Vector3.right * DetectionDistance / 2f;
                _gizmoSize.x = DetectionDistance;
                _gizmoSize.y = RayWidth;
                _gizmoSize.z = 1f;
                Gizmos.DrawCube(_gizmoCenter, _gizmoSize);
            }
        }
    }
}
