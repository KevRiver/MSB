//#define RCRECIEVER_LOG_ON
//#define CLIENT_POSITION_MODIFY
using System;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;
using MoreMountains.CorgiEngine;
using Nettention.Proud;
using Vector3 = UnityEngine.Vector3;

public static class GlobalData
{
    public static int logCnt = 0;
}

public class RCReciever : MonoBehaviour, MMEventListener<MMGameEvent>
{
    private bool isInitialized = false;
    public MSB_Character character;
    private CorgiController _controller;
    public Transform characterModel;
    public Transform weaponAttachment;
    public Weapon weapon;

    public int userNum;

    // For facing direction sync
    public bool lastFacing;

    private Transform _aimIndicator;
    private Vector3 _curPos;
    private Quaternion _targetRot;
    private bool onSync = false;
    private CharacterAbility _ability;
    private CharacterSpin _characterSpin;
    private PositionFollower _positionFollower;
    private float _bounceSpeed;

    void Start()
    {
        if (!(character = GetComponent<MSB_Character>()))
        {
#if RCRECIEVER_LOG_ON
        Debug.Log("MSB_Character is null");
#endif
        }

        if (!(_controller = GetComponent<CorgiController>()))
        {
#if RCRECIEVER_LOG_ON
        Debug.Log("CorgiController is null");
#endif
        }

        _controller.IsRemote = true;
        _positionFollower = new PositionFollower();
        _positionFollower.Gravity = new Nettention.Proud.Vector3(0f, _controller.Parameters.Gravity, 0f);

        characterModel = character.transform.GetChild(0);
        weaponAttachment = characterModel.GetChild(0);
        _aimIndicator = weaponAttachment.GetChild(0);
        if(!(_aimIndicator = weaponAttachment.GetChild(0)))
            _aimIndicator.gameObject.SetActive(false);
        weapon = weaponAttachment.GetComponentInChildren<Weapon>();
        _ability = GetComponent<MSB_CharacterDash>();

        userNum = character.UserNum;

        NetworkModule.GetInstance().AddOnEventGameUserMove(new OnGameUserMove(this));
        NetworkModule.GetInstance().AddOnEventGameUserSync(new OnGameUserSync(this));

        isInitialized = true;
    }

    private enum MSBCollision
    {
        Above,
        Below,
        Left,
        Right,
        None
    }

    private MSBCollision CheckCollision()
    {
        if (_controller.State.IsCollidingBelow)
            return MSBCollision.Below;
        else if (_controller.State.IsCollidingAbove)
            return MSBCollision.Above;
        else if (_controller.State.IsCollidingLeft)
            return MSBCollision.Left;
        else if (_controller.State.IsCollidingRight)
            return MSBCollision.Right;
        return MSBCollision.None;
    }

