using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{	
	[RequireComponent(typeof(Weapon))]
	public class WeaponAmmo : MonoBehaviour, MMEventListener<MMStateChangeEvent<MoreMountains.CorgiEngine.Weapon.WeaponStates>>, MMEventListener<MMInventoryEvent>
	{
		[Header("Ammo")]
		public string AmmoID;
		/// the name of the inventory where the system should look for ammo
		public string AmmoInventoryName = "MainInventory";
		/// the theoretical maximum of ammo
		public int MaxAmmo = 100;
		/// if this is true, everytime you equip this weapon, it'll auto fill with ammo
		public bool ShouldLoadOnStart = true;

		[ReadOnly]
		/// the current amount of ammo available in the inventory
		public int CurrentAmmoAvailable;

		public Inventory AmmoInventory { get; set; }

		protected Weapon _weapon;

		protected virtual void Start()
		{
			// we grab the ammo inventory if it exists
			GameObject ammoInventoryTmp = GameObject.Find (AmmoInventoryName);
			if (ammoInventoryTmp != null) { AmmoInventory = ammoInventoryTmp.GetComponent<Inventory> (); }

			_weapon = GetComponent<Weapon> ();
			LoadOnStart ();
		}

		protected virtual void LoadOnStart()
		{
			FillWeaponWithAmmo ();
		}

		protected virtual void RefreshCurrentAmmoAvailable()
		{
			CurrentAmmoAvailable = AmmoInventory.GetQuantity (AmmoID);
		}

		public bool EnoughAmmoToFire()
		{
			if (AmmoInventory == null)
			{
				Debug.LogWarning (this.name + " couldn't find the associated inventory. Is there one present in the scene? It should be named '" + AmmoInventoryName + "'.");
				return false;
			}

			RefreshCurrentAmmoAvailable ();

			if (_weapon.MagazineBased)
			{
				if (_weapon.CurrentAmmoLoaded >= _weapon.AmmoConsumedPerShot)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (CurrentAmmoAvailable >= _weapon.AmmoConsumedPerShot)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		protected virtual void ConsumeAmmo()
		{
			if (_weapon.MagazineBased)
			{
				_weapon.CurrentAmmoLoaded = _weapon.CurrentAmmoLoaded - _weapon.AmmoConsumedPerShot;
			}


			for (int i = 0; i < _weapon.AmmoConsumedPerShot; i++)
			{
				AmmoInventory.UseItem (AmmoID);	
				CurrentAmmoAvailable--;
			}	

			if (CurrentAmmoAvailable  < _weapon.AmmoConsumedPerShot)
			{
				if (_weapon.AutoDestroyWhenEmpty)
				{
					StartCoroutine (_weapon.WeaponDestruction ());
				}
			}
		}

		public virtual void FillWeaponWithAmmo()
		{
			if (AmmoInventory != null)
			{
				RefreshCurrentAmmoAvailable ();
			}

			if (_weapon.MagazineBased)
			{
				int counter = 0;
				int stock = CurrentAmmoAvailable;
				for (int i = _weapon.CurrentAmmoLoaded; i < _weapon.MagazineSize; i++)
				{
					if (stock > 0)
					{
						stock--;
						counter++;				
					}									
				}
				_weapon.CurrentAmmoLoaded += counter;
			}
		}

		public virtual void OnMMEvent(MMStateChangeEvent<MoreMountains.CorgiEngine.Weapon.WeaponStates> weaponEvent)
		{
			// if this event doesn't concern us, we do nothing and exit
			if (weaponEvent.Target != this.gameObject)
			{
				return;
			}

			switch (weaponEvent.NewState)
			{
				case MoreMountains.CorgiEngine.Weapon.WeaponStates.WeaponUse:
					ConsumeAmmo ();
					break;

				case MoreMountains.CorgiEngine.Weapon.WeaponStates.WeaponReloadStop:
					FillWeaponWithAmmo();
					break;
			}
		}

		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			switch (inventoryEvent.InventoryEventType)
			{
				case MMInventoryEventType.Pick:
					if (inventoryEvent.EventItem.ItemClass == ItemClasses.Ammo)
					{
						RefreshCurrentAmmoAvailable ();
					}
					break;				
			}
		}


		/// <summary>
		/// On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMStateChangeEvent<MoreMountains.CorgiEngine.Weapon.WeaponStates>>();
			this.MMEventStartListening<MMInventoryEvent> ();
		}

		/// <summary>
		/// On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMStateChangeEvent<MoreMountains.CorgiEngine.Weapon.WeaponStates>>();
			this.MMEventStopListening<MMInventoryEvent> ();
		}
	}
}