using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MSBNetwork;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// This class manages the health of an object, pilots its potential health bar, handles what happens when it takes damage,
	/// and what happens when it dies.
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Core/Health")] 
	public class Health : MonoBehaviour
	{
		/// the current health of the character
		[ReadOnly]
		public int CurrentHealth ;
		/// If this is true, this object can't take damage
		[ReadOnly]
		public bool Invulnerable = false;	

		[Header("Health")]
		[Information("Add this component to an object and it'll have health, will be able to get damaged and potentially die.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the initial amount of health of the object
	    public int InitialHealth = 10;
	    /// the maximum amount of health of the object
	    public int MaximumHealth = 10;

		[Header("Damage")]
		[Information("Here you can specify an effect and a sound FX to instantiate when the object gets damaged, and also how long the object should flicker when hit (only works for sprites).",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the effect that will be instantiated everytime the character touches the ground
		public GameObject DamageEffect;
		// the sound to play when the player gets hit
		public AudioClip DamageSfx;
		// should the sprite (if there's one) flicker when getting damage ?
		public bool FlickerSpriteOnHit = true;
        // whether or not this object can get knockback
        public bool ImmuneToKnockback = false;

		[Header("Death")]
		[Information("Here you can set an effect to instantiate when the object dies, a force to apply to it (corgi controller required), how many points to add to the game score, if the device should vibrate (only works on iOS and Android), and where the character should respawn (for non-player characters only).",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the effect to instantiate when the object gets destroyed
		public GameObject DeathEffect;
		/// if this is not true, the object will remain there after its death
		public bool DestroyOnDeath = true;
		/// the time (in seconds) before the character is destroyed or disabled
		public float DelayBeforeDestruction = 0f;
		/// if this is true, collisions will be turned off when the character dies
		public bool CollisionsOffOnDeath = true;
		/// the points the player gets when the object's health reaches zero
		public int PointsWhenDestroyed;
		/// if true, the handheld device will vibrate when the object dies
		public bool VibrateOnDeath;
		/// if this is set to false, the character will respawn at the location of its death, otherwise it'll be moved to its initial position (when the scene started)
		public bool RespawnAtInitialLocation = false;
        
        [Header("Death Forces")]
        /// the force applied when the character dies
        public Vector2 DeathForce = new Vector2(0, 10);
        /// whether or not the controller's forces should be set to 0 on death
        public bool ResetForcesOnDeath = false;

        // respawn
        public delegate void OnHitDelegate();
        public OnHitDelegate OnHit;

		public delegate void OnReviveDelegate();
		public OnReviveDelegate OnRevive;

		public delegate void OnDeathDelegate();
		public OnDeathDelegate OnDeath;

		protected Vector3 _initialPosition;
		protected Color _initialColor;
		protected Color _flickerColor = new Color32(255, 20, 20, 255); 
		protected Renderer _renderer;
		protected Character _character;
        
        [Header("MSB Custom")]
        //MSB Custom
        public MSB_Character _MSB_Character;
        public bool isLocalUser;       
        

		protected CorgiController _controller;
	    protected MMHealthBar _healthBar;
	    protected Collider2D _collider2D;
		protected bool _initialized = false;
		protected AutoRespawn _autoRespawn;
		protected Animator _animator;

        //MSB Custom
        //public DamageOnTouch.KnockbackStyles knockBackStyle;
        //public Vector2 knockBackForce;

	    /// <summary>
	    /// On Start, we initialize our health
	    /// </summary>
	    protected virtual void Start()
	    {
			Initialization();
			InitializeSpriteColor ();
	    }

	    /// <summary>
	    /// Grabs useful components, enables damage and gets the inital color
	    /// </summary>
		protected virtual void Initialization()
		{
			_character = GetComponent<Character>();
            _MSB_Character = GetComponent<MSB_Character>();
            isLocalUser = _MSB_Character.isLocalUser ? true : false;
            //knockBackStyle = DamageOnTouch.KnockbackStyles.NoKnockback;
            //knockBackForce = Vector2.zero;

			if (gameObject.GetComponentNoAlloc<SpriteRenderer>() != null)
			{
				_renderer = GetComponent<SpriteRenderer>();				
			}
			if (_character != null)
			{
				if (_character.CharacterModel != null)
				{
					if (_character.CharacterModel.GetComponentInChildren<Renderer> ()!= null)
					{
						_renderer = _character.CharacterModel.GetComponentInChildren<Renderer> ();	
					}
				}	
			}

            // we grab our animator
            if (_character != null)
            {
                if (_character.CharacterAnimator != null)
                {
                    _animator = _character.CharacterAnimator;
                }
                else
                {
                    _animator = GetComponent<Animator>();
                }
            }
            else
            {
                _animator = GetComponent<Animator>();
            }

            if (_animator != null)
            {
                _animator.logWarnings = false;
            }

            _autoRespawn = GetComponent<AutoRespawn> ();
			_controller = GetComponent<CorgiController>();
			_healthBar = GetComponent<MMHealthBar>();
			_collider2D = GetComponent<Collider2D>();

			_initialPosition = transform.position;
			_initialized = true;
			CurrentHealth = InitialHealth;
			DamageEnabled();
			UpdateHealthBar (false);
		}

		/// <summary>
		/// Stores the inital color of the Character's sprite.
		/// </summary>
		protected virtual void InitializeSpriteColor()
		{
            if (!FlickerSpriteOnHit)
            {
                return;
            }
			if (_renderer != null)
			{
				if (_renderer.material.HasProperty("_Color"))
				{
					_initialColor = _renderer.material.color;
				}
			}
		}

		/// <summary>
		/// Restores the original sprite color
		/// </summary>
		protected virtual void ResetSpriteColor()
		{
			if (_renderer != null)
			{
				if (_renderer.material.HasProperty("_Color"))
				{
					_renderer.material.color = _initialColor;
				}
			}
		}
            
        public void ChangeHealth(int _health)
        {
            int previousHealth = CurrentHealth;
            CurrentHealth = _health;

            UpdateHealthBar(true);
            if (CurrentHealth <= 0)
            {
                if (_MSB_Character != null)
                {
                    
                }
            }

        }

        public void Damage(int _damage)
        {
            //Debug.LogWarning(_MSB_Character.c_userData.userNick + " 's" + " Health.Damage Called");           
        }

        public void TakeMSBCC(DamageOnTouch.MSBCCStyles _CCStyle ,float _MovementMultiplier,float _CCDuration, CharacterStates.CharacterConditions _causedCondition)
        {
            //마찬가지로 로컬일때만 적용한다
            if (_CCStyle == DamageOnTouch.MSBCCStyles.NoCC)
            {
                return;
            }

            if (_causedCondition == CharacterStates.CharacterConditions.Stun)
            {

            }

            if (_causedCondition == CharacterStates.CharacterConditions.Slow)
            {

            }
        }


        /// <summary>
        /// Called when the object takes damage
        /// </summary>
        /// <param name="damage">The amount of health points that will get lost.</param>
        /// <param name="instigator">The object that caused the damage.</param>
        /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
        /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
        public virtual void Damage(int damage,GameObject instigator, float flickerDuration, float invincibilityDuration)
		{            
        }

		/// <summary>
		/// Kills the character, vibrates the device, instantiates death effects, handles points, etc
		/// </summary>
		public virtual void Kill()
		{
			// we make our handheld device vibrate
			if (VibrateOnDeath)
			{
				#if UNITY_ANDROID || UNITY_IPHONE
					Handheld.Vibrate();	
				#endif
			}

			// we prevent further damage
			DamageDisabled();

			// instantiates the destroy effect
			if (DeathEffect!=null)
			{
				GameObject instantiatedEffect=(GameObject)Instantiate(DeathEffect,transform.position,transform.rotation);
	            instantiatedEffect.transform.localScale = transform.localScale;
			}

			// Adds points if needed.
			if(PointsWhenDestroyed != 0)
			{
				// we send a new points event for the GameManager to catch (and other classes that may listen to it too)
				CorgiEnginePointsEvent.Trigger(PointsMethods.Add, PointsWhenDestroyed);
			}

			if (_animator != null)
			{
				//_animator.SetTrigger ("Death");
            }

            if (OnDeath != null)
            {
                OnDeath();
            }

            // if we have a controller, removes collisions, restores parameters for a potential respawn, and applies a death force
            if (_controller != null)
			{
				// we make it ignore the collisions from now on
				if (CollisionsOffOnDeath)
                {
                    _controller.CollisionsOff();	
					if (_collider2D != null)
					{
						_collider2D.enabled = false;
					}
				}

				// we reset our parameters
				_controller.ResetParameters();

                // we reset our controller's forces on death if needed
                if (ResetForcesOnDeath)
                {
                    _controller.SetForce(Vector2.zero);
                }

				// we apply our death force
				if (DeathForce != Vector2.zero)
				{
					_controller.GravityActive(true);
					_controller.SetForce(DeathForce);		
				}
			}


			// if we have a character, we want to change its state
			if (_character != null)
			{
                // we set its dead state to true
                //Debug.LogWarning("CharacterCondition Change to Dead");
				_character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);
				_character.Reset ();

				// if this is a player, we quit here
				if (_character.CharacterType == Character.CharacterTypes.Player)
				{
                    gameObject.SetActive(false);                    
                    return;
				}

			}

			if (DelayBeforeDestruction > 0f)
			{
				Invoke ("DestroyObject", DelayBeforeDestruction);
			}
			else
			{
				// finally we destroy the object
				DestroyObject();	
			}
		}

        public IEnumerator test1()
        {
            yield return new WaitForSeconds(3.0f);
        }

		/// <summary>
		/// Revive this object.
		/// </summary>
		public virtual void Revive()
		{
			if (!_initialized)
			{
				return;
			}

			if (_collider2D != null)
			{
				_collider2D.enabled = true;
			}
			if (_controller != null)
			{
				_controller.CollisionsOn();
				_controller.SetForce(Vector2.zero);
				_controller.ResetParameters();
			}
			if (_character != null)
			{
				_character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
			}

			if (RespawnAtInitialLocation)
			{
				transform.position = _initialPosition;
			}

			Initialization();
            if (FlickerSpriteOnHit)
            {
                ResetSpriteColor();
            }			
			UpdateHealthBar(false);
			if (OnRevive != null)
			{
				OnRevive ();
			}
		}

	    /// <summary>
	    /// Destroys the object, or tries to, depending on the character's settings
	    /// </summary>
	    protected virtual void DestroyObject()
		{
			if (!DestroyOnDeath)
			{
				return;
			}
			
			if (_autoRespawn == null)
			{
                // object is turned inactive to be able to reinstate it at respawn
                Debug.LogWarning("Destroy Object gameObject.SetActive false");
                gameObject.SetActive(false);	
			}
			else
			{
                Debug.LogWarning("AutoRespawn.Kill Called");
				_autoRespawn.Kill ();
			}

		}

		/// <summary>
		/// Called when the character gets health (from a stimpack for example)
		/// </summary>
		/// <param name="health">The health the character gets.</param>
		/// <param name="instigator">The thing that gives the character health.</param>
		public virtual void GetHealth(int health,GameObject instigator)
		{
			// this function adds health to the character's Health and prevents it to go above MaxHealth.
			CurrentHealth = Mathf.Min (CurrentHealth + health,MaximumHealth);
			UpdateHealthBar(true);
		}

		/// <summary>
		/// Plays a sound when the character is hit
		/// </summary>
		protected virtual void PlayHitSfx()
	    {
			if (DamageSfx!=null)
			{
				SoundManager.Instance.PlaySound(DamageSfx,transform.position);
			}
	    }

	    /// <summary>
	    /// Resets the character's health to its max value
	    /// </summary>
	    public virtual void ResetHealthToMaxHealth()
	    {
			CurrentHealth = MaximumHealth;
			UpdateHealthBar (false);
	    }	

	    /// <summary>
	    /// Updates the character's health bar progress.
	    /// </summary>
		protected virtual void UpdateHealthBar(bool show)
	    {
	    	if (_healthBar != null)
	    	{
				_healthBar.UpdateBar(CurrentHealth, 0f, MaximumHealth, show);
	    	}

	    	if (_character != null)
	    	{
	    		if (_character.CharacterType == Character.CharacterTypes.Player)
	    		{
					// We update the health bar
					if (GUIManager.Instance != null)
					{
						GUIManager.Instance.UpdateHealthBar(CurrentHealth, 0f, MaximumHealth, _character.PlayerID);
					}
	    		}
	    	}
	    }

	    /// <summary>
	    /// Prevents the character from taking any damage
	    /// </summary>
	    public virtual void DamageDisabled()
	    {
			Invulnerable = true;
	    }

	    /// <summary>
	    /// Allows the character to take damage
	    /// </summary>
	    public virtual void DamageEnabled()
	    {
	    	Invulnerable = false;
	    }

		/// <summary>
	    /// makes the character able to take damage again after the specified delay
	    /// </summary>
	    /// <returns>The layer collision.</returns>
	    public virtual IEnumerator DamageEnabled(float delay)
		{
			yield return new WaitForSeconds (delay);
			Invulnerable = false;
		}

		/// <summary>
		/// When the object is enabled (on respawn for example), we restore its initial health levels
		/// </summary>
		protected virtual void OnEnable()
		{
			CurrentHealth = InitialHealth;
			DamageEnabled();
			UpdateHealthBar (false);
		}
	}
}