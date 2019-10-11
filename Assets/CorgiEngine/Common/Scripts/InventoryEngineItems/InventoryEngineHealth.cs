using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{	
	[CreateAssetMenu(fileName = "InventoryEngineHealth", menuName = "MoreMountains/CorgiEngine/InventoryEngineHealth", order = 1)]
	[Serializable]
	/// <summary>
	/// Pickable health item
	/// </summary>
	public class InventoryEngineHealth : InventoryItem 
	{
		[Header("Health")]
		[Information("Here you need specify the amount of health gained when using this item.",InformationAttribute.InformationType.Info,false)]
		/// the amount of health to add to the player when the item is used
		public int HealthBonus;

		/// <summary>
		/// When the item is used, we try to grab our character's Health component, and if it exists, we add our health bonus amount of health
		/// </summary>
		public override bool Use()
		{
			base.Use();

			if (TargetInventory.Owner == null)
			{
				return false;
			}

			Health characterHealth = TargetInventory.Owner.GetComponent<Health>();
			if (characterHealth != null)
			{
				characterHealth.GetHealth(HealthBonus,TargetInventory.gameObject);	
			}

            return true;
		}

	}
}