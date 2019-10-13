using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using MSBNetwork;

public class LocalUser : PersistentSingleton<LocalUser>
{
    public ClientUserData localUserData;

    protected override void Awake()
    {
        base.Awake();
        localUserData = new ClientUserData();
        localUserData.userID = "Qon";
        localUserData.userNick = "Qon";
        localUserData.userNumber = 0;
        localUserData.userWeapon = 0;
        localUserData.userSkin = 0;
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
