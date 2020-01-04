using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using MSBNetwork;
using Random = System.Random;

public class TitleTouch : MonoBehaviour, IPointerClickHandler
{
    private static TitleTouch instance = null;

	NetworkModule networkModule;

	public GameObject canvasObject;
	static ToastAlerter toastModule;
	LoadSplash splashModule;

	public GameObject popup;
	public GameObject centerPos;
	public static PopupButton nicknameButton;

	private static string playerID;
	private static string playerPW;
	private static string[] envArguments;
	Vector2 centerV2;

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
        envArguments = Environment.GetCommandLineArgs();
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

	public void createRandomAccount()
	{
		Random generator = new Random();
		string st = "abcdefghijklmnopqrstuvwxyz0123456789";
		char c1 = st[generator.Next(st.Length)];
		char c2 = st[generator.Next(st.Length)];
		char c3 = st[generator.Next(st.Length)];
		char c4 = st[generator.Next(st.Length)];
		playerID = "T" + c1 + c2 + c3 + c4;
		playerPW = playerID;
	}
	
	public void attemptLogin()
	{
		if (string.IsNullOrEmpty(playerID) && string.IsNullOrEmpty(playerPW))
		{
			playerID = SystemInfo.deviceUniqueIdentifier;
			if (playerID.Length > 50) playerID = playerID.Substring(0, 50);
			playerPW = SystemInfo.deviceUniqueIdentifier;
			if (playerPW.Length > 50) playerPW = playerPW.Substring(0, 50);
			if (envArguments != null && envArguments.Length >= 4 && envArguments[0].Equals("-id") && envArguments[2].Equals("-pw"))
			{
				toastModule.showToast("ID PW 를 가져왔습니다!", ToastAlerter.MESSAGE_TYPE.TYPE_GREEN, 1);
				Debug.LogWarning("ID : " + envArguments[1]);
				Debug.LogWarning("PW : " + envArguments[3]);
				playerID = envArguments[1];
				playerPW = envArguments[3];
			}
		}
        networkModule.RequestUserLogin(playerID, playerPW);
    }

	public static void loginResult(bool _result, UserData _user, int _game, string _message)
	{
		
		if (_result)
        {
	        LocalUser localUser = LocalUser.Instance;
	        localUser.localUserData = new ClientUserData();
	        localUser.localUserData.userID = _user.userID;
            localUser.localUserData.userNick = _user.userNick;
            localUser.localUserData.userNumber = _user.userNumber;
            localUser.localUserData.userMoney = _user.userMoney;
            localUser.localUserData.userCash = _user.userCash;
            localUser.localUserData.userRank = _user.userRank;
            localUser.localUserData.userWeapon = _user.userWeapon;
            localUser.localUserData.userSkin = _user.userSkin;

            localUser.DebugLocalUserData();

            if (_user.userNick == null || _user.userNick.Equals(String.Empty) || _user.userNick.Equals("") || _user.userNick == String.Empty)
            {
	            nicknameButton.enterPolicyPopup(playerID);
	            return;
            }
            
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
		SceneManager.LoadScene("Lobby");
	}

	public void showPopup()
	{
		GameObject.Find("ClickText").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
		popup.transform.position = centerV2;
	}

}