    private void ImmediatelyModifyPositionFollower(MSBCollision collisionCheck)
    {
        var p = new Nettention.Proud.Vector3(transform.position.x, transform.position.y, 0f);
        var v = new Nettention.Proud.Vector3(_controller.Speed.x, _controller.Speed.y, 0f);
        if (collisionCheck.Equals(MSBCollision.None))
            return;
        switch (collisionCheck)
        {
            case MSBCollision.Above:
                p.y = _controller.PositionLimitAbove;
                break;
            case MSBCollision.Below:
                p.y = _controller.PositionLimitBelow;
                break;
            case MSBCollision.Left:
                p.x = _controller.PositionLimitLeft;
                break;
            case MSBCollision.Right:
                p.x = _controller.PositionLimitRight;
                break;
        }
        _positionFollower.SetFollower(p, v);
        _positionFollower.SetTarget(p, v);
#if RCRECIEVER_LOG_ON && UNITY_EDITOR
        Debug.LogFormat("ImmediatelyModifyPositionFollower:: {0} {1}, {2}, {3}, {4}", ++(GlobalData.logCnt), p.x, p.y, v.x, v.y);
#endif
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                StartCoroutine(SyncUserPos());
                onSync = true;
                break;

            case "GameOver":
                StopCoroutine(SyncUserPos());
                onSync = false;
                break;
        }
    }

    public void StartMoveSync()
    {
        onSync = true;
        StartCoroutine(SyncUserPos());
    }

    public void StopMoveSync()
    {
        if (onSync)
        {
            onSync = false;
            StopCoroutine(SyncUserPos());
        }
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        if (!isInitialized)
            return;
        StartCoroutine(SyncUserPos());
        onSync = true;
    }

    private void OnDisable()
    {
        if (onSync)
        {
            StopCoroutine(SyncUserPos());
        }
        onSync = false;
        this.MMEventStopListening<MMGameEvent>();
    }

    private IEnumerator SyncUserPos()
    {
        while (true)
        {
            if (!onSync)
            {
                // 플레이어 리스폰 시 target, follower 초기화
                var initialP = new Nettention.Proud.Vector3();
                var initialV = new Nettention.Proud.Vector3();
                var pos = transform.position;
                var vel = _controller.Speed;
                initialP.x = pos.x;
                initialP.y = pos.y;
                initialV.x = vel.x;
                initialV.y = vel.y;
                _positionFollower.SetTarget(initialP,initialV);
                _positionFollower.SetFollower(initialP,initialV);
                yield return null;
            }
            else
            {
                
                _positionFollower.FrameMove(Time.deltaTime);

                var p = new Nettention.Proud.Vector3();
                var v = new Nettention.Proud.Vector3();
#if CLIENT_POSITION_MODIFY
                MSBCollision collisionCheck = CheckCollision();
                if (collisionCheck != MSBCollision.None)
                    ImmediatelyModifyPositionFollower(collisionCheck);
#endif
                _positionFollower.GetFollower(ref p, ref v);
                transform.position = new Vector3((float) p.x, (float) p.y, (float) p.z);

#if RCRECIEVER_LOG_ON && UNITY_EDITOR
            Debug.LogFormat("NewPosition:: {0},{1}", p.x, p.y);
            var targetPosition = _positionFollower.TargetPosition;
            var targetVelocity = _positionFollower.TargetVelocity;
            Debug.LogFormat("TargetPositionData:: {0}, {1}, {2}, {3}", targetPosition.x, targetPosition.y, targetVelocity.x,
            targetVelocity.y);
#endif
                yield return null;
            }
        }
    }

    public void AttackSync(Quaternion rot)
    {
        characterModel.rotation = rot;
        if (weapon != null)
            weapon.WeaponState.ChangeState(Weapon.WeaponStates.WeaponUse);
        if (_ability != null)
            _ability.StartAbility();
    }
    
    private class OnGameUserMove : NetworkModule.OnGameUserMoveListener
    {
        private RCReciever _rc;
        private int _userNum;
        private int _targetNum;
        private float _posX;
        private float _posY;
        private float _posZ;
        private float _xSpeed;
        private float _ySpeed;
        private bool _isFacingRight;

        public OnGameUserMove(RCReciever rc)
        {
            _rc = rc;
            _userNum = _rc.userNum;
        }

        readonly char[] _delimiterChars = {','};

        void NetworkModule.OnGameUserMoveListener.OnGameUserMove(object data)
        {
            // if received data's latency is over the threshold ignore it
            string[] dataArray = ((string) data).Split(_delimiterChars);
            _targetNum = int.Parse(dataArray[0]);
            if (_userNum != _targetNum)
                return;
            _posX = float.Parse(dataArray[1]);
            _posY = float.Parse(dataArray[2]);
            _posZ = float.Parse(dataArray[3]);
            _xSpeed = float.Parse(dataArray[4]);
            _ySpeed = float.Parse(dataArray[5]);
            _isFacingRight = bool.Parse(dataArray[6]);
#if RCRECIEVER_LOG_ON && UNITY_EDITOR
            Debug.LogFormat("ReceivedData:: {0} {1}, {2}, {3}, {4}", ++(GlobalData.logCnt),_posX, _posY, _xSpeed, _ySpeed);
#endif
            Nettention.Proud.Vector3 pos = new Nettention.Proud.Vector3();
            pos.x = _posX;
            pos.y = _posY;
            pos.z = _posZ;
            Nettention.Proud.Vector3 vel = new Nettention.Proud.Vector3();
            vel.x = _xSpeed;
            vel.y = _ySpeed;
            vel.z = 0f;
            _rc._positionFollower.SetTarget(pos, vel);
            if (_rc.lastFacing != _isFacingRight)
            {
                _rc.lastFacing = !_rc.lastFacing;
                _rc.character.Flip();
            }
        }
    }

    private class OnGameUserSync : NetworkModule.OnGameUserSyncListener
    {
        private RCReciever _rc;
        private int _userNum;
        private int _targetNum;
        private Quaternion _rot;

        public OnGameUserSync(RCReciever rc)
        {
#if RCRECIEVER_LOG_ON && UNITY_EDITOR
            Debug.Log("OnGameUserSync Constructor called");
#endif
            _rc = rc;
            _userNum = _rc.userNum;
        }

        readonly char[] _delimiterChars = {','};
        void NetworkModule.OnGameUserSyncListener.OnGameUserSync(object data)
        {
            string[] dataArray = ((string) data).Split(_delimiterChars);
            _targetNum = int.Parse(dataArray[0]);
            if (_userNum != _targetNum)
                return;
            _rot.x = float.Parse(dataArray[1]);
            _rot.y = float.Parse(dataArray[2]);
            _rot.z = float.Parse(dataArray[3]);
            _rot.w = float.Parse(dataArray[4]);
            _rc.AttackSync(_rot);
        }
    }
}
