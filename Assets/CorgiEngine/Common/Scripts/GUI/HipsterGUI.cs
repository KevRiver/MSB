using UnityEngine;
using System.Collections;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// A simple class that disables the avatar on start
	/// </summary>
	public class HipsterGUI : MonoBehaviour 
	{
		// Use this for initialization
		void Start ()
	    {
	        GUIManager.Instance.SetAvatarActive(false);
	    }
	}
}