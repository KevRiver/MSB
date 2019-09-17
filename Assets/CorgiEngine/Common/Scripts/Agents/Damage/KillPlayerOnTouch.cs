using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this to a GameObject with a Collider2D set to Trigger to have it kill the player on touch.
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Damage/Kill Player on Touch")] 
	public class KillPlayerOnTouch : MonoBehaviour 
	{
		/// <summary>
		/// When a collision is triggered, check if the thing colliding is actually the player. If yes, kill it.
		/// </summary>
		/// <param name="collider">The object that collides with the KillPlayerOnTouch object.</param>
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			Character character = collider.GetComponent<Character>();

			if (character == null)
			{
				return;
			}
			
			if (character.CharacterType != Character.CharacterTypes.Player)
			{
				return;
			}
			
			LevelManager.Instance.KillPlayer(character);
		}
	}
}