using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MSBNetwork;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to an object and it will cause damage to objects that collide with it. 
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Damage/DamageOnTouch")] 
	public class DamageOnTouch : MonoBehaviour 
	{
		/// the possible ways to add knockback : noKnockback, which won't do nothing, set force, or add force
		public enum KnockbackStyles { NoKnockback, SetForce, AddForce }
        /// the possible knockback directions
        public enum KnockbackDirections { BasedOnOwnerPosition, BasedOnSpeed }
        //MSB Custom CC Style
        public enum MSBCCStyles { NoCC, Stun, Slow, KnockBack }

        public enum DamageAreaType { Trigger, DestroyOnColliding };

		[Header("Targets")]
		[Information("This component will make your object cause damage to objects that collide with it. Here you can define what layers will be affected by the damage (for a standard enemy, choose Player), how much damage to give, and how much force should be applied to the object that gets the damage on hit. You can also specify how long the post-hit invincibility should last (in seconds).",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		// the layers that will be damaged by this object
		public LayerMask TargetLayerMask;

		[Header("Damage Caused")]
		/// The amount of health to remove from the player's health
		public int DamageCaused = 10;
        /// the type of knockback to apply when causing damage
        //public KnockbackStyles DamageCausedKnockbackType = KnockbackStyles.SetForce;
        /// The direction to apply the knockback 
        //public KnockbackDirections DamageCausedKnockbackDirection;

        //MSB Custom
        public CharacterStates.CharacterConditions CCActivateCondition;
        public MSBCCStyles CausedMSBCCType = MSBCCStyles.NoCC;
        public DamageAreaType _damageAreaType;

        [Header("CCType 'Slow' ")]
        public float MovementMultiplier = 1.0f;
        public float SlowDuration;

        [Header("CCType 'Stun'")]
        public float StunDuration;

        [Header("CCType 'KnowBack'")]
        public Vector2 CausedKnockbackForce = Vector2.zero;
        public float shortStunDuration;
        public Vector2 DamageCausedKnockbackForce = new Vector2(10,2);
        /// The duration of the invincibility frames after the hit (in seconds)
        public float InvincibilityDuration = 0.5f;

		[Header("Damage Taken")]
		[Information("After having applied the damage to whatever it collided with, you can have this object hurt itself. A bullet will explode after hitting a wall for example. Here you can define how much damage it'll take every time it hits something, or only when hitting something that's damageable, or non damageable. Note that this object will need a Health component too for this to be useful.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// The amount of damage taken every time, whether what we collide with is damageable or not
		public int DamageTakenEveryTime = 0;
		/// The amount of damage taken when colliding with a damageable object
		public int DamageTakenDamageable = 0;
		/// The amount of damage taken when colliding with something that is not damageable
		public int DamageTakenNonDamageable = 0;
		/// the type of knockback to apply when taking damage
		public KnockbackStyles DamageTakenKnockbackType = KnockbackStyles.NoKnockback;
        /// The direction to apply the knockback 
        public KnockbackDirections DamagedTakenKnockbackDirection;
        /// The force to apply to the object that gets damaged
        public Vector2 DamageTakenKnockbackForce = Vector2.zero;
        /// The duration of the invincibility frames after the hit (in seconds)
        public float DamageTakenInvincibilityDuration = 0.5f;

        [Header("Feedback")]
        /// the duration of freeze frames on hit (leave it at 0 to ignore)
        public float FreezeFramesOnHitDuration = 0f;

        [ReadOnly]
        /// the owner of the DamageOnTouch zone
        public GameObject Owner;

        // storage		
        protected Vector2 _lastPosition, _velocity, _knockbackForce;
		protected float _startTime = 0f;
		protected Health _colliderHealth;
		protected CorgiController _corgiController;
		protected CorgiController _colliderCorgiController;              
		protected Health _health;
		protected List<GameObject> _ignoredGameObjects;
        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;
        protected CircleCollider2D _circleCollider2D;
        protected BoxCollider2D _boxCollider2D;
        public bool isBelongToLocalUser;
        public bool collideOnce;

        /// <summary>
        /// Initialization
        /// </summary>
        protected virtual void Awake()
		{
			_ignoredGameObjects = new List<GameObject>();
			_health = GetComponent<Health>();
			_corgiController = GetComponent<CorgiController> ();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _gizmosColor = Color.red;
            _gizmosColor.a = 0.25f;
        }

		/// <summary>
		/// OnEnable we set the start time to the current timestamp
		/// </summary>
		protected virtual void OnEnable()
		{
			_startTime = Time.time;
        }

		/// <summary>
		/// During last update, we store the position and velocity of the object
		/// </summary>
		protected virtual void Update () 
		{
			ComputeVelocity();
		}

		/// <summary>
		/// Adds the gameobject set in parameters to the ignore list
		/// </summary>
		/// <param name="newIgnoredGameObject">New ignored game object.</param>
		public virtual void IgnoreGameObject(GameObject newIgnoredGameObject)
		{
			_ignoredGameObjects.Add(newIgnoredGameObject);
		}

		/// <summary>
		/// Removes the object set in parameters from the ignore list
		/// </summary>
		/// <param name="ignoredGameObject">Ignored game object.</param>
		public virtual void StopIgnoringObject(GameObject ignoredGameObject)
		{
			_ignoredGameObjects.Remove(ignoredGameObject);
		}

		/// <summary>
		/// Clears the ignore list.
		/// </summary>
		public virtual void ClearIgnoreList()
		{
			_ignoredGameObjects.Clear();
		}

		/// <summary>
		/// Computes the velocity based on the object's last position
		/// </summary>
		protected virtual void ComputeVelocity()
		{
			_velocity = (_lastPosition - (Vector2)transform.position) /Time.deltaTime;
			_lastPosition = transform.position;
		}
		
		/// <summary>
		/// When a collision with the player is triggered, we give damage to the player and knock it back
		/// </summary>
		/// <param name="collider">what's colliding with the object.</param>
		public virtual void OnTriggerStay2D(Collider2D collider)
		{			
			//Colliding (collider);
	    }

		public virtual void OnTriggerEnter2D(Collider2D collider)
		{            
			Colliding (collider);           
		}

        const int LAYER_DEFAULT = 0;
        const int LAYER_PLATFORM = 8;
		protected virtual void Colliding(Collider2D collider)
		{
            //Debug.LogWarning("Colliding at " + collider.gameObject.name);

            if (!this.isActiveAndEnabled)
			{
                //Debug.LogWarning("DamageOnTouch : func Colliding 1st Condition Enter");
				return;
			}

			// if the object we're colliding with is part of our ignore list, we do nothing and exit
			if (_ignoredGameObjects.Contains(collider.gameObject))
			{
                //Debug.LogWarning("DamageOnTouch : func Colliding 2nd Condition Enter " + collider.gameObject.name);
                return;
			}

			// if what we're colliding with isn't part of the target layers, we do nothing and exit
			if (!MMLayers.LayerInLayerMask(collider.gameObject.layer,TargetLayerMask))
			{
                //Debug.LogWarning("Collider Layer : " + collider.gameObject.layer);
                switch (collider.gameObject.layer)
                {
                    case LAYER_PLATFORM:
                        if (_damageAreaType == DamageAreaType.DestroyOnColliding)
                        {
                            gameObject.SetActive(false);
                        }
                        break;
                }
                return;
			}
			
			_colliderHealth = collider.gameObject.GetComponentNoAlloc<Health>();

			// if what we're colliding with is damageable
			if (_colliderHealth != null)
			{
			    if(_colliderHealth.CurrentHealth > 0)
			    {                    
                    OnCollideWithDamageable(_colliderHealth);
                    if (collideOnce)
                    {
                        if (_damageAreaType == DamageAreaType.DestroyOnColliding)
                            gameObject.SetActive(false);
                        if (_boxCollider2D != null)
                        {
                            if(_damageAreaType == DamageAreaType.Trigger)
                                _boxCollider2D.enabled = false;                           
                        }
                        if (_circleCollider2D != null)
                        {
                            if (_damageAreaType == DamageAreaType.Trigger)
                                    _circleCollider2D.enabled = false;
                        }
                    }
                }
			} 

			// if what we're colliding with can't be damaged
			else
			{                
				OnCollideWithNonDamageable();
			}
		}

	    /// <summary>
	    /// Describes what happens when colliding with a damageable object
	    /// </summary>
	    /// <param name="health">Health.</param>
        /// MSB Custom
	    protected virtual void OnCollideWithDamageable(Health health)
	    {
            
            MSB_Character colliderPlayer = health.GetComponent<MSB_Character>();
            string option = ((int)CausedMSBCCType).ToString() +
                "," + ((int)CCActivateCondition).ToString();

            switch (CausedMSBCCType)
            {
                case MSBCCStyles.NoCC:
                    break;
                case MSBCCStyles.KnockBack:
                    _colliderCorgiController = health.gameObject.GetComponentNoAlloc<CorgiController>();

                    if (Owner == null) { Owner = this.gameObject; }
                    Vector2 relativePosition = _colliderCorgiController.transform.position - Owner.transform.position;

                    _knockbackForce.x = CausedKnockbackForce.x;
                    _knockbackForce.x *= Mathf.Sign(relativePosition.x);
                    _knockbackForce.y = CausedKnockbackForce.y;

                    option += "," + shortStunDuration.ToString() +
                        "," + (_knockbackForce.x).ToString() +
                        "," + (_knockbackForce.y).ToString();
                    break;
                case MSBCCStyles.Slow:
                    option += "," + SlowDuration.ToString() +
                        "," + MovementMultiplier.ToString();
                    break;
                case MSBCCStyles.Stun:
                    option += "," + StunDuration.ToString();
                    break;
            }


            //상대가 무적상태가 아니고, DamageOnTouch를 생성한 Owner가 LocalUser일때 ActionDamage를 요청한다
            if (!_colliderHealth.Invulnerable)
            {
                Debug.Log("Colliding Box is belong to local user ? : " + isBelongToLocalUser);
                if (isBelongToLocalUser)
                {
                    Debug.LogWarning("Request Action Damage : " + option);
                    NetworkModule.GetInstance().RequestGameUserActionDamage(MSB_GameManager.Instance.roomIndex, colliderPlayer.c_userData.userNumber, DamageCaused, option);
                }

                //_colliderHealth.Damage(DamageCaused);
            }
	    }

		/// <summary>
	    /// Describes what happens when colliding with a non damageable object
	    /// </summary>
	    protected virtual void OnCollideWithNonDamageable()
	    {
            Debug.LogWarning("NonDamageable Collider");
            if (_damageAreaType == DamageAreaType.DestroyOnColliding)
            {
                gameObject.SetActive(false);
            }

			SelfDamage(DamageTakenEveryTime + DamageTakenNonDamageable);
	    }

	    /// <summary>
	    /// Applies damage to itself
	    /// </summary>
	    /// <param name="damage">Damage.</param>
	    protected virtual void SelfDamage(int damage)
	    {
	    	if (_health != null)
	    	{
				_health.Damage(damage,gameObject,0f,DamageTakenInvincibilityDuration);
	    	}	

			// we apply knockback to ourself
			if (_corgiController != null)
			{
				Vector2 totalVelocity=_colliderCorgiController.Speed + _velocity;
				Vector2 knockbackForce = new Vector2(
					-1 * Mathf.Sign(totalVelocity.x) * DamageTakenKnockbackForce.x,
					-1 * Mathf.Sign(totalVelocity.y) * DamageTakenKnockbackForce.y	);	

				if (DamageTakenKnockbackType == KnockbackStyles.SetForce)
				{
					_corgiController.SetForce(knockbackForce);	
				}
				if (DamageTakenKnockbackType == KnockbackStyles.AddForce)
				{
					_corgiController.AddForce(knockbackForce);	
				}
			}
	    }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = _gizmosColor;

            if ((_boxCollider2D != null) && _boxCollider2D.enabled)
            {
                _gizmoSize.x =  _boxCollider2D.bounds.size.x ;
                _gizmoSize.y =  _boxCollider2D.bounds.size.y ;
                _gizmoSize.z = 1f;
                Gizmos.DrawCube(_boxCollider2D.bounds.center, _gizmoSize);
            }
            if (_circleCollider2D != null && _circleCollider2D.enabled)
            {
                Gizmos.DrawSphere((Vector2)this.transform.position + _circleCollider2D.offset, _circleCollider2D.radius);                
            }
        }
	}
}