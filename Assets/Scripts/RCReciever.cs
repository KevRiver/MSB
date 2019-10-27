using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;
using MoreMountains.CorgiEngine;
using UnityEngine.Serialization;
using UnityScript.Steps;

public class RCReciever : MonoBehaviour
{
    public MSB_Character character;
    private CorgiController _controller;
    public Transform characterModel;
    public Transform weaponAttachment;
    public Weapon weapon;

    public int userNum;
    // For position sync
    public float posX = 0;
    public float posY = 0;
    public float posZ = 0;
    public float xSpeed = 0;
    public float ySpeed = 0;
    // For facing direction sync
    public bool isFacingRight;
    public bool lastFacing;

    private Vector3 _targetPos;
    private Vector2 _speed;
    private Quaternion _targetRot;

    void Start()
    {
        Debug.Log("RCReciever Start");
        if (!(character = GetComponent<MSB_Character>()))
            Debug.Log("MSB_Character is null");

        if (!(_controller = GetComponent<CorgiController>()))
            Debug.Log("CorgiController is null");

        characterModel = character.transform.GetChild(0);
        weaponAttachment = characterModel.GetChild(0);
        weapon = weaponAttachment.GetComponentInChildren<Weapon>();

        _targetPos = Vector3.zero;
        _targetRot = Quaternion.identity;

        userNum = character.UserNum;
        NetworkModule networkModule = NetworkModule.GetInstance();
        networkModule.AddOnEventGameUserMove(new OnGameUserMove(this));
        networkModule.AddOnEventGameUserSync(new OnGameUserSync(this));
        networkModule.AddOnEventGameEvent(new OnGameEvent(this));

        Debug.Log("RCReciever Initialized");
        MMGameEvent.Trigger("GameStart");
    }

    public void SyncUserPos(float targetPosX, float targetPosY, float xSpeed, float ySpeed, bool isFacingRight, float smoothTime = 0.1f)
    {
        if (lastFacing != isFacingRight)
        {
            lastFacing = !lastFacing;
            character.Flip();
        }
        _targetPos.x = targetPosX;
        _targetPos.y = targetPosY;

        _speed.x = xSpeed;
        _speed.y = ySpeed;
        
        transform.position = Vector3.Lerp(transform.position, _targetPos, 0.5f);
        //transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot, 0.5f);
    }
    
    private class OnGameUserMove : NetworkModule.OnGameUserMoveListener
    {
        private readonly RCReciever _rc;
        private readonly int _userNum;
        private int _targetNum;
        private float _posX;
        private float _posY;
        private float _posZ;
        private float _xSpeed;
        private float _ySpeed;
        private bool _isFacingRight;
        private float _rotZ;

        public OnGameUserMove(RCReciever rc)
        {
            Debug.Log("OnGameUserMove Constructor called");
            Debug.LogWarning(rc.gameObject.name);
            this._rc = rc;
            _userNum = _rc.userNum;
        }

        readonly char[] _delimiterChars = { ',' };
        void NetworkModule.OnGameUserMoveListener.OnGameUserMove(object data)
        {
            //Debug.Log("Recieved : " + data);
            string[] dataArray = ((string)data).Split(_delimiterChars);
            _targetNum = int.Parse(dataArray[0]);            
            //  If this is not target object, return
            if (_userNum != _targetNum)
                return;

            // Allocates recieved data
            _posX = float.Parse(dataArray[1]);
            _posY = float.Parse(dataArray[2]);
            _posZ = float.Parse(dataArray[3]);
            _xSpeed = float.Parse(dataArray[4]);
            _ySpeed = float.Parse(dataArray[5]);
            _isFacingRight = bool.Parse(dataArray[6]);
            //_rotZ = float.Parse(dataArray[7]);
            
            // Sync User position
            _rc.SyncUserPos(_posX, _posY, _xSpeed, _ySpeed, _isFacingRight);
        }
    }

    private class OnGameUserSync : NetworkModule.OnGameUserSyncListener
    {
        private readonly RCReciever _rc;
        private readonly int _userNum;
        private int _targetNum;
        private Quaternion _rot;
        public OnGameUserSync(RCReciever rc)
        {
            Debug.Log("OnGameUserSync Constructor called");
            Debug.LogWarning(rc.gameObject.name);
            _rc = rc;
            _userNum = this._rc.userNum;
        }
        
        readonly char[] _delimiterChars = { ',' }; 
        void NetworkModule.OnGameUserSyncListener.OnGameUserSync(object data)
        {
            string[] dataArray = ((string)data).Split(_delimiterChars);
            _targetNum = int.Parse(dataArray[0]);
            if (_userNum != _targetNum)
                return;
            _rot.x = float.Parse(dataArray[1]);
            _rot.y = float.Parse(dataArray[2]);
            _rot.z = float.Parse(dataArray[3]);
            _rot.w = float.Parse(dataArray[4]);
            _rc.characterModel.rotation = _rot;
            _rc.weapon.WeaponState.ChangeState(Weapon.WeaponStates.WeaponUse);
        }
    }

    private class OnGameEvent : NetworkModule.OnGameEventListener
    {
        private RCReciever rc;
        private int userNum;

        public OnGameEvent(RCReciever _rc)
        {
            rc = _rc;
            userNum = rc.userNum;
        }

        public void OnGameEventDamage(int from, int to, int amount, string option)
        {
            if (to != userNum)
                return;

            throw new System.NotImplementedException();
        }

        public void OnGameEventHealth(int num, int health)
        {
            if (num != userNum)
                return;

            throw new System.NotImplementedException();
        }

        public void OnGameEventItem(int type, int num, int action)
        {
            if (num != userNum)
                return;

            throw new System.NotImplementedException();
        }

        public void OnGameEventKill(int from, int to, string option)
        {
            if (to != userNum)
                return;

            throw new System.NotImplementedException();
        }

        public void OnGameEventObject(int num, int health)
        {
            if (num != userNum)
                return;

            throw new System.NotImplementedException();
        }

        public void OnGameEventRespawn(int num, int time)
        {
            if (num != userNum)
                return;

            throw new System.NotImplementedException();
        }
    }
}
