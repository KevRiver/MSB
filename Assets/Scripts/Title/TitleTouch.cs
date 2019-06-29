using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

	public GameObject LoginButtonAccount;
	public GameObject LoginButtonGoogle;

	private PopupButton popupButton;


	Vector2 centerV2;

	public Boolean IS_LOGIN_AVAILABLE = true;

	public enum LOGIN_TYPE
	{
		LOGIN_ACCOUNT, LOGIN_GOOGLE
	}

	public LOGIN_TYPE loginType;


	// Start is called before the first frame update
	void Start()
	{
		toastModule = canvasObject.GetComponent<ToastAlerter>();
		splashModule = canvasObject.GetComponent<LoadSplash>();

		networkModule = GameObject.Find("NetworkModule").GetComponent<NetworkModule>();

		centerV2 = centerPos.transform.position;
		popupButton = GameObject.Find("Popup").GetComponent<PopupButton>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!IS_LOGIN_AVAILABLE) return;

		if (loginType == LOGIN_TYPE.LOGIN_ACCOUNT)
		{
			showAccountHolder();
		}
		if (loginType == LOGIN_TYPE.LOGIN_GOOGLE)
		{
			// TODO
		}
		
	}

	public void attemptLoginAccount()
	{
		string inputID = GameObject.Find("IDinput").GetComponent<InputField>().text;
		if (inputID.Trim().Length == 0)
		{
			toastModule.showToast("ENTER ID", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
			return;
		}
		if (inputID.Trim().Length < 2)
		{
			toastModule.showToast("ID TOO SHORT", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
			return;
		}
		attemptLogin(inputID);
	}

	public void attemptLogin(string userID)
	{
		LoginButtonAccount.GetComponent<TitleTouch>().IS_LOGIN_AVAILABLE = false;
		LoginButtonGoogle.GetComponent<TitleTouch>().IS_LOGIN_AVAILABLE = false;

		socket = networkModule.get_socket();
		JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);

		userData.AddField("userID", userID);
		userData.AddField("userPW", "");
		userData.AddField("userName", userID);

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

		StartCoroutine(switchLobbyScene());
	}

	IEnumerator switchLobbyScene()
	{
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene("Lobby");
	}

	public void loginFail(SocketIOEvent obj)
	{
		LoginButtonAccount.GetComponent<TitleTouch>().IS_LOGIN_AVAILABLE = true;
		LoginButtonGoogle.GetComponent<TitleTouch>().IS_LOGIN_AVAILABLE = true;
		Debug.Log("Login Fail" + obj);
		JSONObject data = obj.data;
		int result = (int)data[0].n;
		int blocked = (int)data[1].n;
		string error = (string)data[2].str;
		switch (result)
		{
			case 0:
				Debug.Log("No User Data : Register!");
				//showPopup();
				toastModule.showToast("REGISTER FIRST!", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
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

	public void showAccountHolder()
	{
		LoginButtonAccount.transform.localScale = new Vector3(0,0,0);
		LoginButtonGoogle.transform.localScale = new Vector3(0,0,0);
		GameObject.Find("AccountHolder").transform.localScale = new Vector3(1,1,1);
	}

	public void hideAccountHolder()
	{
		LoginButtonAccount.transform.localScale = new Vector3(1, 1, 1);
		LoginButtonGoogle.transform.localScale = new Vector3(1, 1, 1);
		GameObject.Find("AccountHolder").transform.localScale = new Vector3(0, 0, 0);
	}

	public void showPopup()
	{
		GameObject.Find("AccountHolder").transform.localScale = new Vector3(0, 0, 0);
		popup.transform.position = centerV2;
	}

	public void hidePopup()
	{
		//GameObject.Find("AccountHolder").transform.localScale = new Vector3(1, 1, 1);
		popup.GetComponent<PopupButton>().hidePopup();
	}

	public void attemptRegister(string userID, string userName)
	{
		JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);

		userData.AddField("userID", userID);
		userData.AddField("userPW", "");
		userData.AddField("userName", userName);

		socket = networkModule.get_socket();

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
		popupButton.attemptRegisterResult(resultCode, resultMessage);
		if (resultCode == 0)
		{
			string resultID = (string)data[2].str;
			Debug.Log("Register Finished!");
			toastModule.showToast("REGISTER SUCCESS", ToastAlerter.MESSAGE_TYPE.TYPE_GREEN, 1);
			GameObject.Find("IDinput").GetComponent<InputField>().text = resultID;
		}
	}

}