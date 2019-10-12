using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a camera to have it support parallax layers
	/// </summary>
	[AddComponentMenu("Corgi Engine/Camera/Parallax Camera")]
	public class ParallaxCamera : MonoBehaviour 
	{	
		[Information("If you set MoveParallax to true, the camera movement will cause parallax elements to move accordingly.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		public bool MoveParallax=true;
	}
}