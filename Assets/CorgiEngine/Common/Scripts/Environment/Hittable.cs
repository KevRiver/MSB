using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A class you can put on objects to trigger events when they get hit (whether that hit applied damage or not). 
    /// Useful for switches and the likes.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class Hittable : MonoBehaviour
    {
        /// an event that will get triggered when taking damage
        public UnityEvent ActionOnHit;
        /// an event that will get triggered when taking a hit but no damage
        public UnityEvent ActionOnHitZero;

        protected Health _health;

        /// <summary>
        /// On Start we grab our Health component
        /// </summary>
        protected virtual void Start()
        {
            _health = this.gameObject.GetComponent<Health>();
        }

        /// <summary>
        /// When we get hit with actual damage, we trigger the ActionOnHit UnityEvent
        /// </summary>
        protected virtual void OnHit()
        {
            ActionOnHit.Invoke();
        }

        /// <summary>
        /// When we get hit with zero damage, we trigger the ActionOnHitZero UnityEvent
        /// </summary>
        protected virtual void OnHitZero()
        {
            ActionOnHitZero.Invoke();
        }

        /// <summary>
		/// On enable, we grab our health and register to our hit delegates
		/// </summary>
		protected virtual void OnEnable()
        {
            if (_health == null)
            {
                _health = GetComponent<Health>();
            }

            if (_health != null)
            {
                _health.OnHit += OnHit;
                _health.OnHitZero += OnHitZero;
            }
        }

        /// <summary>
        /// On disable, we unbind our hit delegates
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_health != null)
            {
                _health.OnHit -= OnHit;
                _health.OnHitZero -= OnHitZero;
            }
        }
    }
}