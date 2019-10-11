using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using Cinemachine;

namespace MoreMountains.CorgiEngine
{
    public enum MMCameraEventTypes { SetTargetCharacter, SetConfiner, StartFollowing, StopFollowing }

    public struct MMCameraEvent
    {
        public MMCameraEventTypes EventType;
        public Character TargetCharacter;
        public Collider Bounds;

        public MMCameraEvent(MMCameraEventTypes eventType, Character targetCharacter = null, Collider bounds = null)
        {
            EventType = eventType;
            TargetCharacter = targetCharacter;
            Bounds = bounds;
        }

        static MMCameraEvent e;
        public static void Trigger(MMCameraEventTypes eventType, Character targetCharacter = null, Collider bounds = null)
        {
            e.EventType = eventType;
            e.Bounds = bounds;
            e.TargetCharacter = targetCharacter;
            MMEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// A class that handles camera follow for Cinemachine powered cameras
    /// </summary>
    public class CinemachineCameraController : MonoBehaviour, MMEventListener<MMCameraEvent>, MMEventListener<CorgiEngineEvent>
    {
        public enum PerspectiveZoomMethods { FieldOfView, FramingTransposerDistance }

        /// True if the camera should follow the player
        public bool FollowsPlayer { get; set; }

        [Header("Settings")]
        /// if this is true, this camera will follow a player
        public bool FollowsAPlayer = true;
        /// whether this camera should be confined by the bounds determined in the LevelManager or not
        public bool ConfineCameraToLevelBounds = true;
        /// How high (or low) from the Player the camera should move when looking up/down
        public float ManualUpDownLookDistance = 3;
        /// the min and max speed to consider for this character (when dealing with the zoom)
        [Vector("Min", "Max")]
        public Vector2 CharacterSpeed = new Vector2(0f, 16f);
        [ReadOnly]
        /// the target character this camera follows
        public Character TargetCharacter;
        [ReadOnly]
        /// the controller bound to the character this camera follows
        public CorgiController TargetController;

        [Space(10)]
        [Header("Orthographic Zoom")]
        [Information("Determine here the min and max zoom, and the zoom speed. By default the engine will zoom out when your character is going at full speed, and zoom in when you slow down (or stop).", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// Whether this camera should zoom in or out as the character moves 
        public bool UseOrthographicZoom = false;
        /// the minimum & maximum orthographic camera zoom        
        [Condition("UseOrthographicZoom", true)]
        [VectorAttribute("Min", "Max")]
        public Vector2 OrthographicZoom = new Vector2(5f, 9f);
        [Condition("UseOrthographicZoom", true)]
        public float InitialOrthographicZoom = 5f;
        /// the speed at which the orthographic camera zooms	
        [Condition("UseOrthographicZoom", true)]
        public float OrthographicZoomSpeed = 0.4f;

        [Space(10)]
        [Header("Perspective Zoom")]
        [Information("Determine here the min and max zoom, and the zoom speed when the camera is in perspective mode. You can pick two zoom methods, either playing with the field of view or the transposer's distance.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// if this is true, perspective zoom will be processed every frame
        public bool UsePerspectiveZoom = false;
        /// the zoom method for this camera
        [Condition("UsePerspectiveZoom", true)]
        public PerspectiveZoomMethods PerspectiveZoomMethod = PerspectiveZoomMethods.FramingTransposerDistance;
        /// the min and max perspective camera zooms
        [Condition("UsePerspectiveZoom", true)]
        [VectorAttribute("Min", "Max")]
        public Vector2 PerspectiveZoom = new Vector2(10f, 15f);
        [Condition("UsePerspectiveZoom", true)]
        public float InitialPerspectiveZoom = 5f;
        /// the speed at which the perspective camera zooms	
        [Condition("UsePerspectiveZoom", true)]
        public float PerspectiveZoomSpeed = 0.4f;

        [Space(10)]
        [Header("Respawn")]
        /// if this is true, the camera will teleport to the player's location on respawn, otherwise it'll move there at its regular speed
        public bool InstantRepositionCameraOnRespawn = false;

        protected CinemachineVirtualCamera _virtualCamera;
        protected CinemachineConfiner _confiner;
        protected CinemachineFramingTransposer _framingTransposer;

        protected float _currentZoom;

        /// <summary>
        /// On Awake we grab our components
        /// </summary>
        protected virtual void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _confiner = GetComponent<CinemachineConfiner>();

            _currentZoom = _virtualCamera.m_Lens.Orthographic ? InitialOrthographicZoom : InitialPerspectiveZoom;
            _framingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        /// <summary>
        /// On Start we assign our bounding volume
        /// </summary>
        protected virtual void Start()
        {
            if ((_confiner != null) && ConfineCameraToLevelBounds)
            {
                _confiner.m_BoundingVolume = LevelManager.Instance.BoundsCollider;
            }
            if (UseOrthographicZoom)
            {
                _virtualCamera.m_Lens.OrthographicSize = InitialOrthographicZoom;
            }
            if (UsePerspectiveZoom)
            {
                SetPerspectiveZoom(InitialPerspectiveZoom);
            }
        }

        /// <summary>
        /// Sets a new target for this camera to track
        /// </summary>
        /// <param name="character"></param>
        public virtual void SetTarget(Character character)
        {
            TargetCharacter = character;
            TargetController = character.gameObject.MMGetComponentNoAlloc<CorgiController>();
        }

        /// <summary>
        /// Starts following the LevelManager's main player
        /// </summary>
        public virtual void StartFollowing()
        {
            if (!FollowsAPlayer) { return; }
            FollowsPlayer = true;
            _virtualCamera.Follow = TargetCharacter.CameraTarget.transform;
        }

        /// <summary>
        /// Stops following any target
        /// </summary>
        public virtual void StopFollowing()
        {
            if (!FollowsAPlayer) { return; }
            FollowsPlayer = false;
            _virtualCamera.Follow = null;
        }

        /// <summary>
        /// On late update, we handle our zoom level
        /// </summary>
        protected virtual void LateUpdate()
        {
            HandleZoom();        
        }

        /// <summary>
        /// Makes the camera zoom in or out based on the current target speed
        /// </summary>
        protected virtual void HandleZoom()
        {
            if (_virtualCamera.m_Lens.Orthographic)
            {
                PerformOrthographicZoom();
            }
            else
            {
                PerformPerspectiveZoom();
            }
        }

        /// <summary>
        /// Modifies the orthographic zoom
        /// </summary>
        protected virtual void PerformOrthographicZoom()
        {
            if (!UseOrthographicZoom || (TargetController == null))
            {
                return;
            }

            float characterSpeed = Mathf.Abs(TargetController.Speed.x);
            float currentVelocity = Mathf.Max(characterSpeed, CharacterSpeed.x);
            float targetZoom = MMMaths.Remap(currentVelocity, CharacterSpeed.x, CharacterSpeed.y, OrthographicZoom.x, OrthographicZoom.y);
            _currentZoom = Mathf.Lerp(_currentZoom, targetZoom, Time.deltaTime * OrthographicZoomSpeed);
            _virtualCamera.m_Lens.OrthographicSize = _currentZoom;
        }

        /// <summary>
        /// Modifies the zoom if the camera is in perspective mode
        /// </summary>
        protected virtual void PerformPerspectiveZoom()
        {
            if (!UsePerspectiveZoom || (TargetController == null))
            {
                return;
            }

            float characterSpeed = Mathf.Abs(TargetController.Speed.x);
            float currentVelocity = Mathf.Max(characterSpeed, CharacterSpeed.x);
            float targetZoom = MMMaths.Remap(currentVelocity, CharacterSpeed.x, CharacterSpeed.y, PerspectiveZoom.x, PerspectiveZoom.y);
            _currentZoom = Mathf.Lerp(_currentZoom, targetZoom, Time.deltaTime * PerspectiveZoomSpeed);
            SetPerspectiveZoom(_currentZoom);            
        }

        protected virtual void SetPerspectiveZoom(float newZoom)
        {
            switch (PerspectiveZoomMethod)
            {
                case PerspectiveZoomMethods.FieldOfView:
                    _virtualCamera.m_Lens.FieldOfView = newZoom;
                    break;

                case PerspectiveZoomMethods.FramingTransposerDistance:
                    if (_framingTransposer != null)
                    {
                        _framingTransposer.m_CameraDistance = newZoom;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Acts on MMCameraEvents when caught
        /// </summary>
        /// <param name="cameraEvent"></param>
        public virtual void OnMMEvent(MMCameraEvent cameraEvent)
        {
            switch (cameraEvent.EventType)
            {
                case MMCameraEventTypes.SetTargetCharacter:
                    SetTarget(cameraEvent.TargetCharacter);
                    break;
                case MMCameraEventTypes.SetConfiner:
                    if (_confiner != null)
                    {
                        _confiner.m_BoundingVolume = cameraEvent.Bounds;
                    }
                    break;
                case MMCameraEventTypes.StartFollowing:
                    if (cameraEvent.TargetCharacter != null)
                    {
                        if (cameraEvent.TargetCharacter != TargetCharacter)
                        {
                            return;
                        }
                    }
                    StartFollowing();
                    break;

                case MMCameraEventTypes.StopFollowing:
                    if (cameraEvent.TargetCharacter != null)
                    {
                        if (cameraEvent.TargetCharacter != TargetCharacter)
                        {
                            return;
                        }
                    }
                    StopFollowing();
                    break;
            }
        }

        /// <summary>
        /// Teleports the camera's transform to the target's position
        /// </summary>
        public virtual void TeleportCameraToTarget()
        {
            this.transform.position = TargetCharacter.transform.position;
        }

        /// <summary>
        /// When getting game events, acts on them
        /// </summary>
        /// <param name="corgiEngineEvent"></param>
        public virtual void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
        {
            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.Respawn)
            {
                if (InstantRepositionCameraOnRespawn)
                {
                    TeleportCameraToTarget();
                }
            }

            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.CharacterSwitch)
            {
                SetTarget(LevelManager.Instance.Players[0]);
                StartFollowing();
            }

            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.CharacterSwap)
            {
                SetTarget(LevelManager.Instance.Players[0]);
                StartFollowing();
            }
        }

        /// <summary>
        /// On enable we start listening for events
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMCameraEvent>();
            this.MMEventStartListening<CorgiEngineEvent>();
        }

        /// <summary>
        /// On disable we stop listening for events
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMCameraEvent>();
            this.MMEventStopListening<CorgiEngineEvent>();
        }
    }
}