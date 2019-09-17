using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// This component allows the definition of a level that can then be accessed and loaded. Used mostly in the level map scene.
	/// </summary>
	public class LevelSelector : MonoBehaviour
	{
		/// the exact name of the target level
	    public string LevelName;

		/// <summary>
		/// Loads the level specified in the inspector
		/// </summary>
	    public virtual void GoToLevel()
	    {
	        LevelManager.Instance.GotoLevel(LevelName);
	    }

		/// <summary>
		/// Restarts the current level
		/// </summary>
	    public virtual void RestartLevel()
		{
			// we trigger an unPause event for the GameManager (and potentially other classes)
			CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);
			LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
	    }
		
	}
}