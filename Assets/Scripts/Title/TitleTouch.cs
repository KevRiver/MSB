using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SocketIO;
using System;

public class TitleTouch : MonoBehaviour, IPointerClickHandler
{
	NetworkModule networkModule;
	SocketIOComponent socket;

	public GameObject canvasObject;
	ToastAlerter toastModule;
	LoadSplash splashModule;

	public GameObject popup;
	public GameObject centerPos;
	public PopupButton nicknameButton;
	Vector2 centerV2;

	string strPlayerName = "tiram6sue";

	// Start is called before the first frame update
	void Start()
	{
		toastModule = canvasObject.GetComponent<ToastAlerter>();
		splashModule = canvasObject.GetComponent<LoadSplash>();

		networkModule = GameObject.Find("NetworkModule").GetComponent<NetworkModule>();

		centerV2 = centerPos.transform.position;
		nicknameButton = GameObject.Find("NicknameButton").GetComponent<PopupButton>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		splashModule.showLoadSplash(100, "로그인 중입니다", LoadSplash.SPLASH_TYPE.TYPE_SHORT);
		Invoke("attemptLogin", (float)100 / 1000);
	}

	public void attemptLogin()
	{
		socket = networkModule.get_socket();
		JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);

		userData.AddField("userID", strPlayerName);
		userData.AddField("userPW", "");

		socket.Off("loginSuccess", null);
		socket.Off("loginFail", null);
		socket.On("loginSuccess", loginSuccess);
		socket.On("loginFail", loginFail);
		socket.Emit("login", userData);
	}

	public void loginSuccess(SocketIOEvent obj)
	{
		User localPlayer = new User();
		Debug.Log("Login Success!" + obj);
		JSONObject data = obj.data;
		int userNumber = (int)data[0].n;
		string userID = (string)data[1].str;
		string userName = (string)data[2].str;
		int userRank = (int)data[3].n;
		int userBlocked = (int)data[4].n;
		int userMoney = (int)data[5].n;
		int userCash = (int)data[6].n;

		try
		{
			localPlayer.Num = userNumber;
			localPlayer.Id = userID;
			localPlayer.Name = userName;
			localPlayer.Rank = userRank;
			localPlayer.Blocked = userBlocked;
			localPlayer.Money = userMoney;
			localPlayer.Gold = userCash;
		}
		catch (Exception err)
		{
			userNumber = -1;
			Debug.Log("Error : " + err);
		}

		Debug.Log("User number is : " + localPlayer.Num);
		toastModule.showToast(localPlayer.Name + "님 환영합니다!", ToastAlerter.MESSAGE_TYPE.TYPE_GREEN, 1);
		GameObject.Find("LocalPlayer").GetComponent<LocalPlayer>().setLocalPlayer(localPlayer);

		//로그인 버튼 -> 로그 아웃 버튼, 큐 입장 버튼 생성
	}

	public void loginFail(SocketIOEvent obj)
	{
		Debug.Log("Login Fail" + obj);
		JSONObject data = obj.data;
		int result = (int)data[0].n;
		int blocked = (int)data[1].n;
		string error = (string)data[2].str;
		switch (result)
		{
			case 0:
				Debug.Log("No User Data : Register!");
				showPopup();
				break;
			case 1:
				Debug.Log("PASSWORD NOT MATCH");
				toastModule.showToast("PASSWORD NOT MATCH", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
				break;
			case 2:
				Debug.Log("YOU ARE BANNED");
				toastModule.showToast("YOU ARE BANNED", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
				break;
			case 3:
				Debug.Log("Unknown Error : " + error);
				toastModule.showToast("ERROR: " + error, ToastAlerter.MESSAGE_TYPE.TYPE_RED, 1);
				break;

		}
	}

	public void showPopup()
	{
		GameObject.Find("ClickText").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
		popup.transform.position = centerV2;
	}

	public void attemptRegister(string nickname)
	{
		JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);

		userData.AddField("userID", strPlayerName);
		userData.AddField("userPW", "");
		userData.AddField("userName", nickname);

		socket.Off("registerResult", null);
		socket.On("registerResult", registerResult);
		socket.Emit("register", userData);
	}

	public void registerResult(SocketIOEvent obj)
	{
		Debug.Log("Register Result" + obj);
		JSONObject data = obj.data;
		int resultCode = (int)data[0].n;
		string resultMessage = (string)data[1].str;
		nicknameButton.attemptRegisterResult(resultCode, resultMessage);
		if (resultCode == 0)
		{
			Debug.Log("Register Finished!");
			toastModule.showToast("REGISTER SUCCESS", ToastAlerter.MESSAGE_TYPE.TYPE_GREEN, 1);
		}
	}

}