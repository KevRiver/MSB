using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	[AddComponentMenu("Corgi Engine/Weapons/Bomb")] 
	/// <summary>
	/// A basic melee weapon class, that will activate a "hurt zone" when the weapon is used
	/// </summary>
	public class Bomb : MonoBehaviour 
	{
		public enum DamageAreaShapes { Rectangle, Circle }

		[Header("Explosion")]
		public float TimeBeforeExplosion = 2f;
		public GameObject ExplosionEffect;
		public AudioClip ExplosionSfx;

		[Header("Flicker")]
		public bool FlickerSprite = true;
		public float TimeBeforeFlicker = 1f;

		[Header("Damage Area")]
		public Collider2D DamageAreaCollider;
		public float DamageAreaActiveDuration = 1f;

		protected float _timeSinceStart;
		protected Renderer _renderer;
		protected MMPoolableObject _poolableObject;

		protected bool _flickering;
		protected bool _damageAreaActive;

		protected Color _initialColor;
		protected Color _flickerColor = new Color32(255, 20, 20, 255); 

		protected virtual void OnEnable()
		{
			Initialization ();
		}

		protected virtual void Initialization()
		{
			if (DamageAreaCollider == null)
			{
				Debug.LogWarning ("There's no damage area associated to this bomb : " + this.name + ". You should set one via its inspector.");
				return;
			}
			DamageAreaCollider.isTrigger = true;
			DisableDamageArea ();

			_renderer = gameObject.GetComponentNoAlloc<Renderer> ();
			if (_renderer != null)
			{
				if (_renderer.material.HasProperty("_Color"))
				{
					_initialColor = _renderer.material.color;
				}
			}

			_poolableObject = gameObject.GetComponentNoAlloc<MMPoolableObject> ();
			if (_poolableObject != null)
			{
				_poolableObject.LifeTime = 0;
			}

			_timeSinceStart = 0;
			_flickering = false;
			_damageAreaActive = false;
		}

		protected virtual void Update()
		{
			_timeSinceStart += Time.deltaTime;
			// flickering
			if (_timeSinceStart >= TimeBeforeFlicker)
			{
				if (!_flickering && FlickerSprite)
				{
					// We make the bomb's sprite flicker
					if (_renderer != null)
					{
						StartCoroutine(MMImage.Flicker(_renderer,_initialColor,_flickerColor,0.05f,(TimeBeforeExplosion - TimeBeforeFlicker)));	
					}
				}
			}

			// activate damage area
			if (_timeSinceStart >= TimeBeforeExplosion && !_damageAreaActive)
			{
				EnableDamageArea ();
				_renderer.enabled = false;
				InstantiateExplosionEffect ();
				PlayExplosionSound ();
				_damageAreaActive = true;
			}

			if (_timeSinceStart >= TimeBeforeExplosion + DamageAreaActiveDuration)
			{
				Destroy ();
			}
		}

		protected virtual void Destroy()
		{
			_renderer.enabled = true;
			_renderer.material.color = _initialColor;
			if (_poolableObject != null)
			{
				_poolableObject.Destroy ();	
			}
			else
			{
				Destroy ();
			}

		}

		protected virtual void InstantiateExplosionEffect()
		{
			// instantiates the destroy effect
			if (ExplosionEffect!=null)
			{
				GameObject instantiatedEffect=(GameObject)Instantiate(ExplosionEffect,transform.position,transform.rotation);
				instantiatedEffect.transform.localScale = transform.localScale;
			}
		}

		protected virtual void PlayExplosionSound()
		{
			if (ExplosionSfx!=null)
			{
				SoundManager.Instance.PlaySound(ExplosionSfx,transform.position);
			}
		}

		/// <summary>
		/// Enables the damage area.
		/// </summary>
		protected virtual void EnableDamageArea()
		{
			DamageAreaCollider.enabled = true;
		}

		/// <summary>
		/// Disables the damage area.
		/// </summary>
		protected virtual void DisableDamageArea()
		{
			DamageAreaCollider.enabled = false;
		}
	}
}