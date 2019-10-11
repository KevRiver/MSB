using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// A class needed on pushable objects if you want your character to be able to detect them
	/// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
	public class Pushable : MonoBehaviour 
	{
        /// the speed at which this object can be pushed
        public float PushSpeed = 3f;
        [Header("Collision")]
        /// the length of the raycast we cast to detect if the object is grounded
        public float GroundedRaycastLength = 1f;
        /// the layers this object considers as ground
        public LayerMask GroundedLayerMask;
        [ReadOnly]
        /// whether or not the object touches the ground this frame
        public bool Grounded = false;                
        [ReadOnly]
        /// the CorgiController currently pushing this object
        public CorgiController Pusher;
        [ReadOnly]
        /// the direction this object is being pushed towards
        public Vector2 Direction = Vector2.zero;

        protected CorgiController _corgiController;
        protected Collider2D _collider2D;
        protected Vector2 _leftColliderBounds;
        protected Vector2 _rightColliderBounds;

        /// <summary>
        /// On Awake we grab our components
        /// </summary>
        protected virtual void Awake()
        {
            _corgiController = this.gameObject.GetComponent<CorgiController>();
            _collider2D = this.gameObject.GetComponent<Collider2D>();            
        }
        
        /// <summary>
        /// Attaches a pusher controller to this pushable object
        /// </summary>
        /// <param name="pusher"></param>
        public virtual void Attach(CorgiController pusher)
        {
            Pusher = pusher;
        }

        /// <summary>
        /// Detaches the current pusher object from this pushable object
        /// </summary>
        /// <param name="pusher"></param>
        public virtual void Detach(CorgiController pusher)
        {
            if (pusher == Pusher)
            {
                Pusher = null;
                if (_corgiController != null)
                {
                    _corgiController.SetForce(Vector2.zero);
                }                
            }            
        }

        /// <summary>
        /// On Update, we apply a force if needed
        /// </summary>
        protected virtual void Update()
        {
            CheckIfGrounded();

            if (_corgiController == null)
            {
                return;
            }
            if (Pusher == null)
            {
                return;
            }

            if ((Pusher != null) && Grounded)
            {
                if ((Pusher.State.IsCollidingLeft && (Pusher.ExternalForce.x < 0) && (Pusher.transform.position.x < this.transform.position.x))
                    || (Pusher.State.IsCollidingRight && (Pusher.ExternalForce.x > 0) && (Pusher.transform.position.x > this.transform.position.x)))
                {
                    _corgiController.SetHorizontalForce(0f);
                }
                else
                {
                    _corgiController.SetHorizontalForce(Pusher.ExternalForce.x);
                }                
            }

            if ((Pusher != null) && (!Grounded))
            {
                Detach(Pusher);
            }
        }

        /// <summary>
        /// Casts rays below the object to check if it's grounded this frame
        /// </summary>
        protected virtual void CheckIfGrounded()
        {
            _leftColliderBounds = _collider2D.bounds.min;
            _rightColliderBounds = _collider2D.bounds.max;
            _rightColliderBounds.y = _leftColliderBounds.y;

            RaycastHit2D hitLeft = MMDebug.RayCast(_leftColliderBounds, Vector2.down, GroundedRaycastLength, GroundedLayerMask, Color.green, true);
            RaycastHit2D hitRight = MMDebug.RayCast(_rightColliderBounds, Vector2.down, GroundedRaycastLength, GroundedLayerMask, Color.green, true);

            Grounded = (hitLeft || hitRight);
        }
    }
}