using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a body of water. It will handle splash effects on entering/exiting, and allow the player to jump out of it.
	/// </summary>
	[AddComponentMenu("Corgi Engine/Environment/Water")]
	public class Water : MonoBehaviour 
	{
	    //storage
        protected CharacterSwim _characterSwim;
        
	    /// <summary>
	    /// Triggered when something collides with the water
	    /// </summary>
	    /// <param name="collider">Something colliding with the water.</param>
	    protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
            // we check that the object colliding with the water is actually a corgi controller and a character
          
            _characterSwim = collider.gameObject.GetComponentNoAlloc<CharacterSwim>();
            if (_characterSwim != null)
            {
                _characterSwim.EnterWater();
            }
		}

	    /// <summary>
	    /// Triggered when something exits the water
	    /// </summary>
	    /// <param name="collider">Something colliding with the water.</param>
	    protected virtual void OnTriggerExit2D(Collider2D collider)
		{
            // we check that the object colliding with the water is actually a corgi controller and a character
            _characterSwim = collider.gameObject.GetComponentNoAlloc<CharacterSwim>();
            if (_characterSwim != null)
            {
                _characterSwim.ExitWater();
            }
		}
	}
}