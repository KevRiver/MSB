using UnityEngine;
using System.Collections;

namespace MoreMountains.CorgiEngine
{	
	[AddComponentMenu("Corgi Engine/Environment/Character Hztal Mvt Override")]
	/// <summary>
	/// Add this component to a trigger zone, and it'll override the CharacterHorizontalMovement settings for all characters that cross it 
	/// </summary>
	public class CharacterHorizontalMovementOverride : MonoBehaviour 
	{
		/// basic movement speed
		public float MovementSpeed = 8f;

		protected float _previousMovementSpeed;

		/// <summary>
	    /// Triggered when something collides with the override zone
	    /// </summary>
		/// <param name="collider">Something colliding with the override zone.</param>
	    protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			// we check that the object colliding with the override zone is actually a characterJump
			CharacterHorizontalMovement characterHorizontalMovement = collider.GetComponent<CharacterHorizontalMovement>();
			if (characterHorizontalMovement==null)
			{
				return;	
			}	
			_previousMovementSpeed = characterHorizontalMovement.MovementSpeed ;	
			characterHorizontalMovement.MovementSpeed = MovementSpeed;	
		}

	    /// <summary>
	    /// Triggered when something exits the water
	    /// </summary>
	    /// <param name="collider">Something colliding with the water.</param>
	    protected virtual void OnTriggerExit2D(Collider2D collider)
		{
			// we check that the object colliding with the water is actually a characterJump
			CharacterHorizontalMovement characterHorizontalMovement = collider.GetComponent<CharacterHorizontalMovement>();
			if (characterHorizontalMovement==null)
			{
				return;	
			}
			characterHorizontalMovement.MovementSpeed = _previousMovementSpeed;	
		}
		
	}
}

