using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	[RequireComponent(typeof(Collider2D))]
	/// <summary>
	/// Add this component to an object with a trigger 2D collider and it'll act as a gravity zone, modifying the gravity for all Characters entering it, providing they have the CharacterGravity ability
	/// </summary>
	[AddComponentMenu("Corgi Engine/Environment/Gravity Zone")]
	public class GravityZone : CorgiControllerPhysicsVolume2D 
	{
		[Range(0,360)]
		/// the angle of the gravity for this zone
		public float GravityDirectionAngle = 180;
		/// the vector angle of the gravity for this zone
		public Vector2 GravityDirectionVector { get { return MMMaths.RotateVector2 (Vector2.down, GravityDirectionAngle);	}}

		/// <summary>
		/// On DrawGizmos, we draw an arrow to show the zone's current gravity direction
		/// </summary>
		protected virtual void OnDrawGizmosSelected()
		{
			MMDebug.DebugDrawArrow (this.transform.position, GravityDirectionVector, Color.green, 3f, 0.2f, 35f);
		}			
	}
}
