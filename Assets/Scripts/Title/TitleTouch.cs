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

    public GameObject popup;
    public GameObject centerPos;
    Vector2 centerV2;

    string strPlayerName = "namoo";

    // Start is called before the first frame update
    void Start()
    {
        networkModule = GameObject.Find("NetworkModule").GetComponent<NetworkModule>();

        centerV2 = centerPos.transform.position;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //showPopup();

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
            Debug.Log("Error : UserNumber = -1");
        };

        Debug.Log("User number is : " + localPlayer.Num);
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
                Debug.Log("ID already Exists!");
                break;
            case 2:
                Debug.Log("Name already Exists!");
                break;
            case 3:
                Debug.Log("Unknown Error : " + error);
                break;

        }
    }

    public void showPopup()
    {
        popup.transform.position = centerV2;
    }

    public void reslutRegisterPopup()
    {

    }
}
