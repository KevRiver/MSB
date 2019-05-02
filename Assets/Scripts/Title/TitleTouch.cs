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
    // Start is called before the first frame update
    void Start()
    {
        networkModule = GameObject.Find("NetworkModule").GetComponent<NetworkModule>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       //Debug.Log(popup.transform.position);
       //Debug.Log("Click");
        showPopup();
        socket = networkModule.get_socket();
        JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);
        string strPlayerName = "ABC";

        userData.AddField("userID", strPlayerName);
        userData.AddField("userPW", "");

        socket.On("loginSuccess", loginSuccess);
        socket.On("loginFail", loginFail);
        socket.Emit("login", userData);
    }

    public void loginSuccess(SocketIOEvent obj)
    {
        User localPlayer = new User();
        Debug.Log("Login Success!" + obj);
        JSONObject data = obj.data;
        int userNumber;
        try
        {
            localPlayer.Num = (int)data[0].n;
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
    }

    public void showPopup()
    {
        popup.transform.position = new Vector2(300,173);
    }
}
