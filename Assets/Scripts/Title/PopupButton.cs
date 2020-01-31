using System.Collections;
using System.Collections.Generic;
using MSBNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupButton : MonoBehaviour
{
	public GameObject canvasObject;
	static ToastAlerter toastModule;
	static LoadSplash splashModule;
	public GameObject popupUI;
	public GameObject policyUI;
	public GameObject nickNameUI;
	public GameObject nickNameInput;
	public GameObject registerResult;
	public static string playerID;
	public static string playerNICK;
	static Vector2 centerPos;
	static Vector2 outsidePos;
	// Start is called before the first frame update
	void Start()
	{
		toastModule = canvasObject.GetComponent<ToastAlerter>();
		splashModule = canvasObject.GetComponent<LoadSplash>();
		centerPos = GameObject.Find("CenterPos").transform.position;
		outsidePos = GameObject.Find("PolicyUI").transform.position;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void enterPolicyPopup(string _playerID)
	{
		playerID = _playerID;
		popupUI.transform.position = centerPos;
		nickNameUI.transform.position = outsidePos;
		policyUI.transform.position = centerPos;
	}

	public void enterNicknameInput()
	{
		policyUI.transform.position = outsidePos;
		nickNameUI.transform.position = centerPos;
	}

	public void nicknameButtonClick()
	{
		Debug.Log("nickname button click!");
		registerResult.GetComponent<Text>().text = "";
		string userNameInput = nickNameInput.GetComponent<InputField>().text;
		if (userNameInput == null || userNameInput.Length < 1)
		{
			registerResult.GetComponent<Text>().text = "닉네임을 입력하세요";
			toastModule.showToast("닉네임을 입력하세요", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
			return;
		}
		if (userNameInput.Length < 4)
		{
			registerResult.GetComponent<Text>().text = "닉네임이 너무 짧습니다";
			toastModule.showToast("닉네임이 너무 짧습니다", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
			return;
		}
		if (userNameInput.Length > 8)
		{
			registerResult.GetComponent<Text>().text = "닉네임이 너무 깁니다";
			toastModule.showToast("닉네임이 너무 깁니다", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
			return;
		}
		playerNICK = userNameInput;
		Debug.Log("nickname button click! register attempt");
		NetworkModule.GetInstance().AddOnEventSystem(new SystemCallback(this));
		NetworkModule.GetInstance().RequestUserSystemNick(PopupButton.playerID, userNameInput);
	}

	class SystemCallback : NetworkModule.OnSystemResultListener
	{
		private PopupButton instance;
		public SystemCallback(PopupButton _instance)
		{
			instance = _instance;
		}

		public void OnSystemNickResult(bool _result, string _data)
		{
			instance.attemptRegisterResult(_result, _data);
		}

		public void OnSystemRankResult(bool _result, string _data)
		{

		}
	}

	public void attemptRegisterResult(bool result, string message)
	{
		Debug.Log("attempt register result");
		switch (result)
		{
			case true:
				LocalUser localUser = LocalUser.Instance;
				localUser.localUserData.userNick = playerNICK;
				SceneManager.LoadScene("Lobby");
				break;
			case false:
				registerResult.GetComponent<Text>().text = message;
				toastModule.showToast(message, ToastAlerter.MESSAGE_TYPE.TYPE_RED, 1);
				break;
		}
	}
}
