using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a trigger and it will send your player to the next level
	/// </summary>
	[AddComponentMenu("Corgi Engine/Spawn/Finish Level")]
	public class FinishLevel : ButtonActivated 
	{
		public string LevelName;

		/// <summary>
		/// When the button is pressed we start the dialogue
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			base.TriggerButtonAction ();
			GoToNextLevel();
			ActivateZone ();
		}			

		/// <summary>
		/// Loads the next level
		/// </summary>
	    public virtual void GoToNextLevel()
	    {
	    	if (LevelManager.Instance!=null)
	    	{
				LevelManager.Instance.GotoLevel(LevelName);
	    	}
	    	else
	    	{
		        LoadingSceneManager.LoadScene(LevelName);
			}
	    }
	}
}