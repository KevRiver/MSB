//
//  CharacterSelectButton
//  Created by 문주한 on 20/05/2019.
//
//  로비 화면에서 투명 버튼의 기능
//  버튼 클릭시 카메라 이동
//  스킨 ID와 무기 ID 값을 받아와 전달함 
//  

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using SocketIO;

public class CharacterSelectButton : MonoBehaviour, IPointerClickHandler
{
    NetworkModule networkModule;
    SocketIOComponent socket;

    // 카메라 관련
    GameObject mainCamera;
    float cameraSize;

    public GameObject scrollView_Character;
    public GameObject scrollView_Weapon;
    public GameObject joinQueue;

    Vector3 nextPos;
    bool nextOn = false;

    public bool characterChoice;

    int skinID;
    int weaponID;

    // 스크롤 뷰 관련 
    GameObject centerSlot;

    // 배경 캐릭터
    GameObject character;

    // 큐 선택 버튼 
    GameObject soloQueueButton;
    GameObject multiQueueButton;

    // 오픈되지 않은 스킨 & 무기 관련
    public GameObject rejectBackground;

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

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (nextOn)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, nextPos, Time.deltaTime * 2f);
            mainCamera.GetComponent<Camera>().orthographicSize = cameraSize;

            if (mainCamera.GetComponent<Camera>().orthographicSize < 8)
            {
                cameraSize += 0.1f;
            }

            if (mainCamera.transform.position.x > 9.9f)
            {
                nextOn = false;
                scrollView_Weapon.SetActive(true);

                centerSlot.SetActive(true);
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (characterChoice == false)
        {
            // 캐릭터 스킨 선택
            nextOn = true;
            scrollView_Character.SetActive(false);
            centerSlot.SetActive(false);
            characterChoice = true;

            // 캐릭터 이동 시작
            character.GetComponent<LobbyCharacter>().start = true;
        }
        else
        {
            // 큐 선택 팝업
            activeLoading();

            // 큐 버튼에 케릭터 정보 전달
            sendCharacterInfo();
        }

    }

    public void getSkinID(int id)
    {
        skinID = id;
    }

    public void getWeaponID(int id)
    {
        weaponID = id;
    }

    // 큐 로딩 화면 띄워주기
    void activeLoading()
    {
        joinQueue.SetActive(true);
    }

    // 큐 로딩 화면 끄기
    void inactiveLoading()
    {
        joinQueue.SetActive(false);
    }

    // 큐 버튼에 케릭터 정보 전달 
    void sendCharacterInfo()
    {
        soloQueueButton = GameObject.Find("Button_Solo");
        multiQueueButton = GameObject.Find("Button_Multi");

        soloQueueButton.GetComponent<QueueButton>().getCharacterInfo(skinID, weaponID);
        multiQueueButton.GetComponent<QueueButton>().getCharacterInfo(skinID, weaponID);
    }

    // 선택 거부 배경 켜기
    public void activeRejectBackground()
    {
        rejectBackground.SetActive(true);
    }
    // 선택 거부 배경 끄기 
    public void disableRejectBackground()
    {
        rejectBackground.SetActive(false);
    }
}
