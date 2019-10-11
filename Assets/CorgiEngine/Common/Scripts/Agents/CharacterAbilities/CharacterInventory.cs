using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a character and it'll be able to control an inventory
    /// Animator parameters : none
    /// Note that its start feedback will play on weapon change
    /// </summary>

    [HiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Inventory")] 
	public class CharacterInventory : CharacterAbility, MMEventListener<MMInventoryEvent>, MMEventListener<CorgiEngineEvent>
    {
        /// the name of the main inventory
		public string MainInventoryName;
        /// the name of the weapon inventory
		public string WeaponInventoryName;
        /// the name of the hotbar inventory
		public string HotbarInventoryName;
        /// the reference to the main inventory
		public Inventory MainInventory { get; set; }
        /// the reference to the weapon inventory
		public Inventory WeaponInventory { get; set; }
        /// the reference to the hotbar inventory
		public Inventory HotbarInventory { get; set; }
        /// if this is true, when switching to this character, if there's a main weapon equipped, it'll be equipped
        public bool AutoEquipWeaponOnCharacterSwitch;

		protected List<int> _availableWeapons;
		protected List<string> _availableWeaponsIDs;
		protected CharacterHandleWeapon _characterHandleWeapon;
		protected string _nextWeaponID;
        protected bool _nextFrameWeapon = false;
        protected string _nextFrameWeaponName;

        /// <summary>
        /// On init, we trigger our setup
        /// </summary>
		protected override void Initialization () 
		{
			base.Initialization();
			Setup ();
		}

        /// <summary>
        /// On process ability, we equip our next weapon if needed
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            if (_nextFrameWeapon)
            {
                EquipWeapon(_nextFrameWeaponName);
                _nextFrameWeapon = false;
            }
        }

        /// <summary>
        /// Setup grabs inventories, component, and fills the weapon lists
        /// </summary>
        protected virtual void Setup()
		{
			GrabInventories ();
			_characterHandleWeapon = GetComponent<CharacterHandleWeapon> ();
			FillAvailableWeaponsLists ();
		}

        /// <summary>
        /// Grabs references to all inventories
        /// </summary>
		protected virtual void GrabInventories()
		{
			if (MainInventory == null)
			{
				GameObject mainInventoryTmp = GameObject.Find (MainInventoryName);
				if (mainInventoryTmp != null) { MainInventory = mainInventoryTmp.GetComponent<Inventory> (); }	
			}
			if (WeaponInventory == null)
			{
				GameObject weaponInventoryTmp = GameObject.Find (WeaponInventoryName);
				if (weaponInventoryTmp != null) { WeaponInventory = weaponInventoryTmp.GetComponent<Inventory> (); }	
			}
			if (HotbarInventory == null)
			{
				GameObject hotbarInventoryTmp = GameObject.Find (HotbarInventoryName);
				if (hotbarInventoryTmp != null) { HotbarInventory = hotbarInventoryTmp.GetComponent<Inventory> (); }	
			}
			if (MainInventory != null) { MainInventory.SetOwner (this.gameObject); MainInventory.TargetTransform = this.transform;}
			if (WeaponInventory != null) { WeaponInventory.SetOwner (this.gameObject); WeaponInventory.TargetTransform = this.transform;}
			if (HotbarInventory != null) { HotbarInventory.SetOwner (this.gameObject); HotbarInventory.TargetTransform = this.transform;}
		}

        /// <summary>
        /// We watch for a switch weapon input
        /// </summary>
		protected override void HandleInput()
		{
			if (_inputManager.SwitchWeaponButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				SwitchWeapon ();
			}
		}

        /// <summary>
        /// Fills a list with all available weapons in the inventories
        /// </summary>
		protected virtual void FillAvailableWeaponsLists()
		{
			_availableWeaponsIDs = new List<string> ();
			if ((_characterHandleWeapon == null) || (WeaponInventory == null))
			{
				return;
			}
			_availableWeapons = MainInventory.InventoryContains (ItemClasses.Weapon);
			foreach (int index in _availableWeapons)
			{
				_availableWeaponsIDs.Add (MainInventory.Content [index].ItemID);
			}
			if (!InventoryItem.IsNull(WeaponInventory.Content[0]))
			{
				_availableWeaponsIDs.Add (WeaponInventory.Content [0].ItemID);
			}

			_availableWeaponsIDs.Sort ();
		}

        /// <summary>
        /// Determines the name of the next weapon
        /// </summary>
		protected virtual void DetermineNextWeaponName ()
		{
			if (InventoryItem.IsNull(WeaponInventory.Content[0]))
			{
				_nextWeaponID = _availableWeaponsIDs [0];
				return;
			}

			for (int i = 0; i < _availableWeaponsIDs.Count; i++)
			{
				if (_availableWeaponsIDs[i] == WeaponInventory.Content[0].ItemID)
				{
					if (i == _availableWeaponsIDs.Count - 1)
					{
						_nextWeaponID = _availableWeaponsIDs [0];
					}
					else
					{
						_nextWeaponID = _availableWeaponsIDs [i+1];
					}
				}
			}
		}

        /// <summary>
        /// Equips a weapon specified in parameters
        /// </summary>
        /// <param name="weaponID"></param>
		protected virtual void EquipWeapon(string weaponID)
		{
			for (int i = 0; i < MainInventory.Content.Length ; i++)
			{
				if (InventoryItem.IsNull(MainInventory.Content[i]))
				{
					continue;
				}
				if (MainInventory.Content[i].ItemID == weaponID)
				{
                    MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, MainInventory.name, MainInventory.Content[i], 0, i);
				}
			}
		}

        /// <summary>
        /// Switches to the next weapon in line
        /// </summary>
		protected virtual void SwitchWeapon()
		{
			// if there's no character handle weapon component, we can't switch weapon, we do nothing and exit
			if ((_characterHandleWeapon == null) || (WeaponInventory == null))
			{
				return;
			}

			FillAvailableWeaponsLists ();

			// if we only have 0 or 1 weapon, there's nothing to switch, we do nothing and exit
			if (_availableWeaponsIDs.Count <= 0)
			{
				return;
			}

			DetermineNextWeaponName ();
			EquipWeapon (_nextWeaponID);
            PlayAbilityStartFeedbacks();
		}

		/// <summary>
		/// Watches for InventoryLoaded events
		/// When an inventory gets loaded, if it's our WeaponInventory, we check if there's already a weapon equipped, and if yes, we equip it
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryLoaded)
			{
				if (inventoryEvent.TargetInventoryName == WeaponInventoryName)
				{
					this.Setup ();
					if (WeaponInventory != null)
					{
						if (!InventoryItem.IsNull (WeaponInventory.Content [0]))
						{
							_characterHandleWeapon.Setup ();
							WeaponInventory.Content [0].Equip ();
						}
					}
				}
            }
            if (inventoryEvent.InventoryEventType == MMInventoryEventType.Pick)
            {
                if (inventoryEvent.EventItem.GetType() == typeof(InventoryEngineWeapon))
                {
                    InventoryEngineWeapon inventoryWeapon = (InventoryEngineWeapon)inventoryEvent.EventItem;
                    switch (inventoryWeapon.AutoEquipMode)
                    {
                        case InventoryEngineWeapon.AutoEquipModes.NoAutoEquip:
                            // we do nothing
                            break; 

                        case InventoryEngineWeapon.AutoEquipModes.AutoEquip:
                            _nextFrameWeapon = true;
                            _nextFrameWeaponName = inventoryEvent.EventItem.ItemID;                            
                            break;

                        case InventoryEngineWeapon.AutoEquipModes.AutoEquipIfEmptyHanded:
                            if (_characterHandleWeapon.CurrentWeapon == null)
                            {
                                _nextFrameWeapon = true;
                                _nextFrameWeaponName = inventoryEvent.EventItem.ItemID;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// When we detect a character switch, we equip the current weapon if AutoEquipWeaponOnCharacterSwitch is true
        /// </summary>
        /// <param name="corgiEngineEvent"></param>
        public virtual void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
        {
            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.CharacterSwitch)
            {
                if (!AutoEquipWeaponOnCharacterSwitch)
                {
                    return;
                }
                this.Setup();
                if (WeaponInventory != null)
                {
                    if (!InventoryItem.IsNull(WeaponInventory.Content[0]))
                    {
                        _characterHandleWeapon.Setup();
                        WeaponInventory.Content[0].Equip();
                    }
                }
            }
        }

        /// <summary>
        /// On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        protected override void OnEnable()
		{
			this.MMEventStartListening<MMInventoryEvent>();
            this.MMEventStartListening<CorgiEngineEvent>();
        }

		/// <summary>
		/// On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable ();
			this.MMEventStopListening<MMInventoryEvent>();
            this.MMEventStopListening<CorgiEngineEvent>();
        }
	}
}
