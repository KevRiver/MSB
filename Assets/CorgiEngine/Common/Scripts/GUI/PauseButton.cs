using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// A simple component meant to be added to the pause button
	/// </summary>
	public class PauseButton : MonoBehaviour
	{
		/// Puts the game on pause
	    public virtual void PauseButtonAction()
	    {
			// we trigger a Pause event for the GameManager and other classes that could be listening to it too
			CorgiEngineEvent.Trigger(CorgiEngineEventTypes.Pause);
	    }	
	}
}