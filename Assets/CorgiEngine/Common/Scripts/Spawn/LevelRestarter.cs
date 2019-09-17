using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a trigger to cause the level to restart when the player hits the trigger
	/// </summary>
	[AddComponentMenu("Corgi Engine/Spawn/Level Restarter")]
	public class LevelRestarter : MonoBehaviour 
	{
		/// <summary>
		/// When a character enters the zone, restarts the level
		/// </summary>
		/// <param name="collider">Collider.</param>
	    protected virtual void OnTriggerEnter2D (Collider2D collider)
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

			LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}