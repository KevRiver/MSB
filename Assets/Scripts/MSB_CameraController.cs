using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class MSB_CameraController : CameraController
{
    [Header("MSB Custom")]
    public MSB_Character TargetCharacter;

    // Start is called before the first frame update
    protected override void Start()
    {
        // we get the camera component
        _camera = GetComponent<Camera>();

        // We make the camera follow the player
        FollowsPlayer = true;
        _currentZoom = MinimumZoom;

        // we make sure we have a Player
        if ((MSB_LevelManager.Instance.MSB_Players == null) || (MSB_LevelManager.Instance.MSB_Players.Count == 0))
        {
            Debug.LogWarning("MSB_CameraController : The LevelManager couldn't find a Player character. Make sure there's one set in the Level Manager. The camera script won't work without that.");
            return;
        }
        Debug.Log("MSB_CameraController Assign Target");
        AssignTarget();

        // if we have a level manager, we grab our level bounds and optionnally our no going back object
        if (MSB_LevelManager.Instance != null)
        {
            _levelBounds = MSB_LevelManager.Instance.LevelBounds;
            if (MSB_LevelManager.Instance.OneWayLevelMode != MSB_LevelManager.OneWayLevelModes.None)
            {
                _noGoingBackObject = MSB_LevelManager.Instance.NoGoingBackObject;
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
            MakeCameraPixelPerfect();
            GetLevelBounds();
        }
        else
        {
            Zoom();
        }
    }

    protected override void AssignTarget()
    {
        Debug.Log("Check MSB Players at MSB_LevelManager : " + MSB_LevelManager.Instance.MSB_Players.Count);
        ///MSB Cusotm 로컬유저의 유저넘버와 Character의 c_userData.userNumber를 비교하여 로컬유저의 캐릭터라면 Camera의 Target으로 설정
        foreach (MSB_Character player in MSB_LevelManager.Instance.MSB_Players)
        {
            Debug.Log("UserNumber : " + player.c_userData.userNumber);
            Debug.Log("LocalUserNumber : " + LocalUser.Instance.localUserData.userNumber);
            if (player.c_userData.userNumber == LocalUser.Instance.localUserData.userNumber)
            {
                Target = player.transform;
                Debug.Log(Target.gameObject.name);
                TargetController = Target.GetComponent<CorgiController>();
                TargetCharacter = player;
            }
        }
    }

    private IEnumerator CameraInitialization()
    {
        if ((MSB_LevelManager.Instance.MSB_Players == null) || (MSB_LevelManager.Instance.MSB_Players.Count == 0))
            yield return null;

        Debug.Log("MSB_CameraCotroller AssignTarget");
        AssignTarget();

        // if we have a level manager, we grab our level bounds and optionnally our no going back object
        if (MSB_LevelManager.Instance != null)
        {
            _levelBounds = MSB_LevelManager.Instance.LevelBounds;
            if (MSB_LevelManager.Instance.OneWayLevelMode != MSB_LevelManager.OneWayLevelModes.None)
            {
                _noGoingBackObject = MSB_LevelManager.Instance.NoGoingBackObject;
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
            MakeCameraPixelPerfect();
            GetLevelBounds();
        }
        else
        {
            Zoom();
        }
    }
    
    protected override void LateUpdate()
    {
        GetLevelBounds();
        FollowPlayer();

        if (TargetCharacter.MovementState.CurrentState == CharacterStates.MovementStates.Jumping)
        {
           
            ZoomOut();
        }
        else
        {           
            Zoom();
        }
    }

    public void ZoomOut()
    {
        if (PixelPerfect)
        {
            return;
        }

        float characterSpeed = Mathf.Abs(TargetController.Speed.x);
        float currentVelocity = 0f;

        _currentZoom = Mathf.SmoothDamp(_currentZoom, MaximumZoom, ref currentVelocity, ZoomSpeed);

        _camera.orthographicSize = _currentZoom;
    }

    protected override void FollowPlayer()
    {
        if (Target == null)
            return;

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
        if (_shakeDuration > 0)
        {
            shakeFactorPosition = Random.insideUnitSphere * _shakeIntensity * _shakeDuration;
            _shakeDuration -= _shakeDecay * Time.deltaTime;
        }
        newCameraPosition = newCameraPosition + shakeFactorPosition;


        if (_camera.orthographic == true)
        {            
            float posX, posY, posZ = 0f;
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
            transform.position = new Vector3(posX, posY, posZ);            
        }
        else
        {
            transform.position = newCameraPosition;
        }

        _lastTargetPosition = Target.position;
    }
}
