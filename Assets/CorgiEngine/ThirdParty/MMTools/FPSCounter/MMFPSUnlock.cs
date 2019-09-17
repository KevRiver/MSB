using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MoreMountains.Tools
{
	public class MMFPSUnlock : MonoBehaviour 
	{
		public int TargetFPS;

		protected virtual void Start()
		{
			Application.targetFrameRate = TargetFPS;
		}		
	}
}
