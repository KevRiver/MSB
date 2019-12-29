//
//  LobbyButton
//  Created by 문주한 on 26/07/2019.
//
//  Select Skin Button
//  Select Weapon Button
//  Profile Button
//  Game Join Button
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using MSBNetwork;

public class LobbyButton : MonoBehaviour
{
    // 배경 캐릭터
    GameObject character;

    // 큐 선택 버튼 
    GameObject soloQueueButton;
    GameObject multiQueueButton;

    // Canvas
    ManageLobbyObject canvas;

    // Animation
    bool characterSelectWindowBool = false;

    void Start()
    {
        character = GameObject.Find("LobbyCharacter");

        canvas = FindObjectOfType<ManageLobbyObject>();
    }

    // 큐 로딩 화면 띄워주기
    void activeLoading()
    {
    }

    // 큐 로딩 화면 끄기
    void deactiveLoading()
    {
    }

    public void RequestGameQueue(int mode)
    {
        //현재 정보 담고
        //담은 정보로 RequestSoloQueue;
        ManageLobbyObject lobbyObj = FindObjectOfType<ManageLobbyObject>();
        Debug.LogWarning("LobbyObj SkinID : " + lobbyObj.skinID);

        //현재 무기 값이 WeaponID 이고 스킨 값은 SkinID이다 WeaponID = SkinID
        int weaponID = lobbyObj.weaponID;
        int skinID = lobbyObj.skinID;

        LocalUser.Instance.localUserData.userWeapon = weaponID;
        LocalUser.Instance.localUserData.userSkin = skinID;
        LocalUser.Instance.SetWeaponID(weaponID);
        LocalUser.Instance.SetSkinID(skinID);
        if(mode == 0)
            NetworkModule.GetInstance().RequestGameSoloQueue(weaponID, skinID);
        else if(mode == 1)
            NetworkModule.GetInstance().RequestGameTeamQueue(weaponID,skinID);
        else
        {
            Debug.LogWarning("set game mode");
        }
    }

    // 스킨 선택 버튼
    public void characterSelectWindow()
    {
        // Need Animation
        characterSelectTransition();
    }

    void characterSelectTransition()
    {
        if (characterSelectWindowBool)
        {
            characterSelectWindowBool = false;
            canvas.bot_PlayButton.SetActive(true);
            canvas.GetComponent<Animator>().SetTrigger("CharacterSelectWindowOutTransition");
        }
        else
        {
            characterSelectWindowBool = true;
            canvas.bot_PlayButton.SetActive(false);
            canvas.GetComponent<Animator>().SetTrigger("CharacterSelectWindowInTransition");
        }
    }

    // Home Button
    void backLobbyButton()
    {
        canvas.GetComponent<Animator>().SetTrigger("PlayLobbyOutTransition");
        canvas.GetComponent<ManageLobbyObject>().lobbyCharacter.GetComponent<SpriteRenderer>().enabled = true;
    }

    
    void gamePlayButton()
    {
        canvas.GetComponent<Animator>().SetTrigger("PlayLobbyInTransition");
        canvas.lobbyCharacter.GetComponent<SpriteRenderer>().enabled = false;
    }
    
}
