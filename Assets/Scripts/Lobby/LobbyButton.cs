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
    // 카메라 관련
    GameObject mainCamera;
    float cameraSize;

    Vector3 nextPos;
    bool nextOn = false;

    public bool characterChoice;

    // 배경 캐릭터
    GameObject character;

    // 큐 선택 버튼 
    GameObject soloQueueButton;
    GameObject multiQueueButton;

    // Canvas
    GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        characterChoice = false;      

        mainCamera = GameObject.Find("Main Camera");
        nextPos = new Vector3(10, 0, -10);

        character = GameObject.Find("LobbyCharacter");

        cameraSize = mainCamera.GetComponent<Camera>().orthographicSize;

        canvas = GameObject.Find("Canvas");
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

        //현재 무기 값이 SkinID 이고 스킨 값은 0으로 고정이다
        int weaponID = lobbyObj.skinID;
        int skinID = 0;

        LocalUser.Instance.localUserData.userWeapon = weaponID;
        LocalUser.Instance.localUserData.userSkin = skinID;

        NetworkModule.GetInstance().RequestSoloQueue(weaponID, skinID);
    }

    // 큐 버튼에 케릭터 정보 전달 
    void sendCharacterInfo()
    {
    }

    // 스킨 스크롤뷰 선택 버튼
    public void skinSelectButton()
    {
        // Need Animation

        // Deactive Lobby UI
        deactiveLobbyUI();

        // Active Select Skin
        canvas.GetComponent<ManageLobbyObject>().sv_Skin.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().homeButton.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().centerSlot.SetActive(true);
        // weapon select button
    }

    // 무기 스크롤뷰 선택 버튼
    public void weaponSelectButton()
    {
        // Need Animation

        // Deactive Lobby UI
        deactiveLobbyUI();

        // Active Select Weapon
        canvas.GetComponent<ManageLobbyObject>().sv_Weapon.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().homeButton.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().centerSlot.SetActive(true);
        // MAKE! skin select button
        // MAKE! game start button

    }

    // 캐릭터 선택시 뒤로가기 버튼
    public void backLobbyButton()
    {
        // Deactive Select UI or Play UI
        if(canvas.GetComponent<ManageLobbyObject>().sv_Weapon.activeSelf == true)
        {
            canvas.GetComponent<ManageLobbyObject>().sv_Weapon.SetActive(false);
            canvas.GetComponent<ManageLobbyObject>().centerSlot.SetActive(false);
        }
        else if(canvas.GetComponent<ManageLobbyObject>().sv_Skin.activeSelf == true)
        {
            canvas.GetComponent<ManageLobbyObject>().sv_Skin.SetActive(false);
            canvas.GetComponent<ManageLobbyObject>().centerSlot.SetActive(false);
        }
        else if (canvas.GetComponent<ManageLobbyObject>().single_Button.activeSelf == true)
        {
            canvas.GetComponent<ManageLobbyObject>().single_Button.SetActive(false);
            canvas.GetComponent<ManageLobbyObject>().multi_Button.SetActive(false);

            canvas.GetComponent<ManageLobbyObject>().lobbyCharacter.GetComponent<SpriteRenderer>().enabled = true;
        }
        canvas.GetComponent<ManageLobbyObject>().homeButton.SetActive(false);
        // MAKE! select button
        // MAKE! game start button

        // Active Lobby UI
        activeLobbyUI();
    }

    void deactiveLobbyUI()
    {
        // Deactive Lobby UI
        canvas.GetComponent<ManageLobbyObject>().top_Coin.SetActive(false);
        canvas.GetComponent<ManageLobbyObject>().top_Cash.SetActive(false);
        canvas.GetComponent<ManageLobbyObject>().top_Profile.SetActive(false);
        canvas.GetComponent<ManageLobbyObject>().mid_Right.SetActive(false);
        canvas.GetComponent<ManageLobbyObject>().mid_Left.SetActive(false);
        canvas.GetComponent<ManageLobbyObject>().bot_Store.SetActive(false);
        canvas.GetComponent<ManageLobbyObject>().bot_Setting.SetActive(false);
    }

    void activeLobbyUI()
    {
        // Active Lobby UI
        canvas.GetComponent<ManageLobbyObject>().top_Coin.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().top_Cash.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().top_Profile.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().mid_Right.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().mid_Left.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().bot_Store.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().bot_Setting.SetActive(true);
    }
    
    public void gamePlayButton()
    {
        deactiveLobbyUI();

        // Deactive Character
        canvas.GetComponent<ManageLobbyObject>().lobbyCharacter.GetComponent<SpriteRenderer>().enabled = false;

        // Active Play Button UI
        canvas.GetComponent<ManageLobbyObject>().single_Button.SetActive(true);
        canvas.GetComponent<ManageLobbyObject>().multi_Button.SetActive(true);

        canvas.GetComponent<ManageLobbyObject>().homeButton.SetActive(true);
    }
    
}
