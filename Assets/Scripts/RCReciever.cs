using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;
using MoreMountains.CorgiEngine;

public class RCReciever : MonoBehaviour
{
    public MSB_Character character;
    public Transform weaponAttachment;
    public Weapon weapon;

    public int userNum;   
   
    void Start()
    {
        Debug.Log("RCReciever Start");
        character = GetComponent<MSB_Character>();
        weaponAttachment = character.transform.GetChild(0);
        weapon = weaponAttachment.GetComponentInChildren<Weapon>();

        Debug.Log("RCReciever Initialized");

        userNum = character.UserNum;
        //Debug.Log("User number : " + userNum);
        //Debug.Log("Weapon name : " + weapon.gameObject.name);

        NetworkModule networkModule = NetworkModule.GetInstance();
        networkModule.AddOnEventGameUserMove(new OnGameUserMove(this));
        networkModule.AddOnEventGameUserSync(new OnGameUserSync(this));
        networkModule.AddOnEventGameEvent(new OnGameEvent(this));
    }

    private void SyncUserPos(float targetPosX, float targetPosY, float xSpeed, float ySpeed, float smoothTime = 0.1f)
    {
        float newPosX = Mathf.SmoothDamp(transform.position.x, targetPosX, ref xSpeed, smoothTime);
        float newPosY = Mathf.SmoothDamp(transform.position.y, targetPosY, ref ySpeed, smoothTime);

        transform.position = new Vector3(newPosX, newPosY);
    }

    private class OnGameUserMove : NetworkModule.OnGameUserMoveListener
    {
        private RCReciever rc;
        private int userNum;
        private static bool lastFacing;

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
            rc = _rc;
            userNum = rc.userNum;
            lastFacing = _rc.character.IsFacingRight;
        }

        char[] delimiterChars = { ',' };
        void NetworkModule.OnGameUserMoveListener.OnGameUserMove(object _data)
        {
            Debug.Log("OnGameUserMove");
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
            rc.SyncUserPos(posX, posY, xSpeed, ySpeed);
        }
    }

    private class OnGameUserSync : NetworkModule.OnGameUserSyncListener
    {
        private RCReciever rc;
        private int userNum;

        public OnGameUserSync(RCReciever _rc)
        {
            rc = _rc;
            userNum = rc.userNum;
        }

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
