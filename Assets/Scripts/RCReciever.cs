using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;
using MoreMountains.CorgiEngine;
using UnityEngine.Serialization;
using UnityScript.Steps;

public class RCReciever : MonoBehaviour,MMEventListener<MMGameEvent>
{
    private bool isInitialized = false;
    public MSB_Character character;
    private CorgiController _controller;
    public Transform characterModel;
    public Transform weaponAttachment;
    public Weapon weapon;
    public Health health;
    public int userNum;
    // For position sync
    public float posX = 0;
    public float posY = 0;
    public float posZ = 0;
    public float xSpeed = 0;
    public float ySpeed = 0;
    // For facing direction sync
    public bool isFacingRight;
    public bool isGrounded;
    public bool lastFacing;

    private Vector3 _curPos;
    private Vector3 _targetPos;
    private Vector2 _speed;
    private Quaternion _targetRot;
    private bool onSync = false;
    void Start()
    {
        if (!(character = GetComponent<MSB_Character>()))
            Debug.Log("MSB_Character is null");

        if (!(_controller = GetComponent<CorgiController>()))
            Debug.Log("CorgiController is null");

        characterModel = character.transform.GetChild(0);
        weaponAttachment = characterModel.GetChild(0);
        weapon = weaponAttachment.GetComponentInChildren<Weapon>();
        //health = GetComponent<Health>();
        
        _targetPos = transform.position;
        _targetRot = transform.rotation;

        userNum = character.UserNum;
        
        NetworkModule.GetInstance().AddOnEventGameUserMove(new OnGameUserMove(this));
        NetworkModule.GetInstance().AddOnEventGameUserSync(new OnGameUserSync(this));

        isInitialized = true;
        Debug.Log("RCReciever Initialized");
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
    
    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        if (!isInitialized)
            return;
        onSync = true;
        StartCoroutine(SyncUserPos());
    }
    
    private void OnDisable()
    {
        if (onSync)
        {
            onSync = false;
            StopCoroutine(SyncUserPos());
        }

        this.MMEventStopListening<MMGameEvent>();
    }

    private float _newPosX;
    private float _newPosY;
    private IEnumerator SyncUserPos()
    {
        while (true)
        {
            _curPos = transform.position;
            _newPosX = Mathf.SmoothDamp(_curPos.x, _targetPos.x, ref _speed.x, 0.1f);
            _newPosY = Mathf.SmoothDamp(_curPos.y, _targetPos.y, ref _speed.y, 0.1f);
            
            /*if(_controller.State.IsGrounded)
                _controller.SetVerticalForce(0f);*/
            transform.position = new Vector3(_newPosX, _curPos.y);
            _controller.SetVerticalForce(_speed.y);
            yield return null;
        }
    }

    public void SetTargetPos(float targetPosX, float targetPosY, float xSpeed, float ySpeed, bool isFacingRight, float smoothTime = 0.1f)
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
        
        //transform.position = Vector3.Lerp(transform.position, _targetPos, 0.5f);
        //transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot, 0.5f);
    }

    public void AttackSync(Quaternion rot)
    {
        characterModel.rotation = rot;
        weapon.WeaponState.ChangeState(Weapon.WeaponStates.WeaponUse);
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
        private float _rotZ;

        public OnGameUserMove(RCReciever rc)
        {
            Debug.Log("OnGameUserMove Constructor called");
            //Debug.LogWarning(rc.gameObject.name);
            _rc = rc;
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
            _rc.SetTargetPos(_posX, _posY, _xSpeed, _ySpeed, _isFacingRight);
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
            Debug.Log("OnGameUserSync Constructor called");
            //Debug.LogWarning(rc.gameObject.name);
            _rc = rc;
            _userNum = _rc.userNum;
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
            _rc.AttackSync(_rot);
        }
    }

   /* private class OnGameEvent : NetworkModule.OnGameEventListener
    {
        private RCReciever _rc;
        private int _userNum;

        public OnGameEvent(RCReciever rc)
        {
            _rc = rc;
            _userNum = _rc.userNum;
        }

        public void OnGameEventDamage(int from, int to, int amount, string option)
        {
            Debug.LogWarning("DamageEvent Occured - from : " + from + " to : " + to + " damage : " + amount);
        }

        public void OnGameEventHealth(int num, int health)
        {
            if (num != _userNum)
                return;
            Debug.LogWarning(num + "'s Health Changed");
            _rc.health.ChangeHealth(health);
        }

        public void OnGameEventItem(int type, int num, int action)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameEventKill(int from, int to, string option)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameEventObject(int num, int health)
        {
            if (num != _userNum)
                return;

            throw new System.NotImplementedException();
        }

        public void OnGameEventRespawn(int num, int time)
        {
            if (num != _userNum)
                return;

            throw new System.NotImplementedException();
        }
    }*/

    
}
