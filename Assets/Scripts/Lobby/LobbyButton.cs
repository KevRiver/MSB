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
using UnityEngine.UI;
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

    // Lobby Character
    LobbyCharacter lobbyCharacter;

    // Animation
    bool characterSelectWindowBool = false;

    void Start()
    {
        character = GameObject.Find("LobbyCharacter");

        canvas = FindObjectOfType<ManageLobbyObject>();
        lobbyCharacter = FindObjectOfType<LobbyCharacter>();
    }

    // 큐 로딩 화면 띄워주기
    public void activeLoading(int _mode)
    {
        canvas.queueLoading = true;

        canvas.homeButton.GetComponent<Image>().sprite = canvas.backButtonSprite;

        canvas.soloButton.SetActive(false);
        canvas.multiButton.SetActive(false);

        canvas.background.GetComponent<Animator>().SetTrigger("QueueLoading");
        canvas.loadingCharacter.SetActive(true);
        if(_mode == 0)
        {
            canvas.loadingCharacter.GetComponent<Animator>().SetTrigger("Solo");
        }
        else
        {
            canvas.loadingCharacter.GetComponent<Animator>().SetTrigger("Multi");
        }
    }

    // 큐 로딩 화면 끄기
    void deactiveLoading()
    {
        canvas.queueLoading = false;

        canvas.homeButton.GetComponent<Image>().sprite = canvas.homeButtonSprite;

        canvas.soloButton.SetActive(true);
        canvas.multiButton.SetActive(true);

        canvas.background.GetComponent<Animator>().SetTrigger("Lobby");
        canvas.loadingCharacter.SetActive(false);
    }

    // 큐 잡힘 애니메이션 추려
    public void matchedLoading(int _room)
    {
        canvas.homeButton.SetActive(false);
        
        canvas.loadingCharacter.GetComponent<QueueLoadingObject>().getRoom(_room);
        canvas.loadingCharacter.GetComponent<Animator>().SetTrigger("Finish");
    }


    public void RequestGameQueue(int mode)
    {
        activeLoading(mode);
        canvas.networkManager.getLobbyButton(this);

        //현재 정보 담고
        //담은 정보로 RequestSoloQueue;
        Debug.LogWarning("Canvas SkinID : " + canvas.skinID);

        //현재 무기 값이 WeaponID 이고 스킨 값은 SkinID이다 WeaponID = SkinID
        int weaponID = canvas.weaponID;
        int skinID = canvas.skinID;

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
        if (canvas.queueLoading)
        {
            deactiveLoading();
        }
        else
        {
            canvas.GetComponent<Animator>().SetTrigger("PlayLobbyOutTransition");
        }

    }
        
    void gamePlayButton()
    {
        canvas.GetComponent<Animator>().SetTrigger("PlayLobbyInTransition");
        lobbyCharacter.GetComponent<SpriteRenderer>().enabled = false;

        lobbyCharacter.orangStop();
    }

    public void endPlayLobbyOutTransition()
    {
        lobbyCharacter.GetComponent<SpriteRenderer>().enabled = true;
    }
    
}
