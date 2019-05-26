//
//  SelectButton
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

public class SelectButton : MonoBehaviour, IPointerClickHandler
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



    // Start is called before the first frame update
    void Start()
    {

		networkModule = GameObject.Find("NetworkModule").GetComponent<NetworkModule>();

		characterChoice = false;

        centerSlot = GameObject.Find("CenterSlot");

        characterChoice = false;

        mainCamera = GameObject.Find("Main Camera");
        nextPos = new Vector3(10, 0, -10);

        character = GameObject.Find("LobbyPlayer");

        cameraSize = mainCamera.GetComponent<Camera>().orthographicSize;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (nextOn)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, nextPos, Time.deltaTime * 2f);
            mainCamera.GetComponent<Camera>().orthographicSize = cameraSize;

            if(mainCamera.GetComponent<Camera>().orthographicSize < 8)
            {
                cameraSize += 0.1f;
            }

            if (mainCamera.transform.position.x > 9.9f)
            {
                nextOn = false;
                scrollView_Weapon.SetActive(true);

                centerSlot.SetActive(true);

                Debug.Log(mainCamera.GetComponent<Camera>().orthographicSize);
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(characterChoice == false)
        { 
            // 캐릭터 스킨 선택
            nextOn = true;
            scrollView_Character.SetActive(false);
            centerSlot.SetActive(false);
            characterChoice = true;

            // 캐릭터 이동 시작
            character.GetComponent<LobbyPlayer>().start = true;
        }
        else
        {
            // 무기 선택
            sendSkinWeaponID();

            // 큐 선택 팝업 
            //activeLoading();

            // 큐 선택 팝업 
            //inactiveLoading();

            // 클라이언트 씬 전환 
            //SceneManager.LoadScene("GameScene");
        }

    }

    public void sendSkinWeaponID()
    {
		socket = networkModule.get_socket();
		// SkinID and WeaponID 전송
		Debug.Log("Skin ID : " + skinID);
        Debug.Log("Weapon ID : " + weaponID);

		socket.On("soloMatched", soloMatched);

		JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
		data.AddField("skinIndex", skinID);
		data.AddField("weaponIndex", weaponID);
		socket.Emit("matchMakeSolo", data);
    }

	public void soloMatched(SocketIOEvent e)
	{
		Debug.Log("soloMatched : " + e);
		JSONObject data = e.data;

		int gameRoomIndex = -1;
		try
		{
			gameRoomIndex = (int)data[0].n;
		}
		catch (Exception err) { };

		int position = -1;
		try
		{
			position = (int)data[1].n;
		}
		catch (Exception err) { };

		JSONObject userListJSON;
		try
		{
			userListJSON = data[2];

			GameObject.Find("UserData").GetComponent<UserData>().clearUserData();
			int i = 0;
			foreach (JSONObject userData in userListJSON.list)
			{
				//Debug.Log("Queue Handler log" + ++i);
				User player = new User();
				player.Num = (int)userData[0].n;
				player.Id = userData[1].str;
				GameObject.Find("UserData").GetComponent<UserData>().addUser(player);
			}

			SceneManager.LoadScene("GameScene");
		}
		catch (Exception err) { }

		GameObject.Find("UserData").GetComponent<UserData>().setRoomIndex(gameRoomIndex);
		GameObject.Find("UserData").GetComponent<UserData>().setPlayerIndex(position);
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
}
