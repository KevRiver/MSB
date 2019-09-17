using UnityEngine;
using System.Collections;

namespace MoreMountains.Tools
{	
	public class MMLayers  
	{
		public static bool LayerInLayerMask(int layer, LayerMask layerMask)
		{
            //Debug.LogWarning("MMLayer func LayerInLayerMask Conditions : " + (1 << layer) + " , " + layerMask);
			if(((1 << layer) & layerMask) != 0)	
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
	}
}