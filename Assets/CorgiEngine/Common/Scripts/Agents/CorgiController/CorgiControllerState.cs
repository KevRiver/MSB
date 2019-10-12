using UnityEngine;
using System.Collections;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// The various states you can use to check if your character is doing something at the current frame
	/// </summary>

	public class CorgiControllerState 
	{
		/// is the character colliding right ?
		public bool IsCollidingRight { get; set; }
		/// is the character colliding left ?
		public bool IsCollidingLeft { get; set; }
		/// is the character colliding with something above it ?
		public bool IsCollidingAbove { get; set; }
		/// is the character colliding with something above it ?
		public bool IsCollidingBelow { get; set; }
		/// is the character colliding with anything ?
		public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; }}

        /// returns the distance to the left collider, equals -1 if not colliding left
        public float DistanceToLeftCollider;
        /// returns the distance to the right collider, equals -1 if not colliding right
        public float DistanceToRightCollider;

        /// returns the slope angle met horizontally
        public float LateralSlopeAngle { get; set; }
		/// returns the slope the character is moving on angle
		public float BelowSlopeAngle { get; set; }
		/// returns true if the slope angle is ok to walk on
		public bool SlopeAngleOK { get; set; }
		/// returns true if the character is standing on a moving platform
		public bool OnAMovingPlatform { get; set; }
		
		/// Is the character grounded ? 
		public bool IsGrounded { get { return IsCollidingBelow; } }
		/// is the character falling right now ?
		public bool IsFalling { get; set; }
        /// is the character falling right now ?
		public bool IsJumping { get; set; }
        /// was the character grounded last frame ?
        public bool WasGroundedLastFrame { get ; set; }
		/// was the character grounded last frame ?
		public bool WasTouchingTheCeilingLastFrame { get ; set; }
		/// did the character just become grounded ?
		public bool JustGotGrounded { get ; set;  }
        /// is the character being resized to fit in tight spaces?
        public bool ColliderResized { get; set; }

        /// <summary>
        /// Reset all collision states to false
        /// </summary>
        public virtual void Reset()
		{
			IsCollidingLeft = false;
			IsCollidingRight = false;
			IsCollidingAbove = false;
            DistanceToLeftCollider = -1;
            DistanceToRightCollider = -1;
			SlopeAngleOK = false;
			JustGotGrounded = false;
			IsFalling = true;
            IsJumping = false;
			LateralSlopeAngle = 0;

        }
		
		/// <summary>
		/// Serializes the collision states
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current collision states.</returns>
		public override string ToString ()
		{
			return string.Format("(controller: collidingRight:{0} collidingLeft:{1} collidingAbove:{2} collidingBelow:{3} lateralSlopeAngle:{4} belowSlopeAngle:{5} isGrounded: {6}",
			IsCollidingRight,
			IsCollidingLeft,
			IsCollidingAbove,
			IsCollidingBelow,
			LateralSlopeAngle,
            BelowSlopeAngle,
            IsGrounded);
		}	
	}
}