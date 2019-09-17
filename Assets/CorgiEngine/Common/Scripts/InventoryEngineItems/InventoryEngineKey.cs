using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{	
	[CreateAssetMenu(fileName = "InventoryEngineKey", menuName = "MoreMountains/CorgiEngine/InventoryEngineKey", order = 1)]
	[Serializable]
	/// <summary>
	/// Pickable key item
	/// </summary>
	public class InventoryEngineKey : InventoryItem 
	{
		/// <summary>
		/// When the item is used, we try to grab our character's Health component, and if it exists, we add our health bonus amount of health
		/// </summary>
		public override bool Use()
		{
			base.Use();

            return true;
		}
	}
}