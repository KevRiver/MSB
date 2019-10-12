using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// An event used to let characters know that they may have connected with a ledge
    /// </summary>
    public struct LedgeEvent
    {
        public Collider2D CharacterCollider;
        public Ledge LedgeGrabbed;

        public LedgeEvent(Collider2D characterCollider, Ledge ledgeGrabbed)
        {
            CharacterCollider = characterCollider;
            LedgeGrabbed = ledgeGrabbed;
        }

        static LedgeEvent e;
        public static void Trigger(Collider2D characterCollider, Ledge ledgeGrabbed)
        {
            e.CharacterCollider = characterCollider;
            e.LedgeGrabbed = ledgeGrabbed;
            MMEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// Add this component to an object and it'll be able to be grabbed by characters equipped with a CharacterLedgeHang ability
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Ledge : MonoBehaviour
    {
        /// the direction ledges can be grabbed from (left means it can be grabbed by a character facing left)
        public enum LedgeGrabDirections { Left, Right }
        /// the direction this ledge gets grabbed from
        public LedgeGrabDirections LedgeGrabDirection = LedgeGrabDirections.Left;
        /// the offset to apply when hanging to this ledge
        public Vector3 HangOffset;
        /// the offset to apply when climb is complete 
        public Vector3 ClimbOffset;

        /// the tag used to reference the player
        protected const string _playerTag = "Player";

        /// <summary>
        /// When something collides with our ledge collider, we trigger a LedgeEvent
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == _playerTag)
            {
                LedgeEvent.Trigger(collider, this);
            }
        }	
        
        /// <summary>
        /// Draws a blue point at the HangOffset position, and an orange one at the point the Character will get teleported to at the end of the climb
        /// Now you're thinking with portals.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position + HangOffset, 0.1f);
            Gizmos.color = MMColors.Orange;
            Gizmos.DrawWireSphere(this.transform.position + ClimbOffset, 0.1f);
        }
	}
}
