using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;

public class ButtonScripts : MonoBehaviour
{
    private NetworkModule networkModule;
    private LocalUser localUser;

    // Start is called before the first frame update
    void Start()
    {
        networkModule = NetworkModule.GetInstance();
        localUser = LocalUser.Instance;
    }

    public void CharacterSelect(int index)
    {
        localUser.SetWeaponID(index);
    }

    public void RequestSoloMatch()
    {
        networkModule.RequestSoloQueue(localUser.localUserData.userWeapon, localUser.localUserData.userSkin);
        //Debug.Log("Request Solo Queue");
    }
}
