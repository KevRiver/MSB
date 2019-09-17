using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{	
	[CreateAssetMenu(fileName = "InventoryEnginePoints", menuName = "MoreMountains/CorgiEngine/InventoryEnginePoints", order = 2)]
	[Serializable]
	/// <summary>
	/// Pickable health item
	/// </summary>
	public class InventoryEnginePoints : InventoryItem 
	{
		[Header("Points")]
		[Information("Here you need to specify the amount of points that need to be gained when picking this item.",InformationAttribute.InformationType.Info,false)]
		/// The amount of points to add when collected
		public int PointsToAdd = 10;

		/// <summary>
		/// When the item is picked, we add points
		/// </summary>
		public override bool Pick()
		{
			base.Pick ();
			// we send a new points event for the GameManager to catch (and other classes that may listen to it too)
			CorgiEnginePointsEvent.Trigger(PointsMethods.Add, PointsToAdd);
            return true;
		}
	}
}
