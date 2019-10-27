using System.Collections;
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
    private bool lastFacing;

    private Vector3 targetPos;

    void Start()
    {
        Debug.Log("RCReciever Start");
        if (!(character = GetComponent<MSB_Character>()))
            Debug.Log("MSB_Character is null");

        if (!(_controller = GetComponent<CorgiController>()))
            Debug.Log("CorgiController is null");

        weaponAttachment = character.transform.GetChild(0);
        weapon = weaponAttachment.GetComponentInChildren<Weapon>();

        targetPos = Vector3.zero;

        userNum = character.UserNum;
        lastFacing = character.IsFacingRight;
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
        if (lastFacing != isFacingRight)
        {
            lastFacing = !lastFacing;
            character.Flip();
        }

        targetPos.x = targetPosX;
        targetPos.y = targetPosY;

        transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
    }

    private class OnGameUserMove : NetworkModule.OnGameUserMoveListener
    {
        private RCReciever rc;
        private int userNum;
        private bool lastFacing;

        // Target number
        int targetNum;
        
        // For position sync
        float posX;
        float posY;
        float posZ;
        float xSpeed;
        float ySpeed;

        // For facing direction sync
        bool isFacingRight;

        public OnGameUserMove(RCReciever _rc)
        {
            Debug.Log("OnGameUserMove Constructor called");
            Debug.LogWarning(_rc.gameObject.name);
            rc = _rc;
            userNum = rc.userNum;
            lastFacing = _rc.character.IsFacingRight;
        }

        char[] delimiterChars = { ',' };
        void NetworkModule.OnGameUserMoveListener.OnGameUserMove(object _data)
        {
            string[] dataArray = ((string)_data).Split(delimiterChars);
            targetNum = int.Parse(dataArray[0]);            
            //  If this is not target object, return
            if (userNum != targetNum)
                return;

            // Allocates recieved data

            posX = float.Parse(dataArray[1]);
            posY = float.Parse(dataArray[2]);
            posZ = float.Parse(dataArray[3]);

            xSpeed = float.Parse(dataArray[4]);
            ySpeed = float.Parse(dataArray[5]);

            isFacingRight = bool.Parse(dataArray[6]);

            // Sync User Facing
            if (lastFacing != isFacingRight)
            {
                lastFacing = !lastFacing;
                rc.character.Flip();
            }
            // Sync User position
            rc.SyncUserPos(posX, posY, xSpeed, ySpeed, isFacingRight);
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
