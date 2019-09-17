using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Use this class on any platform or surface a Character could usually wallcling to.
	/// You'll be able to override the slow factor (close to 0 : very slow fall, 1 : normal fall, larger than 1 : faster fall than normal).
	/// </summary>
	[AddComponentMenu("Corgi Engine/Environment/Wallclinging Override")]
	public class WallClingingOverride : MonoBehaviour 
	{
		[Information("Use this component on any platform or surface a Character could usually wallcling to. Here you can override the slow factor (close to 0 : very slow fall, 1 : normal fall, larger than 1 : faster fall than normal), and decide if wallclinging is possible or not.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// if this is set to false, a Character won't be able to wallcling to this object
		public bool CanWallClingToThis = true;
		/// the slow factor to consider when wallclinging to this object
		[Range(0,2)]
		public float WallClingingSlowFactor=1f;
	}
}