using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this script to a platform and it'll fall down when walked upon by a playable character
	/// Add an AutoRespawn component to your platform and it'll get reset when your character dies
	/// </summary>
	[AddComponentMenu("Corgi Engine/Environment/Falling Platform")]
	public class FallingPlatform : MonoBehaviour 
	{
		/// the time (in seconds) before the fall of the platform
		public float TimeBeforeFall = 2f;
		/// the speed at which the platforms falls
		public float FallSpeed = 2f;
        /// the tolerance to apply when comparing the relative positions of the falling platform and 
        public float Tolerance = 0.1f;

	    // private stuff
		protected Animator _animator;
	    protected bool _shaking=false;
	    protected Vector2 _newPosition;
	    protected Bounds _bounds;
		protected Collider2D _collider2D;
		protected Vector3 _initialPosition;
		protected float _timer;
		protected float _platformTopY;
		protected AutoRespawn _autoRespawn;

	    /// <summary>
	    /// Initialization
	    /// </summary>
	    protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// Grabs components and saves initial position and timer
		/// </summary>
		protected virtual void Initialization()
		{
			// we get the animator
			_animator = GetComponent<Animator>();
			_collider2D = GetComponent<Collider2D> ();
			_autoRespawn = GetComponent<AutoRespawn> ();
			_bounds=LevelManager.Instance.LevelBounds;
			_initialPosition = this.transform.position;
			_timer = TimeBeforeFall;
		}
		
		/// <summary>
		/// This is called every frame.
		/// </summary>
		protected virtual void FixedUpdate()
		{		
			// we send our various states to the animator.		
			UpdateAnimator ();		
			
			if (_timer < 0)
			{
				_newPosition = new Vector2(0, -FallSpeed * Time.deltaTime);
				                           
				transform.Translate(_newPosition,Space.World);
				
				if (transform.position.y < _bounds.min.y)
				{
					DisableFallingPlatform ();
				}
			}
		}

		/// <summary>
		/// Disables the falling platform. We're not destroying it, so we can revive it on respawn
		/// </summary>
		protected virtual void DisableFallingPlatform()
		{
			if (_autoRespawn == null)
			{
				this.gameObject.SetActive (false);			
			}
			else
			{
				_autoRespawn.Kill ();				
			}
			this.transform.position = _initialPosition;		
			_timer = TimeBeforeFall;
			_shaking = false;
		}

		/// <summary>
		/// Updates the block's animator.
		/// </summary>
	    protected virtual void UpdateAnimator()
		{				
			if (_animator!=null)
			{
				MMAnimator.UpdateAnimatorBool(_animator,"Shaking",_shaking);	
			}
		}
		
		/// <summary>
		/// Triggered when a CorgiController touches the platform
		/// </summary>
		/// <param name="controller">The corgi controller that collides with the platform.</param>		
		public virtual void OnTriggerStay2D(Collider2D collider)
		{
			CorgiController controller=collider.GetComponent<CorgiController>();
			if (controller==null)
				return;
			
			if (TimeBeforeFall>0)
			{
				_platformTopY = (_collider2D != null) ? _collider2D.bounds.max.y : this.transform.position.y;

				if (controller.ColliderBottomPosition.y >= _platformTopY - Tolerance)
				{
					_timer -= Time.deltaTime;
					_shaking=true;
				}
			}	
			else
			{
				_shaking=false;
			}
		}
	    /// <summary>
	    /// Triggered when a CorgiController exits the platform
	    /// </summary>
	    /// <param name="controller">The corgi controller that collides with the platform.</param>
	    protected virtual void OnTriggerExit2D(Collider2D collider)
		{
			CorgiController controller=collider.GetComponent<CorgiController>();
			if (controller==null)
				return;
			
			_shaking=false;
		}

		protected virtual void OnRevive()
		{
			this.transform.position = _initialPosition;		
			_timer = TimeBeforeFall;
			_shaking = false;
		}

		/// <summary>
		/// When the player respawns, we reinstate this agent.
		/// </summary>
		/// <param name="checkpoint">Checkpoint.</param>
		/// <param name="player">Player.</param>
		protected virtual void OnEnable ()
		{
			if (gameObject.GetComponentNoAlloc<AutoRespawn>() != null)
			{
				gameObject.GetComponentNoAlloc<AutoRespawn>().OnRevive += OnRevive;
			}
		}

		protected virtual void OnDisable()
		{
			if (_autoRespawn != null)
			{
				_autoRespawn.OnRevive -= OnRevive;
			}			
		}
	}
}