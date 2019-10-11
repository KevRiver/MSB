using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this class to a **TRIGGER** collider2D and it'll let you know when a character enters it
    /// and will let you trigger actions in consequence
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CharacterDetector : MonoBehaviour
    {
        /// It this is true, the character will have to be tagged Player for this to work
        public bool RequiresPlayer = true;
        [ReadOnly]
        /// if this is true, a character (and possibly a player based on the setting above) is in the area
        public bool CharacterInArea = false;
        /// a UnityEvent to fire when the targeted character enters the area
        public UnityEvent OnEnter;
        /// a UnityEvent to fire while the targeted character stays in the area
        public UnityEvent OnStay;
        /// a UnityEvent to fire when the targeted character exits the area
        public UnityEvent OnExit;

        protected Collider2D _collider2D;
        protected Character _character;

        /// <summary>
        /// On Start we grab our collider2D and set it to trigger in case we forgot AGAIN to set it to trigger
        /// </summary>
        protected virtual void Start()
        {
            _collider2D = this.gameObject.GetComponent<Collider2D>();
            if (_collider2D != null)
            {
                _collider2D.isTrigger = true;
            }            
        }        

        /// <summary>
        /// When a character enters we turn our state to true
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (!TargetFound(collider))
            {
                return;
            }

            CharacterInArea = true;

            if (OnEnter != null)
            {
                OnEnter.Invoke();
            }            
        }

        /// <summary>
        /// While a character stays we keep our boolean true
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerStay2D(Collider2D collider)
        {
            if (!TargetFound(collider))
            {
                return;
            }
            
            CharacterInArea = true;

            if (OnStay != null)
            {
                OnStay.Invoke();
            }
        }

        /// <summary>
        /// When a character exits we reset our boolean
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (!TargetFound(collider))
            {
                return;
            }
            
            CharacterInArea = false;

            if (OnExit != null)
            {
                OnExit.Invoke();
            }
        }

        /// <summary>
        /// Returns true if the collider set in parameter is the targeted type, false otherwise
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        protected virtual bool TargetFound(Collider2D collider)
        {
            _character = collider.gameObject.MMGetComponentNoAlloc<Character>();
            
            if (_character == null)
            {
                return false;
            }

            if (RequiresPlayer && (_character.CharacterType != Character.CharacterTypes.Player))
            {
                return false;
            }

            return true;
        }
    }
}
