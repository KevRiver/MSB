using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a collider 2D and you'll be able to have it perform an action when 
	/// a character equipped with the specified key enters it.
	/// </summary>
	[AddComponentMenu("Corgi Engine/Environment/Key Operated Zone")]
	public class KeyOperatedZone : ButtonActivated 
	{
		[Header("Key")]
		/// whether this zone actually requires a key
		public bool RequiresKey = true;
		/// the key ID, that will be checked against the existence (or not) of a key of the same name in the player's inventory
		public string KeyID;
		/// the method that should be triggered when the key is used
		public UnityEvent KeyAction;

		protected Collider2D _collidingObject;
		protected List<int> _keyList;

		/// <summary>
		/// On Start we initialize our object
		/// </summary>
		protected virtual void Start()
		{
			_keyList = new List<int> ();
		}

		/// <summary>
		/// On enter we store our colliding object
		/// </summary>
		/// <param name="collider">Something colliding with the water.</param>
		protected override void OnTriggerEnter2D(Collider2D collider)
		{
			_collidingObject = collider;
			base.OnTriggerEnter2D (collider);
		}	

		/// <summary>
		/// When the button is pressed, we check if we have a key in our inventory
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}

			if (_collidingObject == null) { return; }

			if (RequiresKey)
			{
				CharacterInventory characterInventory = _collidingObject.gameObject.GetComponentNoAlloc<CharacterInventory> ();
				if (characterInventory == null)
				{
					return;
				}	

				_keyList.Clear ();
				_keyList = characterInventory.MainInventory.InventoryContains (KeyID);
				if (_keyList.Count == 0)
				{
                    if (_buttonPromptAnimator != null)
                    {
                        _buttonPromptAnimator.SetTrigger("Error");
                    }
                    return;
				}
				else
				{
					base.TriggerButtonAction ();
					characterInventory.MainInventory.UseItem(KeyID);
				}
			}
			TriggerKeyAction ();
			ActivateZone ();
		}

		/// <summary>
		/// Calls the method associated to the key action
		/// </summary>
		protected virtual void TriggerKeyAction()
		{
			if (KeyAction != null)
			{
				KeyAction.Invoke ();
			}
		}
	}
}
