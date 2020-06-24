using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using MSBNetwork;
using Random = System.Random;

public class AutoConnector : MonoBehaviour, IPointerClickHandler
{
    public static AutoConnector instance = null;

	NetworkModule networkModule;

	public GameObject AutoConnectorText;
	
	public GameObject canvasObject;
	static ToastAlerter toastModule;
	LoadSplash splashModule;

	public GameObject popup;
	public GameObject centerPos;
	public static PopupButton nicknameButton;

	private static string playerID;
	private static string playerPW;
	private static string[] envArguments;
	private Vector2 centerV2;

	public static bool AUTO_FAILED = false;
	public static bool SERVER_CONNECTED = false;

	public GameObject NetworkManager;

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
        
        NetworkManager.GetComponent<NetworkManager>().connectServer();
        
        AutoConnectorText.SetActive(false);
        Invoke("showMessage", 1);
        // Invoke("startConnector", 3);
	}

    public static void OnConnectResult(bool result, string msg)
    {
        if (result)
        {
            Debug.LogWarning("SERVER CONNECTION SUCCESS" + msg);
            SERVER_CONNECTED = true;
        } else
        {
            Debug.LogError("SERVER CONNECTION ERROR : " + msg);
            SERVER_CONNECTED = false;
            instance.AutoConnectorText.GetComponent<Text>().text = "서버 연결 실패";
            AUTO_FAILED = true;
            try
            {
	            NetworkModule.GetInstance().Disconnect();
            } catch (Exception) { }
        }
    }

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!AUTO_FAILED) return;
		startConnector();
	}

	public void showMessage()
	{
		AutoConnectorText.SetActive(true);
		AutoConnectorText.GetComponent<Text>().text = "서버 연결중입니다";
	}
	
	public void startConnectorLazy()
	{
		Invoke("startConnector", 2);
	}
	
	public void startConnector()
	{
		Debug.LogWarning("SERVER LOGIN START");
		if (!SERVER_CONNECTED)
		{
			try
			{
				NetworkManager.GetComponent<NetworkManager>().connectServer();
				return;
			}
			catch (Exception e)
			{
				AutoConnectorText.GetComponent<Text>().text = "서버 연결 실패";
				return;
			}
		}
		attemptLogin();
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
		//AutoConnectorText.GetComponent<Text>().text = "로그인 중입니다";
		if (string.IsNullOrEmpty(playerID) && string.IsNullOrEmpty(playerPW))
		{
			playerID = SystemInfo.deviceUniqueIdentifier;
			if (playerID.Length > 50) playerID = playerID.Substring(0, 50);
			playerPW = SystemInfo.deviceUniqueIdentifier;
			if (playerPW.Length > 50) playerPW = playerPW.Substring(0, 50);
			if (envArguments != null && envArguments.Length >= 4 && envArguments[0].Equals("-id") && envArguments[2].Equals("-pw"))
			{
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
	            instance.AutoConnectorText.GetComponent<Text>().text = "닉네임을 설정해 주세요";
	            nicknameButton.enterPolicyPopup(playerID);
	            return;
            }

            instance.AutoConnectorText.GetComponent<Text>().text = LocalUser.Instance.localUserData.userNick + "님, 환영합니다!";
            instance.StartCoroutine(switchLobbyScene());
        } else
        {
	        instance.AutoConnectorText.GetComponent<Text>().text = "서버 인증 실패";
            Debug.LogError("LOGIN ERROR : " + _message);
        }
	}

	private static IEnumerator switchLobbyScene()
	{
		yield return new WaitForSeconds(2);
		instance.StartCoroutine(LoadLobbyScene());
	}

	private static IEnumerator LoadLobbyScene()
	{
		yield return null;

		AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
		loadOperation.allowSceneActivation = false;
		instance.AutoConnectorText.GetComponent<Text>().text = "잠시만 기다려 주세요";
		while (!loadOperation.isDone)
		{
			if (loadOperation.progress >= 0.9f) loadOperation.allowSceneActivation = true;
			yield return null;
		}
	}

	public void showPopup()
	{
		GameObject.Find("ClickText").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
		popup.transform.position = centerV2;
	}

}