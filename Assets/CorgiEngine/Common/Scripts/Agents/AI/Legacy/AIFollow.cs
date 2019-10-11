using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this script to a CharacterBehavior+CorgiController2D object to make it follow (or try to follow) Player1
	/// So far the Follower will move horizontally towards the player, and use a jetpack to reach it, or jump above obstacles.
	/// </summary>
	[RequireComponent(typeof(CharacterHorizontalMovement))]
	[AddComponentMenu("Corgi Engine/Character/AI/AI Follow")] 
	public class AIFollow : MonoBehaviour 
	{
		/// true if the agent should follow the player
		public bool AgentFollowsPlayer{get;set;}

		[Header("Distances")]
		[Information("Add this script to a Character to make it follow (or try to follow) the main Player character. If it has a CharacterRun component it'll try to run when far from target. If it has a CharacterJetpack it'll try to jetpack over obstacles to reach the target, and if it has a CharacterJump component it'll try to jump over obstacles.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]

		/// the minimum horizontal distance between the agent and the player at which the agent should start running
		public float RunDistance = 10f;
		/// the maximum horizontal distance between the agent and the player at which the agent should walk at normal speed
		public float WalkDistance = 5f;
		/// the horizontal distance from the player at which the agent will stop moving. Between that distance and the walk distance, the agent will slow down progressively
		public float StopDistance = 1f;
		/// the minimum vertical distance at which the agent will start jetpacking if the target is above it
		public float JetpackDistance = 0.2f;
		
		// private stuff 
		protected Transform _target;
	    protected CorgiController _controller;
		protected Character _targetCharacter;
	    protected CharacterHorizontalMovement _characterHorizontalMovement;
	    protected CharacterRun _characterRun;
	    protected CharacterJump _characterJump;
	    protected CharacterJetpack _jetpack;
	    protected float _speed;
	    protected float _direction;

	    /// <summary>
	    /// Initialization
	    /// </summary>
	    protected virtual void Start () 
		{
			if (LevelManager.Instance.Players.Count == 0)
			{
				return;
			}
			// we get the player
			_target=LevelManager.Instance.Players[0].transform;
			// we get its components
			_controller = GetComponent<CorgiController>();
			_targetCharacter = GetComponent<Character>();
			_characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
			_characterRun = GetComponent<CharacterRun>();
			_characterJump = GetComponent<CharacterJump>();
			_jetpack = GetComponent<CharacterJetpack>();

			// we make the agent start following the player
			AgentFollowsPlayer=true;

			_targetCharacter.MovementState.ChangeState (CharacterStates.MovementStates.Idle);
		}

	    /// <summary>
	    /// Every frame, we make the agent move towards the player
	    /// </summary>
	    protected virtual void Update () 
		{
			// if the agent is not supposed to follow the player, we do nothing.
			if (!AgentFollowsPlayer)
				return;
				
			// if the Follower doesn't have the required components, we do nothing.
			if ( (_targetCharacter==null) || (_controller==null) )
				return;

			if ((_targetCharacter.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
				|| (_targetCharacter.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
			{
				return;
			}
			
			// we calculate the distance between the target and the agent
			float distance = Mathf.Abs(_target.position.x - transform.position.x);
					
			// we determine the direction	
			_direction = _target.position.x>transform.position.x ? 1f : -1f;

			if (_characterRun != null && _characterRun.AbilityInitialized)
			{
				// depending on the distance between the agent and the player, we set the speed and behavior of the agent.
				if (distance>RunDistance)
				{
					// run
					_speed=1;
					_characterRun.RunStart();
				}
				else
				{
					_characterRun.RunStop();
				}
			}

			if (distance<RunDistance && distance>WalkDistance)
			{
				// walk
				_speed=1;
			}
			if (distance<WalkDistance && distance>StopDistance)
			{
				// walk slowly
				_speed=distance/WalkDistance;
			}
			if (distance<StopDistance)
			{
				// stop
				_speed=0f;
			}
			
			// we make the agent move
			_characterHorizontalMovement.SetHorizontalMove(_speed*_direction);

			if (_characterJump != null)
			{
				// if there's an obstacle on the left or on the right of the agent, we make it jump. If it's moving, it'll jump over the obstacle.
				if (_controller.State.IsCollidingRight || _controller.State.IsCollidingLeft)
				{
					_characterJump.JumpStart();
				}
			}

			// if the follower is equipped with a jetpack
			if (_jetpack!=null && _jetpack.AbilityInitialized)
			{
				// if the player is above the agent + a magic factor, we make the agent start jetpacking
				if (_target.position.y>transform.position.y+JetpackDistance)
				{
					_jetpack.JetpackStart();
				}
				else
				{
					if (_targetCharacter.MovementState.CurrentState == CharacterStates.MovementStates.Jetpacking)
					{
						_jetpack.JetpackStop();	
					}
				}
			}
		}
	}
}