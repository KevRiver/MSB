using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine 
{
	/// <summary>
	/// Add this script to an orthographic or perspective camera, and it'll try and follow all Character players registered in the LevelManager, and keep them all in the screen.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Corgi Engine/Weapons/Multiplayer Camera Controller")]
	public class MultiplayerCameraController : MonoBehaviour 
	{
		[Header("Camera Controls")]
		/// Defines how fast does the camera change position. Small value : faster camera
		[Information("Add this component to a camera in a multiplayer level, and it'll try and keep all players on screen. Works with orthographic and perspective cameras Here you can tweak the DampTime, which is how fast the camera moves (lower value, faster camera).",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		[Range(0,2)]
		public float DampTime = 0.2f; 

		[Header("Orthographic Parameters")]
		[Information("If the camera is orthographic, you can set its min size here, and how much space around the farthest players should be kept on screen.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// Minimal zoom size
		public float OrthoMinSize = 6.5f;
		/// Space around the screen added after the most distant car
		public float OrthoScreenEdgeBuffer = 4f;

		[Header("Perspective Parameters")]
		[Information("If the camera is perspective based, you can define here its min and max bounds.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the minimum coordinates to which the camera will be constrained
		public Vector3 MinPosition;
		/// the maximum coordinates to which the camera will be constrained
		public Vector3 MaxPosition;

		/// the list of players
		public List<Transform> Players { get; set; }

		protected Camera _camera;
		protected float _zoomSpeed;
		protected Vector3 _moveVelocity;
		protected Vector3 _newPosition;
		protected Vector3 _averagePosition;
		protected float _initialZ;
		protected Bounds _levelBounds;
		protected float _xMin;
		protected float _xMax;
		protected float _yMin;
		protected float _yMax;	
		protected float _aspectRatio;
		protected float _tanFov;

		/// <summary>
		/// On Start, we initialize our camera
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// Stores initial position, aspect ratio, field of view, detects targets and level bounds
		/// </summary>
		protected virtual void Initialization()
		{
			_camera = GetComponentInChildren<Camera>();
			_initialZ = transform.position.z;
			_aspectRatio = Screen.width / Screen.height;
			_tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);

			DetectTargets ();
			if (LevelManager.Instance!=null)
			{
				_levelBounds = LevelManager.Instance.LevelBounds;
			}
			GetLevelBounds ();
		}

		/// <summary>
		/// Looks for and stores the list of players
		/// </summary>
		protected virtual void DetectTargets()
		{
			Players = new List<Transform> ();

			if ( (LevelManager.Instance.Players == null) || (LevelManager.Instance.Players.Count == 0) )
			{
				Debug.LogWarning ("CameraController : The LevelManager couldn't find a Player character. Make sure there's one set in the Level Manager. The camera script won't work without that.");
				return;
			}

			foreach(Character player in LevelManager.Instance.Players)
			{
				Players.Add (player.transform);
			}
		}

		/// <summary>
		/// At FixedUpdate, we compute the new camera's position
		/// </summary>
		protected virtual void FixedUpdate() 
		{
			CameraMovement ();
		}

		/// <summary>
		/// Every fixed update, we determine the new position of the camera and move it
		/// </summary>
		protected virtual void CameraMovement()
		{
			// We need a list of targets to follow
			if (Players == null)
			{
				return;
			}

			CleanPlayersList ();
			FindAveragePosition();
			ComputeZoom();
			ClampNewPosition ();
			MoveCamera ();
		}

		/// <summary>
		/// Moves the camera to the newly computed position.
		/// </summary>
		protected virtual void MoveCamera()
		{
			// Smoothly transition to that position.
			transform.position = Vector3.SmoothDamp(transform.position, _newPosition, ref _moveVelocity, DampTime);
		}

		/// <summary>
		/// Removes disabled players from the list
		/// </summary>
		protected virtual void CleanPlayersList()
		{
			for(var i = Players.Count - 1; i > -1; i--)
			{
				if (Players[i] == null)
					Players.RemoveAt(i);
			}
		}

		/// <summary>
		/// Finds the average position.
		/// </summary>
		protected virtual void FindAveragePosition()
		{
			_averagePosition = Vector3.zero;
			int numTargets = 0;

			// Go through all the targets and add their positions together.
			for (int i = 0; i < Players.Count; i++)
			{
				// Add to the average and increment the number of targets in the average.
				_averagePosition += Players[i].position;
				numTargets++;
			}

			// If there are targets divide the sum of the positions by the number of them to find the average.
			if (numTargets > 0)
			{
				_averagePosition /= numTargets;
			}

			// we fix the z value
			_averagePosition.z = _initialZ;

			// The desired position is the average position;
			_newPosition = _averagePosition;
		}

		/// <summary>
		/// Zooms the camera
		/// </summary>
		protected virtual void ComputeZoom() 
		{
			if (_camera.orthographic)
			{
				float requiredSize;
				// Find the required size based on the desired position and smoothly transition to that size.
				requiredSize = FindRequiredOrthographicSize();
				_camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, requiredSize, ref _zoomSpeed, DampTime);	
				GetLevelBounds();	
			}
			else
			{
				float requiredDistance;
				requiredDistance = FindRequiredDistance();
				_newPosition.z = - requiredDistance;
			}
		}

		/// <summary>
		/// Finds the required size of the orthographic camera's zoom.
		/// </summary>
		/// <returns>The required size.</returns>
		protected virtual float FindRequiredOrthographicSize() 
		{
			Vector3 desiredLocalPos = transform.InverseTransformPoint(_newPosition);

			float size = 0f;

			for (int i = 0; i < Players.Count; i++)
			{
				Vector3 targetLocalPos = transform.InverseTransformPoint(Players[i].position);
				Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

				size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
				size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / _camera.aspect);
			}

			size += OrthoScreenEdgeBuffer;

			size = Mathf.Max(size, OrthoMinSize);

			return size;
		}

		/// <summary>
		/// Determines the perspective camera's z position
		/// </summary>
		/// <returns>The required distance.</returns>
		protected virtual float FindRequiredDistance()
		{
			float maxDistance = 0;
			float newDistance = 0;
			for (int i = 0; i < Players.Count; i++)
			{
				newDistance = Vector3.Distance (Players [i].transform.position, _averagePosition);
				if (newDistance > maxDistance)
				{
					maxDistance = newDistance;
				}
			}

			float distanceBetweenPlayers = newDistance * 2f;
			float cameraDistance = (distanceBetweenPlayers / 2.0f / _aspectRatio) / _tanFov;
			return cameraDistance;
		}

		/// <summary>
		/// Gets the levelbounds coordinates to lock the camera into the level
		/// </summary>
		protected virtual void GetLevelBounds()
		{
			if (_levelBounds.size==Vector3.zero)
			{
				return;
			}

			// camera size calculation (orthographicSize is half the height of what the camera sees.
			float cameraHeight = Camera.main.orthographicSize * 2f;		
			float cameraWidth = cameraHeight * Camera.main.aspect;

			_xMin = _levelBounds.min.x+(cameraWidth/2);
			_xMax = _levelBounds.max.x-(cameraWidth/2); 
			_yMin = _levelBounds.min.y+(cameraHeight/2); 
			_yMax = _levelBounds.max.y-(cameraHeight/2);	
		}

		/// <summary>
		/// Keeps the camera within the defined bounds
		/// </summary>
		protected virtual void ClampNewPosition()
		{
			if (_camera.orthographic)
			{
				if (_levelBounds.size != Vector3.zero)
				{
					_newPosition.x = Mathf.Clamp(_newPosition.x, _xMin, _xMax);
					_newPosition.y = Mathf.Clamp(_newPosition.y, _yMin, _yMax);
				}	
			}
			else
			{
				_newPosition.x = Mathf.Clamp(_newPosition.x, MinPosition.x, MaxPosition.x);
				_newPosition.y = Mathf.Clamp(_newPosition.y, MinPosition.y, MaxPosition.y);
				_newPosition.z = Mathf.Clamp(_newPosition.z, MinPosition.z, MaxPosition.z);
			}

		}
	}
}