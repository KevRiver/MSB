using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// A class used to go from one level to the next while specifying an entry point in the target level. 
	/// Entry points are defined in each level's LevelManager component. They're simply Transforms in a list. 
	/// The index in the list is the identifier for the entry point. 
	/// </summary>
	public class GoToLevelEntryPoint : FinishLevel 
	{
		[Space(10)]
		[Header("Points of Entry")]
		public int PointOfEntryIndex;	
		public Character.FacingDirections FacingDirection;

		/// <summary>
		/// Loads the next level and stores the target entry point index in the game manager
		/// </summary>
		public override void GoToNextLevel()
		{
			GameManager.Instance.StorePointsOfEntry (LevelName, PointOfEntryIndex, FacingDirection);
			base.GoToNextLevel ();
		}
	}
}
