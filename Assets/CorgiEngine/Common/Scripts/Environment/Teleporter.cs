using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.MMInterface;

namespace MoreMountains.CorgiEngine
{	
	[AddComponentMenu("Corgi Engine/Environment/Teleporter")]
	/// <summary>
	/// Add this script to a trigger collider2D to teleport objects from that object to its destination
	/// </summary>
	public class Teleporter : ButtonActivated 
	{
		[Header("Teleporter")]
		/// the teleporter's destination
		public Teleporter Destination;
		/// if true, this won't teleport non player characters
		public bool OnlyAffectsPlayer=true;
		/// a gameobject to instantiate when teleporting
		public GameObject TeleportEffect;

		[Header("Teleporter Camera")]
		/// if this is true, the camera will teleport instantly to the teleporter's destination when activated
		public bool TeleportCamera = false;
		/// if this is true, a fade to black will occur when teleporting
		public bool FadeToBlack = false;
		/// the duration (in seconds) of the fade to black
		public float FadeDuration;

		protected Character _player;
	    protected List<Transform> _ignoreList;

	    /// <summary>
	    /// On start we initialize our ignore list
	    /// </summary>
	    protected virtual void Start()
		{		
			_ignoreList = new List<Transform>();
		}

	    /// <summary>
	    /// Triggered when something enters the teleporter
	    /// </summary>
	    /// <param name="collider">Collider.</param>
	    protected override void OnTriggerEnter2D(Collider2D collider)
		{
			// if the object that collides with the teleporter is on its ignore list, we do nothing and exit.
			if (_ignoreList.Contains(collider.transform))
			{
				return;
			}			

			if (collider.GetComponent<Character>()!=null)
			{
				_player = collider.GetComponent<Character>();
			}

			// if the teleporter is supposed to only affect the player (well, corgiControllers), we do nothing and exit
			if (OnlyAffectsPlayer || !AutoActivation)
			{
				base.OnTriggerEnter2D(collider);
			}
			else
			{
				Teleport(collider);
			}
		}

		/// <summary>
		/// If we're button activated and if the button is pressed, we teleport
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			if (_player.GetComponent<Collider2D>()!=null)
			{
				base.TriggerButtonAction ();
				Teleport(_player.GetComponent<Collider2D>());
			}
			ActivateZone ();
		}

		/// <summary>
		/// Teleports whatever enters the portal to a new destination
		/// </summary>
		protected virtual void Teleport(Collider2D collider)
		{
			// if the teleporter has a destination, we move the colliding object to that destination
			if (Destination!=null)
			{
				collider.transform.position=Destination.transform.position;
				_ignoreList.Remove(collider.transform);
				Destination.AddToIgnoreList(collider.transform);
				
				// we trigger splashs at both portals locations
				Splash ();
				Destination.Splash();

				StartCoroutine (TeleportEnd ());
			}
		}

		protected virtual IEnumerator TeleportEnd()
		{
			if (FadeToBlack)
			{
				if (TeleportCamera)
				{
					LevelManager.Instance.LevelCameraController.FollowsPlayer = false;
				}
				MMFadeInEvent.Trigger(FadeDuration);
				yield return new WaitForSeconds (FadeDuration);
				if (TeleportCamera)
				{
					LevelManager.Instance.LevelCameraController.TeleportCameraToTarget ();
					LevelManager.Instance.LevelCameraController.FollowsPlayer = true;
					
				}
				MMFadeOutEvent.Trigger(FadeDuration);
			}
			else
			{
				if (TeleportCamera)
				{
					LevelManager.Instance.LevelCameraController.TeleportCameraToTarget ();
				}	
			}
		}

	    /// <summary>
	    /// When something exits the teleporter, if it's on the ignore list, we remove it from it, so it'll be considered next time it enters.
	    /// </summary>
	    /// <param name="collider">Collider.</param>
	    protected override void OnTriggerExit2D(Collider2D collider)
		{
			if (_ignoreList.Contains(collider.transform))
			{
				_ignoreList.Remove(collider.transform);
			}
			base.OnTriggerExit2D(collider);
		}
		
		/// <summary>
		/// Adds an object to the ignore list, which will prevent that object to be moved by the teleporter while it's in that list
		/// </summary>
		/// <param name="objectToIgnore">Object to ignore.</param>
		public virtual void AddToIgnoreList(Transform objectToIgnore)
		{
			_ignoreList.Add(objectToIgnore);
		}

	    /// <summary>
	    /// Creates a splash at the point of entry
	    /// </summary>
	    protected virtual void Splash()
		{			
			if (TeleportEffect != null)
			{
				Instantiate(TeleportEffect ,transform.position,Quaternion.identity);	
			}
		}
	}
}