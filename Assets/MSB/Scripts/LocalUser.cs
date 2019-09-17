using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class LocalUser : MonoBehaviour
{
    private static LocalUser _instance;
    public static LocalUser Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LocalUser>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "LocalUser";
                    _instance = obj.AddComponent<LocalUser>();
                }
            }
            return _instance;         
        }
    }

    public ClientUserData localUserData;

    void Awake()
    {       
        localUserData = new ClientUserData();
        DontDestroyOnLoad(gameObject);
    }
   
    public void DebugLocalUserData()
    {
        Debug.Log("DebugLocalUserData called");
        Debug.Log(localUserData.userID);
        Debug.Log(localUserData.userNick);
        Debug.Log(localUserData.userNumber);
        Debug.Log(localUserData.userMoney);
        Debug.Log(localUserData.userCash);
        Debug.Log(localUserData.userRank);
        Debug.Log(localUserData.userWeapon);
        Debug.Log(localUserData.userSkin);
    }

    public void SetWeaponID(int _weaponID)
    {
        localUserData.userWeapon = _weaponID;
        Debug.Log(localUserData.userWeapon);
    }

    public void SetSkinID(int _skinID)
    {
        localUserData.userSkin = _skinID;
        Debug.Log(localUserData.userSkin);
    }
}
