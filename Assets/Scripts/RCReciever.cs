﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;
using MoreMountains.CorgiEngine;

public class RCReciever : MonoBehaviour
{
    public MSB_Character character;
    private CorgiController _controller;
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
    private bool _lastFacing;

    private Vector3 _targetPos;

    void Start()
    {
        Debug.Log("RCReciever Start");
        if (!(character = GetComponent<MSB_Character>()))
            Debug.Log("MSB_Character is null");

        if (!(_controller = GetComponent<CorgiController>()))
            Debug.Log("CorgiController is null");

        weaponAttachment = character.transform.GetChild(0);
        weapon = weaponAttachment.GetComponentInChildren<Weapon>();

        _targetPos = Vector3.zero;

        userNum = character.UserNum;
        _lastFacing = character.IsFacingRight;
        NetworkModule networkModule = NetworkModule.GetInstance();
        networkModule.AddOnEventGameUserMove(new OnGameUserMove(this));
        //networkModule.AddOnEventGameUserSync(new OnGameUserSync(this));
        networkModule.AddOnEventGameEvent(new OnGameEvent(this));

        Debug.Log("RCReciever Initialized");
        MMGameEvent.Trigger("GameStart");
    }

    public void SyncUserPos(float targetPosX, float targetPosY, float xSpeed, float ySpeed, bool isFacingRight, float smoothTime = 0.1f)
    {
        //Debug.Log("SyncUserPos Called");
        if (_lastFacing != isFacingRight)
        {
            _lastFacing = !_lastFacing;
            character.Flip();
        }

        _targetPos.x = targetPosX;
        _targetPos.y = targetPosY;

        transform.position = Vector3.Lerp(transform.position, _targetPos, 0.5f);
    }

    private class OnGameUserMove : NetworkModule.OnGameUserMoveListener
    {
        private readonly RCReciever _rc;
        private readonly int _userNum;
        private bool _lastFacing;

        // Target number
        int _targetNum;
        
        // For position sync
        float _posX;
        float _posY;
        float _posZ;
        float _xSpeed;
        float _ySpeed;

        // For facing direction sync
        bool _isFacingRight;

        public OnGameUserMove(RCReciever rc)
        {
            Debug.Log("OnGameUserMove Constructor called");
            Debug.LogWarning(rc.gameObject.name);
            this._rc = rc;
            _userNum = this._rc.userNum;
            _lastFacing = rc.character.IsFacingRight;
        }

        readonly char[] _delimiterChars = { ',' };
        void NetworkModule.OnGameUserMoveListener.OnGameUserMove(object data)
        {
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

            // Sync User Facing
            if (_lastFacing != _isFacingRight)
            {
                _lastFacing = !_lastFacing;
                _rc.character.Flip();
            }
            // Sync User position
            _rc.SyncUserPos(_posX, _posY, _xSpeed, _ySpeed, _isFacingRight);
        }
    }

    private class OnGameUserSync : NetworkModule.OnGameUserSyncListener
    {
        void NetworkModule.OnGameUserSyncListener.OnGameUserSync(object _data)
        {
            throw new System.NotImplementedException();
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
