using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// Add this to an item to make it modify time when it gets picked up by a Character
	/// </summary>
	[AddComponentMenu("Corgi Engine/Items/Time Modifier")]
	public class TimeModifier : MonoBehaviour
	{
		/// the effect to instantiate when picked up
		public GameObject Effect;
		/// the time speed to apply while the effect lasts
		public float TimeSpeed = 0.5f;
		/// how long the duration will last , in seconds
		public float Duration = 1.0f;

		protected WaitForSeconds _changeTimeWFS;

	    /// <summary>
	    /// Triggered when something collides with the TimeModifier
	    /// </summary>
	    /// <param name="collider">The object that collide with the TimeModifier</param>
	    protected virtual void OnTriggerEnter2D (Collider2D collider) 
		{
			// if the other collider isn't a CharacterBehavior, we exit and do nothing
			if (collider.GetComponent<Character>() == null)
				return;

			_changeTimeWFS = new WaitForSeconds (Duration * TimeSpeed);

			// we start the ChangeTime coroutine
			StartCoroutine (ChangeTime ());

			// adds an instance of the effect at the TimeModifier's position
			Instantiate(Effect,transform.position,transform.rotation);
			// we disable the sprite and the collider
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			gameObject.GetComponent<CircleCollider2D> ().enabled = false;
		}

	    /// <summary>
	    /// Asks the Game Manager to change the time scale for a specified duration.
	    /// </summary>
	    /// <returns>The time.</returns>
	    protected virtual IEnumerator ChangeTime()
		{
			// we send a new time scale event for the GameManager to catch (and other classes that may listen to it too)
			MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeSpeed, Duration, false, 0f, false);
			GUIManager.Instance.SetTimeSplash (true);
			// we multiply the duration by the timespeed to get the real duration in seconds
			yield return _changeTimeWFS;
			GUIManager.Instance.SetTimeSplash (false);
			// we re enable the sprite and collider, and desactivate the object
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			gameObject.GetComponent<CircleCollider2D> ().enabled = true;
			gameObject.SetActive(false);
		}
	}
}