using UnityEngine;
using System.Collections;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to an area (water for example) and it will pass its parameters to any character that gets into it.
	/// </summary>
	[AddComponentMenu("Corgi Engine/Environment/Corgi Controller Override")]
	public class CorgiControllerPhysicsVolume2D : MonoBehaviour 
	{
		/// the new parameters meant to override the olds ones
		public CorgiControllerParameters ControllerParameters;
		[Header("Force Modification on Entry")]
		/// if this is true, all forces currently applied to this controller will be reset on entry
		public bool ResetForcesOnEntry = false;
		/// if this is true, forces will be multiplied by the values specified below
		public bool MultiplyForcesOnEntry = false;
		/// the force by which to multiply the current forces
		public Vector2 ForceMultiplierOnEntry = Vector2.zero;
	}
}