using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{	
	[RequireComponent(typeof(Camera))]
	/// <summary>
	/// The Corgi Engine's Camera Controller. Handles camera movement, shakes, player follow.
	/// </summary>
	[AddComponentMenu("Corgi Engine/Camera/Camera Controller")]
	public class CameraController : MonoBehaviour, MMEventListener<CorgiEngineEvent>
	{
		/// True if the camera should follow the player
		public bool FollowsPlayer{get;set;}

		[Space(10)]	
		[Header("Distances")]
		[Information("The Horizontal Look Distance defines how far ahead from the player the camera should be. The camera offset is an offset applied at all times. The LookAheadTrigger defines the minimal distance you need to move to trigger camera movement. Finally you can define how far up or below the camera will move when looking up or down.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// How far ahead from the Player the camera is supposed to be		
		public float HorizontalLookDistance = 3;
		/// Vertical Camera Offset	
		public Vector3 CameraOffset ;
		/// Minimal distance that triggers look ahead
		public float LookAheadTrigger = 0.1f;
		/// How high (or low) from the Player the camera should move when looking up/down
		public float ManualUpDownLookDistance = 3;
		
		
		[Space(10)]	
		[Header("Movement Speed")]
		[Information("Here you can define how fast the camera goes back to the player, and how fast it moves generally.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// How fast the camera goes back to the Player
		public float ResetSpeed = 0.5f;
		/// How fast the camera moves
		public float CameraSpeed = 0.3f;
		
		[Space(10)]	
		[Header("Camera Zoom")]
		[Information("Determine here the min and max zoom, and the zoom speed. By default the engine will zoom out when your character is going at full speed, and zoom in when you slow down (or stop). Note that if you turn Pixel Perfect on, zoom will be disabled.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		[Range (1, 20)]
		/// the minimum camera zoom
		public float MinimumZoom = 5f;
		[Range (1, 20)]
		/// the maximum camera zoom
		public float MaximumZoom = 10f;
		/// the speed at which the camera zooms	
		public float ZoomSpeed = 0.4f;

		[Space(10)]	
		[Header("Pixel Perfect")]
		[Information("Here you can decide to have a pixel perfect camera, which will resize the camera's orthographic size on start to match the desired size and pixel per units.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// if this is true, the script will resize the camera's orthographic size on start to match the desired size and PPU
		/// if you want to know more about that trick, have a look at Unity's post there : https://blogs.unity3d.com/2015/06/19/pixel-perfect-2d/
		public bool PixelPerfect = false;
		/// the vertical resolution for which you've created your visual assets
		public int ReferenceVerticalResolution = 768;
		/// the reference PPU value (the one you set on your sprites)
		public float ReferencePixelsPerUnit = 32;
		
		[Space(10)]	
		[Header("Camera Effects")]
		[Information("If EnableEffectsOnMobile is set to false, all Cinematic Effects on the camera will be removed at start on mobile targets.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// If set to false, all Cinematic Effects on the camera will be removed at start on mobile targets
		public bool EnableEffectsOnMobile = false;

		[Space(10)]	
		[Header("Respawn")]
		/// if this is true, the camera will teleport to the player's location on respawn, otherwise it'll move there at its regular speed
		public bool InstantRepositionCameraOnRespawn = false;

		
        [ReadOnly]
		public Transform Target;
        [ReadOnly]
        public CorgiController TargetController;

		protected Bounds _levelBounds;

	    protected float _xMin;
	    protected float _xMax;
	    protected float _yMin;
	    protected float _yMax;	 
		
		protected float _offsetZ;
		protected Vector3 _lastTargetPosition;
	    protected Vector3 _currentVelocity;
	    protected Vector3 _lookAheadPos;

	    protected float _shakeIntensity;
	    protected float _shakeDecay;
	    protected float _shakeDuration;
		
		protected float _currentZoom;	
		protected Camera _camera;

		protected NoGoingBack _noGoingBackObject;

	    protected Vector3 _lookDirectionModifier = new Vector3(0,0,0);
		
		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Start ()
		{		
			// we get the camera component
			_camera=GetComponent<Camera>();
            
			// We make the camera follow the player
			FollowsPlayer=true;
			_currentZoom=MinimumZoom;
			
			// we make sure we have a Player
			if ( (LevelManager.Instance.Players == null) || (LevelManager.Instance.Players.Count == 0) )
			{
				Debug.LogWarning ("CameraController : The LevelManager couldn't find a Player character. Make sure there's one set in the Level Manager. The camera script won't work without that.");
				return;
			}

            AssignTarget();

			// if we have a level manager, we grab our level bounds and optionnally our no going back object
			if (LevelManager.Instance!=null)
			{
				_levelBounds = LevelManager.Instance.LevelBounds;
				if (LevelManager.Instance.OneWayLevelMode != LevelManager.OneWayLevelModes.None)
				{
					_noGoingBackObject = LevelManager.Instance.NoGoingBackObject;
				}
				else
				{
					_noGoingBackObject = null;
				}
			}
			
			// we store the target's last position
			_lastTargetPosition = Target.position;
			_offsetZ = (transform.position - Target.position).z;
			transform.parent = null;


			if (PixelPerfect)
			{
				MakeCameraPixelPerfect ();
				GetLevelBounds();
			}
			else
			{
				Zoom();	
			}
		}

        protected virtual void AssignTarget()
        {
            // we make sure it has a CorgiController associated to it
            Target = LevelManager.Instance.Players[0].transform;
            if (Target.GetComponent<CorgiController>() == null)
            {
                Debug.LogWarning("CameraController : The Player character doesn't have a CorgiController associated to it, the Camera won't work.");
                return;
            }

            TargetController = Target.GetComponent<CorgiController>();
        }


	    /// <summary>
	    /// Every frame, we move the camera if needed
	    /// </summary>
		protected virtual void LateUpdate () 
		{
			GetLevelBounds();
			// if the camera is not supposed to follow the player, we do nothing
			if (!FollowsPlayer || TargetController == null)
			{
				return;
			}
				
			if (!PixelPerfect)
			{
				Zoom();	
			}
				
			FollowPlayer ();
		}	

		/// <summary>
		/// Use this method to shake the camera, passing in a Vector3 for intensity, duration and decay
		/// </summary>
		/// <param name="shakeParameters">Shake parameters : intensity, duration and decay.</param>
		public virtual void Shake(Vector3 shakeParameters)
		{
			_shakeIntensity = shakeParameters.x;
			_shakeDuration=shakeParameters.y;
			_shakeDecay=shakeParameters.z;
		}

	    /// <summary>
	    /// Moves the camera up
	    /// </summary>
	    public virtual void LookUp()
		{
			_lookDirectionModifier = new Vector3(0,ManualUpDownLookDistance,0);
		}

	    /// <summary>
	    /// Moves the camera down
	    /// </summary>
	    public virtual void LookDown()
		{
			_lookDirectionModifier = new Vector3(0,-ManualUpDownLookDistance,0);
		}

	    /// <summary>
	    /// Resets the look direction modifier
	    /// </summary>
	    public virtual void ResetLookUpDown()
		{	
			_lookDirectionModifier = new Vector3(0,0,0);
		}

		/// <summary>
		/// Makes the camera pixel perfect by resizing its orthographic size according to the current screen's size.
		/// </summary>
		protected virtual void MakeCameraPixelPerfect ()
		{
			int screenHeight = Screen.height;
			float newOrthographicSize = (screenHeight / ReferencePixelsPerUnit) * 0.5f;
			float referenceSize = (ReferenceVerticalResolution/ ReferencePixelsPerUnit) * 0.5f;

			float rounder = Mathf.Max(1, Mathf.Round(newOrthographicSize / referenceSize));
			newOrthographicSize = newOrthographicSize / rounder;

			QualitySettings.antiAliasing = 0;
			_camera.orthographicSize = newOrthographicSize;
		}

		/// <summary>
		/// Moves the camera around so it follows the player
		/// </summary>
		protected virtual void FollowPlayer()
		{
			// if the player has moved since last update
			float xMoveDelta = (Target.position - _lastTargetPosition).x;
			bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > LookAheadTrigger;

			if (updateLookAheadTarget) 
			{
				_lookAheadPos = HorizontalLookDistance * Vector3.right * Mathf.Sign(xMoveDelta);
			} 
			else 
			{
				_lookAheadPos = Vector3.MoveTowards(_lookAheadPos, Vector3.zero, Time.deltaTime * ResetSpeed);	
			}

			Vector3 aheadTargetPos = Target.position + _lookAheadPos + Vector3.forward * _offsetZ + _lookDirectionModifier + CameraOffset;
			Vector3 newCameraPosition = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref _currentVelocity, CameraSpeed);
			Vector3 shakeFactorPosition = Vector3.zero;

			// If shakeDuration is still running.
			if (_shakeDuration>0)
			{
				shakeFactorPosition= Random.insideUnitSphere * _shakeIntensity * _shakeDuration;
				_shakeDuration-=_shakeDecay*Time.deltaTime ;
			}		
			newCameraPosition = newCameraPosition+shakeFactorPosition;		


			if (_camera.orthographic==true)
			{
				float posX,posY,posZ=0f;
				// Clamp to level boundaries
				if (_levelBounds.size != Vector3.zero)
				{
					posX = Mathf.Clamp(newCameraPosition.x, _xMin, _xMax);
					posY = Mathf.Clamp(newCameraPosition.y, _yMin, _yMax);
				}
				else
				{
					posX = newCameraPosition.x;
					posY = newCameraPosition.y;
				}
				posZ = newCameraPosition.z;
				// We move the actual transform
				transform.position=new Vector3(posX, posY, posZ);
			}
			else
			{
				transform.position=newCameraPosition;
			}		

			_lastTargetPosition = Target.position;	
		}

		/// <summary>
		/// Handles the zoom of the camera based on the main character's speed
		/// </summary>
		protected virtual void Zoom()
		{
			// if we're in pixel perfect mode, we do nothing and exit.
			if (PixelPerfect)
			{
				return;
			}

			float characterSpeed=Mathf.Abs(TargetController.Speed.x);
			float currentVelocity=0f;

			_currentZoom=Mathf.SmoothDamp(_currentZoom,(characterSpeed/10)*(MaximumZoom-MinimumZoom)+MinimumZoom,ref currentVelocity,ZoomSpeed);

			_camera.orthographicSize=_currentZoom;
			//GetLevelBounds();
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
			float cameraHeight = _camera.orthographicSize * 2f;		
			float cameraWidth = cameraHeight * _camera.aspect;

			_xMin = _levelBounds.min.x+(cameraWidth/2);
			_xMax = _levelBounds.max.x-(cameraWidth/2); 
			_yMin = _levelBounds.min.y+(cameraHeight/2); 
			_yMax = _levelBounds.max.y-(cameraHeight/2);

			// if we have a NoGoingBackObject, we prevent the camera from going further back
			if (_noGoingBackObject != null)
			{
				if (_noGoingBackObject.OneWayLevelMode == LevelManager.OneWayLevelModes.Left)
				{
					float newXMin = _noGoingBackObject.transform.position.x + (_noGoingBackObject.NoGoingBackColliderSize.x / 2) + (cameraWidth/2);
					_xMin = (cameraWidth/2 <= Mathf.Abs(_levelBounds.max.x - newXMin)) ? newXMin : _levelBounds.max.x - cameraWidth/2 + 0.001f;
				}

				if (_noGoingBackObject.OneWayLevelMode == LevelManager.OneWayLevelModes.Right)
				{
					float newXMax = _noGoingBackObject.transform.position.x - (_noGoingBackObject.NoGoingBackColliderSize.x / 2) - (cameraWidth/2);
					_xMax = (cameraWidth/2 <= Mathf.Abs(newXMax - _levelBounds.min.x)) ? newXMax : _levelBounds.min.x + cameraWidth/2 + 0.001f;
				}

				if (_noGoingBackObject.OneWayLevelMode == LevelManager.OneWayLevelModes.Down)
				{
					float newYMin = _noGoingBackObject.transform.position.y + (_noGoingBackObject.NoGoingBackColliderSize.y / 2) + (cameraHeight/2);
					_yMin = (cameraHeight/2 <= Mathf.Abs(_levelBounds.max.y - newYMin)) ? newYMin : _levelBounds.max.y - cameraWidth/2 + 0.001f;
				}

				if (_noGoingBackObject.OneWayLevelMode == LevelManager.OneWayLevelModes.Top)
				{
					float newYMax = _noGoingBackObject.transform.position.y - (_noGoingBackObject.NoGoingBackColliderSize.y / 2) - (cameraHeight/2);
					_yMax = (cameraHeight/2 <= Mathf.Abs(newYMax - _levelBounds.min.y)) ? newYMax : _levelBounds.min.y + cameraWidth/2 + 0.001f;
				}
			}

			// if the level is too narrow, we center the camera on the levelbound's horizontal center
			if (_levelBounds.max.x - _levelBounds.min.x <= cameraWidth)
			{
				_xMin = _levelBounds.center.x;
				_xMax = _levelBounds.center.x;
			}

			// if the level is not high enough, we center the camera on the levelbound's vertical center
			if (_levelBounds.max.y - _levelBounds.min.y <= cameraHeight)
			{
				_yMin = _levelBounds.center.y;
				_yMax = _levelBounds.center.y;
			}	
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
				if (InstantRepositionCameraOnRespawn)
				{
					TeleportCameraToTarget ();
				}
            }

            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.CharacterSwitch)
            {
                AssignTarget();
            }

            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.CharacterSwap)
            {
                AssignTarget();
            }
        }

        /// <summary>
		/// When a MMCameraShakeEvent is caught, shakes the camera
		/// </summary>
		/// <param name="shakeEvent">Shake event.</param>
		public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, bool infinite, int channel)
        {
            Vector3 parameters = new Vector3(amplitude, duration, frequency);
            Shake(parameters);
        }

        public virtual void TeleportCameraToTarget()
		{
			this.transform.position = Target.position + _lookAheadPos + Vector3.forward * _offsetZ + _lookDirectionModifier + CameraOffset;
		}

		protected virtual void OnEnable()
		{
			this.MMEventStartListening<CorgiEngineEvent> ();
            MMCameraShakeEvent.Register(OnCameraShakeEvent);
        }

		protected virtual void OnDisable()
		{
			this.MMEventStopListening<CorgiEngineEvent> ();
            MMCameraShakeEvent.Unregister(OnCameraShakeEvent);
        }
	}
}