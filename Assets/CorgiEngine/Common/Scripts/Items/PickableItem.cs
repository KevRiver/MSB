using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// An event typically fired when picking an item, letting listeners know what item has been picked
	/// </summary>
	public struct PickableItemEvent
	{
		public PickableItem PickedItem;
		/// <summary>
		/// Initializes a new instance of the <see cref="MoreMountains.CorgiEngine.PickableItemEvent"/> struct.
		/// </summary>
		/// <param name="pickedItem">Picked item.</param>
		public PickableItemEvent(PickableItem pickedItem)
		{
			PickedItem = pickedItem;
		}
        
        static PickableItemEvent e;
        public static void Trigger(PickableItem pickedItem)
        {
            e.PickedItem = pickedItem;
            MMEventManager.TriggerEvent(e);
        }
    }

	/// <summary>
	/// Coin manager
	/// </summary>
	public class PickableItem : MonoBehaviour
    {
        [Header("Pickable Item")]
        /// the MMFeedback to play when the item gets picked
        public MMFeedbacks PickFeedbacks;
		/// if this is set to true, the object will be disabled when picked
		public bool DisableObjectOnPick = true;
        /// if this is true, disable the object's renderer on pick
        public bool DisableRendererOnPick = false;
        /// if this is true, disable the object's collider on pick
        public bool DisableColliderOnPick = false;

        protected Collider2D _collider;
        protected Collider2D _pickingCollider;
        protected Renderer _renderer;
		protected Character _character = null;
		protected bool _pickable = false;
		protected ItemPicker _itemPicker = null;

		protected virtual void Start()
		{
			_itemPicker = this.gameObject.GetComponent<ItemPicker> ();
            _renderer = this.gameObject.GetComponent<Renderer>();
            _collider = this.gameObject.GetComponent<Collider2D>();

        }

		/// <summary>
		/// Triggered when something collides with the coin
		/// </summary>
		/// <param name="collider">Other.</param>
		public virtual void OnTriggerEnter2D (Collider2D collider) 
		{
			_pickingCollider = collider;
			PickItem ();
		}

		/// <summary>
		/// Check if the item is pickable and if yes, proceeds with triggering the effects and disabling the object
		/// </summary>
		public virtual void PickItem()
		{
			if (CheckIfPickable ())
			{
				Effects ();
				PickableItemEvent.Trigger(this);
				Pick ();
				if (DisableObjectOnPick)
				{
					// we desactivate the gameobject
					gameObject.SetActive (false);	
				}
                else
                {
                    if (DisableColliderOnPick)
                    {
                        _collider.enabled = false;
                    }
                    if (DisableRendererOnPick && (_renderer != null))
                    {
                        _renderer.enabled = false;
                    }
                }
			} 
		}

		/// <summary>
		/// Checks if the object is pickable.
		/// </summary>
		/// <returns><c>true</c>, if if pickable was checked, <c>false</c> otherwise.</returns>
		protected virtual bool CheckIfPickable()
		{
			// if what's colliding with the coin ain't a characterBehavior, we do nothing and exit
			_character = _pickingCollider.GetComponent<Character>();
			if (_character == null)
			{
				return false;
			}
			if (_character.CharacterType != Character.CharacterTypes.Player)
			{
				return false;
			}
			if (_itemPicker != null)
			{
				if  (!_itemPicker.Pickable())
				{
					return false;	
				}
			}

			return true;
		}

		/// <summary>
		/// Triggers the various pick effects
		/// </summary>
		protected virtual void Effects()
		{
            PickFeedbacks?.PlayFeedbacks();

            /*
            if (PickSfx!=null) 
			{	
				SoundManager.Instance.PlaySound(PickSfx,transform.position);	
			}

			if (Effect != null)
			{
				// adds an instance of the effect at the coin's position
				Instantiate(Effect,transform.position,transform.rotation);				
			}*/
		}

		/// <summary>
		/// Override this to describe what happens when the object gets picked
		/// </summary>
		protected virtual void Pick()
		{
			
		}
	}
}