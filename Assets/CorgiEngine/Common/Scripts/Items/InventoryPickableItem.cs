using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
	public class InventoryPickableItem : ItemPicker 
	{
		[Header("Inventory Pickable Item")]
        /// the MMFeedback to play when the object gets picked
        public MMFeedbacks PickFeedbacks;

        protected override void PickSuccess()
		{
			base.PickSuccess ();
			Effects ();
		}

		/// <summary>
		/// Triggers the various pick effects
		/// </summary>
		protected virtual void Effects()
		{
			if (!Application.isPlaying)
			{
				return;
			}				
			else
			{
                PickFeedbacks?.PlayFeedbacks();
			}
		}
	}
}
