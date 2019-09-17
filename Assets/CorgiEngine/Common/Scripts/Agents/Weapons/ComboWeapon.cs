using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to an object containing multiple weapons and it'll turn it into a ComboWeapon, allowing you to chain attacks from all the different weapons
    /// </summary>
    [AddComponentMenu("Corgi Engine/Weapons/Combo Weapon")]
    public class ComboWeapon : MonoBehaviour
    {
        /// whether or not the combo can be dropped if enough time passes between two consecutive attacks
        public bool DroppableCombo = true;
        /// the delay after which the combo drops
        public float DropComboDelay = 0.5f;

        [ReadOnly]
        /// the list of weapons, set automatically by the class
        public Weapon[] Weapons;
        [ReadOnly]
        /// the reference to the weapon's Owner
        public CharacterHandleWeapon OwnerCharacterHandleWeapon;
        [ReadOnly]
        /// the time spent since the last weapon stopped
        public float TimeSinceLastWeaponStopped;

        protected int _currentWeaponIndex = 0;
        protected bool _countdownActive = false;
        
        /// <summary>
        /// On start we initialize our Combo Weapon
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        /// Grabs all Weapon components and initializes them
        /// </summary>
        public virtual void Initialization()
        {
            Weapons = GetComponents<Weapon>();
            InitializeUnusedWeapons();
        }

        /// <summary>
        /// On Update we reset our combo if needed
        /// </summary>
        protected virtual void Update()
        {
            ResetCombo();
        }

        /// <summary>
        /// Resets the combo if enough time has passed since the last attack
        /// </summary>
        protected virtual void ResetCombo()
        {
            if (Weapons.Length > 1)
            {
                if (_countdownActive && DroppableCombo)
                {
                    TimeSinceLastWeaponStopped += Time.deltaTime;
                    if (TimeSinceLastWeaponStopped > DropComboDelay)
                    {
                        _countdownActive = false;
                        
                        _currentWeaponIndex = 0;
                        OwnerCharacterHandleWeapon.CurrentWeapon = Weapons[_currentWeaponIndex];
                        OwnerCharacterHandleWeapon.ChangeWeapon(Weapons[_currentWeaponIndex], Weapons[_currentWeaponIndex].WeaponID, true);
                    }
                }
            }
        }

        /// <summary>
        /// When one of the weapons get used we turn our countdown off
        /// </summary>
        /// <param name="weaponThatStarted"></param>
        public virtual void WeaponStarted(Weapon weaponThatStarted)
        {
            _countdownActive = false;
        }

        /// <summary>
        /// When one of the weapons has ended its attack, we start our countdown and switch to the next weapon
        /// </summary>
        /// <param name="weaponThatStopped"></param>
        public virtual void WeaponStopped(Weapon weaponThatStopped)
        {
            OwnerCharacterHandleWeapon = Weapons[_currentWeaponIndex].CharacterHandleWeapon;
            
            int newIndex = 0;
            if (OwnerCharacterHandleWeapon != null)
            {
                if (Weapons.Length > 1)
                {
                    if (_currentWeaponIndex < Weapons.Length-1)
                    {
                        newIndex = _currentWeaponIndex + 1;
                    }
                    else
                    {
                        newIndex = 0;
                    }

                    _countdownActive = true;
                    TimeSinceLastWeaponStopped = 0f;

                    _currentWeaponIndex = newIndex;
                    OwnerCharacterHandleWeapon.CurrentWeapon = Weapons[newIndex];
                    OwnerCharacterHandleWeapon.ChangeWeapon(Weapons[newIndex], Weapons[newIndex].WeaponID, true);
                }
            }
        }

        /// <summary>
        /// Flips all unused weapons so they remain properly oriented
        /// </summary>
        public virtual void FlipUnusedWeapons()
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                if (i != _currentWeaponIndex)
                {
                    Weapons[i].Flipped = !Weapons[i].Flipped;
                }                
            }
        }

        /// <summary>
        /// Initializes all unused weapons
        /// </summary>
        protected virtual void InitializeUnusedWeapons()
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                if (i != _currentWeaponIndex)
                {
                    Weapons[i].SetOwner(Weapons[_currentWeaponIndex].Owner, Weapons[_currentWeaponIndex].CharacterHandleWeapon);
                    Weapons[i].Initialization();
                }
            }
        }
    }
}
