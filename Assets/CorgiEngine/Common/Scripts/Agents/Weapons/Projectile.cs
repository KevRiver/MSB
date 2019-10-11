using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	[AddComponentMenu("Corgi Engine/Weapons/Projectile")]
	public class Projectile : MMPoolableObject 
	{
		[Header("Movement")]
		/// if true, the projectile will rotate at initialization towards its rotation
		public bool FaceDirection = true;
		/// the speed of the object (relative to the level's speed)
		public float Speed=0;
		/// the acceleration of the object over time. Starts accelerating on enable.
	    public float Acceleration = 0;
	    /// the current direction of the object
	    public Vector3 Direction = Vector3.left;
	    /// if set to true, the spawner can change the direction of the object. If not the one set in its inspector will be used.
	    public bool DirectionCanBeChangedBySpawner = true;
		/// the flip factor to apply if and when the projectile is mirrored
		public Vector3 FlipValue = new Vector3(-1,1,1);
		/// determines whether or not the projectile is facing right
		public bool ProjectileIsFacingRight = true;

		[Header("Spawn")]
		[Information("Here you can define an initial delay (in seconds) during which this object won't take or cause damage. This delay starts when the object gets enabled. You can also define whether the projectiles should damage their owner (think rockets and the likes) or not",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the initial delay during which the projectile can't be destroyed
		public float InitialInvulnerabilityDuration=0f;
		/// should the projectile damage its owner ?
		public bool DamageOwner = false;

		protected Weapon _weapon;
		protected GameObject _owner;
	    protected Vector3 _movement;
		protected float _initialSpeed;
		protected SpriteRenderer _spriteRenderer;
		protected DamageOnTouch _damageOnTouch;
		protected WaitForSeconds _initialInvulnerabilityDurationWFS;

		protected const float _raycastSkinSecurity=0.01f;
		protected BoxCollider2D _collider;
		protected Vector2 _raycastOrigin;
		protected Vector2 _raycastDestination;
		protected bool _facingRightInitially;
		protected bool _initialFlipX;
		protected Vector3 _initialLocalScale;

	    /// <summary>
		/// On awake, we store the initial speed of the object 
	    /// </summary>
	    protected virtual void Awake ()
		{
			_facingRightInitially = ProjectileIsFacingRight;
			_initialSpeed = Speed;
			_collider = GetComponent<BoxCollider2D> ();
			_spriteRenderer = GetComponent<SpriteRenderer> ();
			_damageOnTouch = GetComponent<DamageOnTouch>();
			_initialInvulnerabilityDurationWFS = new WaitForSeconds (InitialInvulnerabilityDuration);
			if (_spriteRenderer != null) {	_initialFlipX = _spriteRenderer.flipX ;		}
			_initialLocalScale = transform.localScale;		
		}

		/// <summary>
		/// On enable, we reset the object's speed
		/// </summary>
	    protected override void OnEnable()
		{
			base.OnEnable();
			Initialization();
			if (InitialInvulnerabilityDuration>0)
			{
				StartCoroutine(InitialInvulnerability());
			}
	    }

	    /// <summary>
	    /// Handles the projectile's initial invincibility
	    /// </summary>
	    /// <returns>The invulnerability.</returns>
	    protected virtual IEnumerator InitialInvulnerability()
	    {
			if (_damageOnTouch == null) { yield break; }
			if (_weapon == null) { yield break; }
            _damageOnTouch.ClearIgnoreList();
			_damageOnTouch.IgnoreGameObject(_weapon.Owner.gameObject);
	    	yield return _initialInvulnerabilityDurationWFS;
			if (DamageOwner)
			{
				_damageOnTouch.StopIgnoringObject(_weapon.Owner.gameObject);
			}
	    }

	    /// <summary>
	    /// Initializes the projectile
	    /// </summary>
	    protected virtual void Initialization()
		{
			Speed = _initialSpeed;
			ProjectileIsFacingRight = _facingRightInitially;
			if (_spriteRenderer != null) {	_spriteRenderer.flipX = _initialFlipX;	}
			transform.localScale = _initialLocalScale;	
	    }
		
		// On update(), we move the object based on the level's speed and the object's speed, and apply acceleration
		protected override void Update ()
		{
			base.Update ();
			Movement();
	    }

	    /// <summary>
	    /// Handles the projectile's movement, every frame
	    /// </summary>
	    public virtual void Movement()
	    {
			_movement = Direction * (Speed / 10) * Time.deltaTime;
			transform.Translate(_movement,Space.World);
			// We apply the acceleration to increase the speed
			Speed += Acceleration * Time.deltaTime;
		}

		/// <summary>
		/// Sets the projectile's direction.
		/// </summary>
		/// <param name="newDirection">New direction.</param>
		/// <param name="newRotation">New rotation.</param>
		/// <param name="spawnerIsFacingRight">If set to <c>true</c> spawner is facing right.</param>
		public virtual void SetDirection(Vector3 newDirection, Quaternion newRotation, bool spawnerIsFacingRight=true)
		{
			if (DirectionCanBeChangedBySpawner)
			{
				Direction = newDirection;
			}
			if (ProjectileIsFacingRight != spawnerIsFacingRight)
			{
				Flip ();
			}
			if (FaceDirection)
			{
				transform.rotation = newRotation;
			}
		}

		/// <summary>
		/// Flip the projectile
		/// </summary>
		protected virtual void Flip()
		{
			if (_spriteRenderer != null)
			{
				_spriteRenderer.flipX = !_spriteRenderer.flipX;
			}	
			else
			{
				this.transform.localScale = Vector3.Scale(this.transform.localScale,FlipValue) ;
			}
		}

		/// <summary>
		/// Sets the projectile's parent weapon.
		/// </summary>
		/// <param name="newWeapon">New weapon.</param>
		public virtual void SetWeapon(Weapon newWeapon)
		{
			_weapon = newWeapon;
		}

		/// <summary>
		/// Sets the projectile's owner.
		/// </summary>
		/// <param name="newOwner">New owner.</param>
		public virtual void SetOwner(GameObject newOwner)
		{
			_owner = newOwner;
			DamageOnTouch damageOnTouch = this.gameObject.MMGetComponentNoAlloc<DamageOnTouch>();            
			if (damageOnTouch != null)
            {
                damageOnTouch.Owner = newOwner;
                damageOnTouch.Owner = newOwner;
                if (!DamageOwner)
                {
                    damageOnTouch.IgnoreGameObject(newOwner);
                }                
			}
		}
		
	}	
}
