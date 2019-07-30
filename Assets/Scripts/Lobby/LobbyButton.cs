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
using SocketIO;

public class LobbyButton : MonoBehaviour
{
    NetworkModule networkModule;
    SocketIOComponent socket;

    // 카메라 관련
    GameObject mainCamera;
    float cameraSize;
    public GameObject joinQueue;

    Vector3 nextPos;
    bool nextOn = false;

    public bool characterChoice;


    // 스크롤 뷰 관련 
    GameObject centerSlot;

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

        centerSlot = GameObject.Find("CenterSlot");

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
        joinQueue.SetActive(true);
    }

    // 큐 로딩 화면 끄기
    void deactiveLoading()
    {
        joinQueue.SetActive(false);
    }

    // 큐 버튼에 케릭터 정보 전달 
    void sendCharacterInfo()
    {
        soloQueueButton = GameObject.Find("Button_Solo");
        multiQueueButton = GameObject.Find("Button_Multi");

        soloQueueButton.GetComponent<QueueButton>().getCharacterInfo(canvas.GetComponent<ManageLobbyObject>().skinID, canvas.GetComponent<ManageLobbyObject>().weaponID);
        multiQueueButton.GetComponent<QueueButton>().getCharacterInfo(canvas.GetComponent<ManageLobbyObject>().skinID, canvas.GetComponent<ManageLobbyObject>().weaponID);
    }

    // 스킨 선택 버튼
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

    // 무기 선택 버튼
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
        // Deactive Select UI
        if(canvas.GetComponent<ManageLobbyObject>().sv_Weapon.activeSelf == true)
        {
            canvas.GetComponent<ManageLobbyObject>().sv_Weapon.SetActive(false);
        }
        else
        {
            canvas.GetComponent<ManageLobbyObject>().sv_Skin.SetActive(false);
        }
        canvas.GetComponent<ManageLobbyObject>().homeButton.SetActive(false);
        canvas.GetComponent<ManageLobbyObject>().centerSlot.SetActive(false);
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


}
