using UnityEngine;
using System.Collections;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// Add this class to a collectible to have the player change weapon when collecting it
	/// </summary>
	[AddComponentMenu("Corgi Engine/Items/Pickable Weapon")]
	public class PickableWeapon : PickableItem
	{
		/// the new weapon the player gets when collecting this object
		public Weapon WeaponToGive;

		/// <summary>
		/// What happens when the weapon gets picked
		/// </summary>
		protected override void Pick()
		{
			CharacterHandleWeapon characterShoot = _collider.GetComponent<CharacterHandleWeapon>();
			characterShoot.ChangeWeapon(WeaponToGive, null);
			if (characterShoot != null)
			{
				if (characterShoot.CanPickupWeapons)
				{
				}
			}	
		}

		/// <summary>
		/// Checks if the object is pickable.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		protected override bool CheckIfPickable()
		{
			_character = _collider.GetComponent<Character>();

			// if what's colliding with the coin ain't a characterBehavior, we do nothing and exit
			if ((_character == null) || (_collider.GetComponent<CharacterHandleWeapon>() == null))
			{
				return false;
			}
			if (_character.CharacterType != Character.CharacterTypes.Player)
			{
				return false;
			}
			return true;
		}
	}
}