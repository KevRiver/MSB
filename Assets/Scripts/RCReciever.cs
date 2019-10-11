using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;

namespace MoreMountains.CorgiEngine
{
    public struct MSBActionEvent
    {
        public int userNumber;
        public Vector3 shootAngle;

        public MSBActionEvent(int _userNumber,Vector3 _shootAngle)
        {
            userNumber = _userNumber;
            shootAngle = _shootAngle;
        }

        static MSBActionEvent e;
        public static void Trigger(int _userNumber, Vector3 _shootAngle)
        {
            e.userNumber = _userNumber;
            e.shootAngle = _shootAngle;
            MMEventManager.TriggerEvent(e);
        }
    }

    public class RCReciever : MonoBehaviour,                          
                            NetworkModule.OnGameUserMoveListener,
                            NetworkModule.OnGameUserSyncListener
                            
    {
        MSB_Character character;
        Transform weaponAttachment;
        Weapon weapon;

        private int userNum;

        // Sync frequency
        const float smoothTime = 0.1f;

        // For position synchronizing
        Vector3 targetPos;
        float xSpeed;
        float ySpeed;
        private bool isGrounded;
        private bool isFacingRight;

        // Start is called before the first frame update
        void Start()
        {
            character = GetComponent<MSB_Character>();
            weaponAttachment = character.transform.GetChild(0);
            weapon = weaponAttachment.GetComponentInChildren<Weapon>();

            userNum = character.UserNum;
            Debug.Log("User number : " + userNum);
            //Debug.Log("Weapon name : " + weapon.gameObject.name);
        }

        private void SyncUserPos()
        {
            float newPosX = Mathf.SmoothDamp(transform.position.x, targetPos.x, ref xSpeed, smoothTime);
            float newPosY = Mathf.SmoothDamp(transform.position.y, targetPos.y, ref ySpeed, smoothTime);

            transform.position = new Vector3(newPosX, newPosY);
        }

        char[] delimiterChars = { ',' };
        public void OnGameUserMove(object _data)
        {
            string[] dataArray = ((string)_data).Split(delimiterChars);
            int targetNum = int.Parse(dataArray[0]);

            //  If this is not target object return
            if (userNum != targetNum)
                return;

            // Allocates recieved data
            float _posX = float.Parse(dataArray[1]);
            float _posY = float.Parse(dataArray[2]);
            float _posZ = float.Parse(dataArray[3]);
            targetPos = new Vector3(_posX, _posY, _posZ);

            float _xSpeed = float.Parse(dataArray[4]);
            float _ySpeed = float.Parse(dataArray[5]);
            xSpeed = _xSpeed;
            ySpeed = _ySpeed;

            bool _isGrounded = bool.Parse(dataArray[6]);
            isGrounded = _isGrounded;

            bool _isFacingRight = bool.Parse(dataArray[7]);
            isFacingRight = _isGrounded;

            // Sync user position
            SyncUserPos();
        }

        public void OnGameUserSync(object _data)
        {
            throw new System.NotImplementedException();
        }
    }
}
