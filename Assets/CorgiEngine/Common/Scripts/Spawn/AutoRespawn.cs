using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this script to an object and it will automatically be reactivated and revived when the player respawns.
	/// </summary>
	[AddComponentMenu("Corgi Engine/Spawn/Auto Respawn")]
	public class AutoRespawn : MonoBehaviour, Respawnable 
	{
		[Header("Respawn when the player respawns")]
		/// if this is true, this object will respawn at its last position when the player revives
		public bool RespawnOnPlayerRespawn = true;
		/// if this is true, this object will be repositioned at its initial position when the player revives
		public bool RepositionToInitOnPlayerRespawn = false;

		[Header("Auto respawn after X seconds")]
		/// if this has a value superior to 0, this object will respawn at its last position X seconds after its death
		public float AutoRespawnDuration = 0f;
        /// the MMFeedbacks to play when the player respawns
        public MMFeedbacks RespawnFeedback;

        // respawn
        public delegate void OnReviveDelegate();
		public OnReviveDelegate OnRevive;

		protected MonoBehaviour[] _otherComponents;
		protected Collider2D _collider2D;
		protected Renderer _renderer;
        protected Health _health;

		protected bool _reviving = false;
		protected float _timeOfDeath = 0f;
		protected bool _firstRespawn = true;
		protected Vector3 _initialPosition;

		/// <summary>
		/// On Start we grab our various components
		/// </summary>
		protected virtual void Start()
		{
			_otherComponents = GetComponents<MonoBehaviour>() ;
			_collider2D = GetComponent<Collider2D> ();
			_renderer = GetComponent<Renderer> ();
            _health = GetComponent<Health>();
            _initialPosition = this.transform.position;
		}

		/// <summary>
	    /// When the player respawns, we reinstate this agent.
	    /// </summary>
	    /// <param name="checkpoint">Checkpoint.</param>
	    /// <param name="player">Player.</param>
		public virtual void OnPlayerRespawn (CheckPoint checkpoint, Character player)
		{
			if (RepositionToInitOnPlayerRespawn)
			{
				this.transform.position = _initialPosition;				
			}

			if (RespawnOnPlayerRespawn)
			{
				if (GetComponent<Health>() != null)
				{
					GetComponent<Health>().Revive();
				}
				Revive ();
			}
		}

		/// <summary>
		/// On Update we check whether we should be reviving this agent
		/// </summary>
		protected virtual void Update()
		{
			if (_reviving)
			{
				if (_timeOfDeath + AutoRespawnDuration < Time.time)
				{
					Revive ();
					_reviving = false;
				}
			}
		}

		/// <summary>
		/// Kills this object, turning its parts off based on the settings set in the inspector
		/// </summary>
		public virtual void Kill()
		{
			if (AutoRespawnDuration <= 0f)
			{
				// object is turned inactive to be able to reinstate it at respawn
				gameObject.SetActive(false);
			}
			else
			{
				foreach (MonoBehaviour component in _otherComponents)
				{
					if (component != this)
					{
						component.enabled = false;	
					}
				}
				if (_collider2D != null) { _collider2D.enabled = false;	}
				if (_renderer != null)	{ _renderer.enabled = false; }
				_reviving = true;
				_timeOfDeath = Time.time;
			}
		}

		/// <summary>
		/// Revives this object, turning its parts back on again
		/// </summary>
		public virtual void Revive()
		{
            if (_health != null)
            {
                _health.Revive();
            }

			if (AutoRespawnDuration <= 0f)
			{
				// object is turned inactive to be able to reinstate it at respawn
				gameObject.SetActive(true);
			}
			else
			{
				foreach (MonoBehaviour component in _otherComponents)
				{
					component.enabled = true;
				}
				if (_collider2D != null) { _collider2D.enabled = true;	}
				if (_renderer != null)	{ _renderer.enabled = true; }

                RespawnFeedback?.PlayFeedbacks();
            }
			if (OnRevive != null)
			{
				OnRevive ();
			}
		}
	}
}