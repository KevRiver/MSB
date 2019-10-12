using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupButton : MonoBehaviour
{
	public GameObject canvasObject;
	ToastAlerter toastModule;
	LoadSplash splashModule;
	public GameObject popup;
	public GameObject policyUI;
	public GameObject nickNameUI;
	public GameObject nickNameInput;
	public GameObject registerResult;
	public TitleTouch titleTouchHandler;
	Vector2 centerPos;
	Vector2 outsidePos;
	// Start is called before the first frame update
	void Start()
	{
		toastModule = canvasObject.GetComponent<ToastAlerter>();
		splashModule = canvasObject.GetComponent<LoadSplash>();
		outsidePos = popup.transform.position;
		titleTouchHandler = GameObject.Find("LoginButton").GetComponent<TitleTouch>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void policyButtonClick()
	{
		centerPos = popup.transform.position;
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
		Debug.Log("nickname button click! register attempt");
		// TODO titleTouchHandler.attemptRegister(userNameInput);
	}

	public void attemptRegisterResult(int resultCode, string error)
	{
		Debug.Log("attempt register result");
		switch (resultCode)
		{
			case 0:
				popup.transform.position = outsidePos;
				GameObject.Find("ClickText").GetComponent<Text>().color = new Color(0f, 0f, 0f, 1f);
				break;
			case 1:
				registerResult.GetComponent<Text>().text = "ID가 이미 존재합니다";
				toastModule.showToast("ID가 이미 존재합니다", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
				break;
			case 2:
				registerResult.GetComponent<Text>().text = "닉네임이 이미 존재합니다";
				toastModule.showToast("닉네임이 이미 존재합니다", ToastAlerter.MESSAGE_TYPE.TYPE_ORANGE, 1);
				break;
			case 3:
				registerResult.GetComponent<Text>().text = "에러 : " + error;
				toastModule.showToast("ERROR: " + error, ToastAlerter.MESSAGE_TYPE.TYPE_RED, 1);
				break;
		}
	}
}
