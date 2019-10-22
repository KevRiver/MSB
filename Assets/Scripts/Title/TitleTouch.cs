using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using MSBNetwork;

public class TitleTouch : MonoBehaviour, IPointerClickHandler
{
    private static TitleTouch instance = null;

	NetworkModule networkModule;

	public GameObject canvasObject;
	static ToastAlerter toastModule;
	LoadSplash splashModule;

	public GameObject popup;
	public GameObject centerPos;
	public PopupButton nicknameButton;
	Vector2 centerV2;

	public string playerName = "LimeCake";
	public string playerPassword = "LimeCake";

    class ConnectionCallback : NetworkModule.OnServerConnectListener
    {
        public void OnServerConnection(bool result, string message)
        {
            OnConnectResult(result, message);
        }
    }

    class LoginCallback : NetworkModule.OnLoginResultListener
    {
        public void OnLoginResult(bool _result, UserData _user, int _game, string _message)
        {
            loginResult(_result, _user, _game, _message);
        }
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
	{
		toastModule = canvasObject.GetComponent<ToastAlerter>();
		splashModule = canvasObject.GetComponent<LoadSplash>();

        networkModule = NetworkModule.GetInstance();

		centerV2 = centerPos.transform.position;
		nicknameButton = GameObject.Find("NicknameButton").GetComponent<PopupButton>();

        networkModule.SetOnEventServerConnect(new ConnectionCallback());
        networkModule.SetOnEventUserLogin(new LoginCallback());
	}

    public static void OnConnectResult(bool result, string msg)
    {
        if (result)
        {
            Debug.LogWarning("SERVER CONNECTION SUCCESS" + msg);
        } else
        {
            Debug.LogError("SERVER CONNECTION ERROR : " + msg);
        }
    }

	public void OnPointerClick(PointerEventData eventData)
	{
		splashModule.showLoadSplash(100, "로그인 중입니다", LoadSplash.SPLASH_TYPE.TYPE_SHORT);
		Invoke("attemptLogin", (float)100 / 1000);
	}

	public void attemptLogin()
    {
        networkModule.RequestUserLogin(playerName, playerPassword);
    }

	public static void loginResult(bool _result, UserData _user, int _game, string _message)
	{
		
		if (_result)
        {
            Debug.LogWarning("OnLoginResult : InitializeLocalUser");
            LocalUser localUser = LocalUser.Instance;
            localUser.localUserData.userID = _user.userID;
            localUser.localUserData.userNick = _user.userNick;
            localUser.localUserData.userNumber = _user.userNumber;
            localUser.localUserData.userMoney = _user.userMoney;
            localUser.localUserData.userCash = _user.userCash;
            localUser.localUserData.userWeapon = _user.userWeapon;
            localUser.localUserData.userSkin = _user.userSkin;

            localUser.DebugLocalUserData();

            toastModule.showToast("Welcome " + localUser.localUserData.userNick + " !", ToastAlerter.MESSAGE_TYPE.TYPE_GREEN, 1);
            instance.StartCoroutine(switchLobbyScene());
        } else
        {
            Debug.LogError("LOGIN ERROR : " + _message);
        }
	}

	private static IEnumerator switchLobbyScene()
	{
		yield return new WaitForSeconds(2);
        //Temp Lobby test
		SceneManager.LoadScene("TempLobby");
	}

	public void showPopup()
	{
		GameObject.Find("ClickText").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
		popup.transform.position = centerV2;
	}

}