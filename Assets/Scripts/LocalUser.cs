using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class LocalUser : PersistentSingleton<LocalUser>
{
    public ClientUserData localUserData;

    protected override void Awake()
    {
        //Debug.Log("LocalUser Awake");
        base.Awake();
        _instance.name = "LocalUser";
        //localUserData = new ClientUserData();
    }

    public void DebugLocalUserData()
    {
        //Debug.Log("DebugLocalUserData called");
        Debug.Log("ID : " + localUserData.userID);
        Debug.Log("Nick : " + localUserData.userNick);
        Debug.Log("Num : " + localUserData.userNumber);
        Debug.Log("Money : " + localUserData.userMoney);
        Debug.Log("Cash : " + localUserData.userCash);
        Debug.Log("Rank : " + localUserData.userRank);
        Debug.Log("Weapon : " + localUserData.userWeapon);
        Debug.Log("Skin : " + localUserData.userSkin);
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
