using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this class to an object and it'll trigger the specified actions on pick
    /// </summary>
    [AddComponentMenu("TopDown Engine/Items/Pickable Action")]
    public class PickableAction : PickableItem
    {
        /// the action(s) to trigger when picked
        public UnityEvent PickEvent;

        /// <summary>
        /// Triggered when something collides with the object
        /// </summary>
        /// <param name="collider">Other.</param>
        protected override void Pick()
        {
            base.Pick();
            if (PickEvent != null)
            {
                PickEvent.Invoke();
            }
        }
    }
}