using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{
	public class InventoryPickableItem : ItemPicker 
	{
		/// The effect to instantiate when the coin is hit
		public GameObject Effect;
		/// The sound effect to play when the object gets picked
		public AudioClip PickSfx;

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
				if (PickSfx!=null) 
				{	
					SoundManager.Instance.PlaySound(PickSfx,transform.position);	
				}

				if (Effect != null)
				{
					// adds an instance of the effect at the coin's position
					Instantiate(Effect,transform.position,transform.rotation);				
				}	
			}
		}
	}
}
