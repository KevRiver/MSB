using System.Collections;
using System.Collections.Generic;
using MSBNetwork;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public enum MatchType { Solo,Team }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButtonClicked()
    {
        //로컬 플레이어가 선택한 무기와 스킨 정보를 저장합니다
        //ClientUserData data = FindObjectOfType<LocalUser>().localUserData;
        ClientUserData data = LocalUser.Instance.localUserData;

        //로컬 플레이어의 WeaponID, SkinID를 RequestMatch의 파라미터로 넘깁니다 
        RequestMatch(MatchType.Solo, data.userWeapon, data.userSkin);
    }

    public void RequestMatch(MatchType _matchType,int _weaponID,int _skinID)
    {
        if (_matchType == MatchType.Solo)
        {
            NetworkModule.GetInstance().RequestSoloQueue(_weaponID, _skinID);
        }
    }
}
