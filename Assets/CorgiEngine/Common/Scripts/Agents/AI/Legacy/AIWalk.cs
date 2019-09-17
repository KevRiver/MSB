using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a CorgiController2D and it will walk, turn back when it hits a wall, and try and avoid holes if you ask it to.
	/// </summary>
	[RequireComponent(typeof(CharacterHorizontalMovement))]
	[AddComponentMenu("Corgi Engine/Character/AI/AI Walk")] 
	public class AIWalk : MonoBehaviour
	{
		/// The agent's possible walk behaviours : patrol will have it walk in random directions until a wall or hole is hit, MoveOnSight will make the agent move only when "seeing" a target
		public enum WalkBehaviours { Patrol, MoveOnSight }

		[Information("Add this component to a Character and it will walk, turn back when it hits a wall, and try and avoid holes if you ask it to.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// The agent's walk behaviour
		public WalkBehaviours WalkBehaviour = WalkBehaviours.Patrol;

		[Header("Obstacle Detection")]
		[Information("Decide whether your character should change direction when hitting a wall, and if it should try to avoid holes.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// If set to true, the agent will change direction when hitting a wall
		public bool ChangeDirectionOnWall = true;
	    /// If set to true, the agent will try and avoid falling
	    public bool AvoidFalling = false;
	    /// The offset the hole detection should take into account
	    public Vector3 HoleDetectionOffset = new Vector3(0, 0, 0);
		/// the length of the ray cast to detect holes
	    public float HoleDetectionRaycastLength=1f;

		[Header("Move on Sight")]
		[Information("If your AI Walk's behaviour is set to Move On Sight, you can define here its 'view' distance (the length of the raycasts used to detect targets), the distance at which it should stop from the target, an offset for the raycast's origin, and the target layer.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// The maximum distance at which the AI can see the target
		public float ViewDistance = 10f;
		/// the horizontal distance from its target at which the agent will stop moving. Between that distance and the walk distance, the agent will slow down progressively
		public float StopDistance = 1f;
		/// The offset to apply to the raycast origin point (by default the position of the object)
		public Vector3 MoveOnSightRayOffset = new Vector3(0,0,0);
		/// the layer mask to use to detect targets
		public LayerMask MoveOnSightLayer;
		/// the layer mask of the sight obstacles (usually platforms)
		public LayerMask MoveOnSightObstaclesLayer;

	    // private stuff
		protected CorgiController _controller;
	    protected Character _character;
		protected Health _health;
		protected CharacterHorizontalMovement _characterHorizontalMovement;
	    protected Vector2 _direction;
	    protected Vector2 _startPosition;
	    protected Vector2 _initialDirection;
		protected Vector3 _initialScale;
	    protected float _distanceToTarget;

	    /// <summary>
	    /// Initialization
	    /// </summary>
	    protected virtual void Start()
	    {
			Initialization ();
	    }

		protected virtual void Initialization()
		{
			// we get the CorgiController2D component
			_controller = GetComponent<CorgiController>();
			_character = GetComponent<Character>();
			_characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
			_health = GetComponent<Health> ();
			// initialize the start position
			_startPosition = transform.position;
			// initialize the direction
			_direction = _character.IsFacingRight ? Vector2.right : Vector2.left;

			_initialDirection = _direction;
			_initialScale = transform.localScale;
		}
		
		/// <summary>
		/// Every frame, moves the agent and checks if it can shoot at the player.
		/// </summary>
		protected virtual void Update () 
		{
			if (_character == null)
			{
				return;
			}
			if ((_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
				|| (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
			{
				return;
			}
	        // moves the agent in its current direction
			CheckForTarget();
	        CheckForWalls();
			CheckForHoles();
			_characterHorizontalMovement.SetHorizontalMove(_direction.x);	        
	    }

	    /// <summary>
	    /// Checks for a wall and changes direction if it meets one
	    /// </summary>
	    protected virtual void CheckForWalls()
	    {
			if (!ChangeDirectionOnWall)
			{
				return;
			}

	        // if the agent is colliding with something, make it turn around
	        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
	        {
	            ChangeDirection();
	        }
	    }

	    /// <summary>
	    /// Checks for holes 
	    /// </summary>
	    protected virtual void CheckForHoles()
	    {
	    	// if we're not grounded or if we're not supposed to check for holes, we do nothing and exit
			if (!AvoidFalling || !_controller.State.IsGrounded)
	        {
	            return;
	        }

	        // we send a raycast at the extremity of the character in the direction it's facing, and modified by the offset you can set in the inspector.
	        Vector2 raycastOrigin = new Vector2(transform.position.x+_direction.x*(HoleDetectionOffset.x+Mathf.Abs(GetComponent<BoxCollider2D>().bounds.size.x)/2), transform.position.y+ HoleDetectionOffset.y - (transform.localScale.y / 2));
			RaycastHit2D raycast = MMDebug.RayCast(raycastOrigin, -transform.up, HoleDetectionRaycastLength, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask,  Color.gray, true);
	        // if the raycast doesn't hit anything
	        if (!raycast)
			{
	            // we change direction
	            ChangeDirection();
	        }
	    }

	    /// <summary>
	    /// Casts a ray (if needed) to see if a target is in sight. If yes, moves towards it.
	    /// </summary>
	    protected virtual void CheckForTarget()
	    {
	    	if (WalkBehaviour != WalkBehaviours.MoveOnSight)
	    	{
	    		return;
	    	}

			bool hit=false;

	        _distanceToTarget = 0;
			// we cast a ray to the left of the agent to check for a Player
			Vector2 raycastOrigin = transform.position + MoveOnSightRayOffset;

			// we cast it to the left	
			RaycastHit2D raycast = MMDebug.RayCast(raycastOrigin,Vector2.left,ViewDistance,MoveOnSightLayer,Color.yellow,true);
			// if we see a player
			if (raycast)
			{
				hit=true;
				// we change direction
	            _direction = Vector2.left;
	            _distanceToTarget= raycast.distance;
			}
			
			// we cast a ray to the right of the agent to check for a Player	
			raycast = MMDebug.RayCast(raycastOrigin,Vector2.right,ViewDistance,MoveOnSightLayer,Color.yellow,true);
			if (raycast)
			{
				hit=true;
	            _direction = Vector2.right;
	            _distanceToTarget = raycast.distance;
			}			

			// if the ray has hit nothing, or if we've reached our target, we prevent our character from moving further.
			if ((!hit) || (_distanceToTarget <= StopDistance)) 
			{
				_direction = Vector2.zero;
			} 	            	
			else
			{
				// if we've hit something, we make sure there's no obstacle between us and our target
				RaycastHit2D raycastObstacle = MMDebug.RayCast(raycastOrigin,_direction,ViewDistance,MoveOnSightObstaclesLayer,Color.gray,true);
				if (raycastObstacle && _distanceToTarget > raycastObstacle.distance)
				{
					_direction = Vector2.zero;
				}
			}
	    }

	    /// <summary>
	    /// Changes the agent's direction and flips its transform
	    /// </summary>
	    protected virtual void ChangeDirection()
	    {
	        _direction = -_direction;
	    }

		protected virtual void OnRevive()
		{
			_direction = _character.IsFacingRight ? Vector2.right : Vector2.left;
			transform.localScale= _initialScale;
			transform.position=_startPosition;
		}

	    /// <summary>
	    /// When the player respawns, we reinstate this agent.
	    /// </summary>
	    /// <param name="checkpoint">Checkpoint.</param>
	    /// <param name="player">Player.</param>
		protected virtual void OnEnable ()
		{
			if (_health != null)
			{
				_health.OnRevive += OnRevive;
			}
		}

		protected virtual void OnDisable()
		{
			if (_health != null)
			{
				_health.OnRevive -= OnRevive;
			}			
		}
	}
}