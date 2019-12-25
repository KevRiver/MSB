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


    public void RequestSoloQueue()
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

        NetworkModule.GetInstance().RequestGameSoloQueue(weaponID, skinID);
    }

    public void RequestTeamQueue()
    {
        ManageLobbyObject lobbyObj = FindObjectOfType<ManageLobbyObject>();
        Debug.LogWarning("LobbyObj SkinID : " + lobbyObj.skinID);

        //현재 무기 값이 WeaponID 이고 스킨 값은 SkinID이다 WeaponID = SkinID
        int weaponID = lobbyObj.weaponID;
        int skinID = lobbyObj.skinID;

        LocalUser.Instance.localUserData.userWeapon = weaponID;
        LocalUser.Instance.localUserData.userSkin = skinID;
        LocalUser.Instance.SetWeaponID(weaponID);
        LocalUser.Instance.SetSkinID(skinID);

        NetworkModule.GetInstance().RequestGameTeamQueue(weaponID, skinID);
    }

    // 큐 버튼에 케릭터 정보 전달 
    void sendCharacterInfo()
    {
    }

    // 스킨 선택 버튼
    public void characterSelectWindow()
    {
        // Need Animation
        characterSelectTransition();

    }

    void characterSelectTransition()
    {
        if(canvas.mainLobby.transform.localPosition.y != 100)
        {
            canvas.bot_PlayButton.SetActive(false);
            canvas.mainLobby.transform.localPosition = new Vector2(0, 100);
        }
        else
        {
            canvas.bot_PlayButton.SetActive(true);
            canvas.mainLobby.transform.localPosition = new Vector2(0, 0);
        }
    }

    // 캐릭터 선택시 뒤로가기 버튼
    public void backLobbyButton()
    {
        canvas.homeButton.SetActive(false);

        if (canvas.playLobby.transform.localPosition.x == 0)
        {
            canvas.lobbyCharacter.GetComponent<SpriteRenderer>().enabled = true;
            canvas.mainLobby.transform.localPosition = new Vector2(0, 0);
            canvas.playLobby.transform.localPosition = new Vector2(800, 0);
        }
        

        // MAKE! select button
        // MAKE! game start button

    }

    
    public void gamePlayButton()
    {
        //deactiveLobbyUI();
        //Debug.Log(canvas.playLobby.transform.localPosition);
        playButton_transition();

        // Deactive Character
        canvas.lobbyCharacter.GetComponent<SpriteRenderer>().enabled = false;

        canvas.homeButton.SetActive(true);
    }

    void playButton_transition()
    {
        canvas.mainLobby.transform.localPosition = new Vector2(-800, 0);
        canvas.playLobby.transform.localPosition = new Vector2(0, 0);
    }


    
}
