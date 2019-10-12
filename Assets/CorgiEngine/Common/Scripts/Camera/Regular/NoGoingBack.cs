using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// This component is automatically added to an object by the LevelManager when you decide to create a One Way level
	/// It will follow the character around, preventing it to go back 
	/// </summary>
	public class NoGoingBack : MonoBehaviour, MMEventListener<CorgiEngineEvent>
	{
		/// the distance at which it should follow the player
		public float ThresholdDistance = 0f;
		/// the target - in most cases the player - the object will follow around
		public Transform Target;
		/// the direction the object should prevent the player to go back to
		public LevelManager.OneWayLevelModes OneWayLevelMode;
		/// the size of the object's collider
		public Vector2 NoGoingBackColliderSize;
		/// the minimum distance at which the object should stay from the level bounds
		public float MinDistanceFromBounds;

		protected Bounds _levelBounds;	
		protected Vector2 _newPosition;
		protected Vector2 _positionLastFrame;

		/// <summary>
		/// On start we initialize our object
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// Grabs the level bounds and determines the initial position of the object
		/// </summary>
		protected virtual void Initialization()
		{
			_positionLastFrame = this.transform.position;
			_levelBounds = LevelManager.Instance.LevelBounds;
			DetermineFirstPostion ();
		}

		/// <summary>
		/// On update we determine the new position of our object
		/// </summary>
		protected virtual void Update()
		{
			DetermineNewPosition ();
		}

		/// <summary>
		/// Determines the first postion of the object based on the one way level mode
		/// </summary>
		protected virtual void DetermineFirstPostion()
		{
			_newPosition = Target.position;

			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Left)	
			{ 
				_newPosition.x -= ThresholdDistance + NoGoingBackColliderSize.x/2f; 
				if (Mathf.Abs(_newPosition.x - _levelBounds.max.x) <= MinDistanceFromBounds) 
				{ 
					_newPosition.x = _positionLastFrame.x; 
				}
			}
			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Right)	
			{ 
				_newPosition.x += ThresholdDistance + NoGoingBackColliderSize.x/2f;
				if (Mathf.Abs(_newPosition.x - _levelBounds.min.x) <= MinDistanceFromBounds) 
				{
					_newPosition.x = _positionLastFrame.x; 
				}
			}
			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Down)	
			{ 
				_newPosition.y -= ThresholdDistance + NoGoingBackColliderSize.y/2f;
				if (Mathf.Abs(_newPosition.y - _levelBounds.max.y) <= MinDistanceFromBounds) 
				{ 
					_newPosition.y = _positionLastFrame.y; 
				} 
			}
			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Top)		
			{ 
				_newPosition.y += ThresholdDistance + NoGoingBackColliderSize.y/2f;
				if (Mathf.Abs(_newPosition.y - _levelBounds.min.y) <= MinDistanceFromBounds)
				{
					_newPosition.y = _positionLastFrame.y; 
				} 
			}

			this.transform.position = _newPosition;
			_positionLastFrame = this.transform.position;
		}

		/// <summary>
		/// Determines the new position of the object based on the one way direction of the level
		/// </summary>
		protected virtual void DetermineNewPosition()
		{
			_newPosition = Target.position;

			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Left)	
			{ 
				_newPosition.x -= ThresholdDistance + NoGoingBackColliderSize.x/2f; 
				if ( (_newPosition.x <= _positionLastFrame.x) || (Mathf.Abs(_newPosition.x - _levelBounds.max.x) <= MinDistanceFromBounds) )
				{ 
					_newPosition.x = _positionLastFrame.x; 
				}
			}
			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Right)	
			{ 
				_newPosition.x += ThresholdDistance + NoGoingBackColliderSize.x/2f;
				if ( (_newPosition.x >= _positionLastFrame.x) || (Mathf.Abs(_newPosition.x - _levelBounds.min.x) <= MinDistanceFromBounds) ) 
				{
					_newPosition.x = _positionLastFrame.x; 
				}
			}
			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Down)	
			{ 
				_newPosition.y -= ThresholdDistance + NoGoingBackColliderSize.y/2f;
				if ( (_newPosition.y <= _positionLastFrame.y) || (Mathf.Abs(_newPosition.y - _levelBounds.max.y) <= MinDistanceFromBounds) )
				{ 
					_newPosition.y = _positionLastFrame.y; 
				} 
			}
			if (OneWayLevelMode == LevelManager.OneWayLevelModes.Top)		
			{ 
				_newPosition.y += ThresholdDistance + NoGoingBackColliderSize.y/2f;
				if ( (_newPosition.y >= _positionLastFrame.y) || (Mathf.Abs(_newPosition.y - _levelBounds.min.y) <= MinDistanceFromBounds) )
				{
					_newPosition.y = _positionLastFrame.y; 
				} 
			}

			this.transform.position = _newPosition;
			_positionLastFrame = this.transform.position;
		}

		/// <summary>
		/// When the player respawns, we reinitialize this object.
		/// </summary>
		/// <param name="checkpoint">Checkpoint.</param>
		/// <param name="player">Player.</param>
		public virtual void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
		{
			if (corgiEngineEvent.EventType == CorgiEngineEventTypes.Respawn) 
			{
				Initialization ();
				DetermineNewPosition ();
			}
		}

		protected virtual void OnEnable()
		{
			this.MMEventStartListening<CorgiEngineEvent> ();
		}

		protected virtual void OnDisable()
		{
			this.MMEventStopListening<CorgiEngineEvent> ();
		}
	}
}
