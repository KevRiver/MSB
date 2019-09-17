using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// A class that combines a progress bar and a text display
	/// and that can be used to display the current ammo level of a weapon
	/// </summary>
	public class AmmoDisplay : MMProgressBar 
	{
		/// the Text object used to display the current ammo numbers
		public Text TextDisplay;

		/// <summary>
		/// Updates the text display with the parameter string
		/// </summary>
		/// <param name="newText">New text.</param>
		public virtual void UpdateTextDisplay(string newText)
		{
			if (TextDisplay != null)
			{
				TextDisplay.text = newText;
			}
		}

		/// <summary>
		/// Updates the ammo display's text and progress bar
		/// </summary>
		/// <param name="magazineBased">If set to <c>true</c> magazine based.</param>
		/// <param name="totalAmmo">Total ammo.</param>
		/// <param name="maxAmmo">Max ammo.</param>
		/// <param name="ammoInMagazine">Ammo in magazine.</param>
		/// <param name="magazineSize">Magazine size.</param>
		/// <param name="displayTotal">If set to <c>true</c> display total.</param>
		public virtual void UpdateAmmoDisplays(bool magazineBased, int totalAmmo, int maxAmmo, int ammoInMagazine, int magazineSize, bool displayTotal)
		{
			if (magazineBased)
			{
				this.UpdateBar(ammoInMagazine,0,magazineSize);	
				if (displayTotal)
				{
					this.UpdateTextDisplay (ammoInMagazine + "/" + magazineSize + " - " + (totalAmmo - ammoInMagazine));					
				}
				else
				{
					this.UpdateTextDisplay (ammoInMagazine + "/" + magazineSize);
				}
			}
			else
			{
				this.UpdateBar(totalAmmo,0,maxAmmo);	
				this.UpdateTextDisplay (totalAmmo + "/" + maxAmmo);
			}
		}
	}
}
