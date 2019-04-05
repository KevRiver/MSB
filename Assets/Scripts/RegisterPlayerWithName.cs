using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System;

public class RegisterPlayerWithName : MonoBehaviour
{
    public InputField m_playerName;
    private string strPlayerName;

    public void OnLoginButtonClick()
    {
        strPlayerName = m_playerName.text;

        GameObject networkmodule = GameObject.Find("NetworkModule");
        SocketIOComponent socket;
        JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);

        userData.AddField("userKey", strPlayerName);
        

        socket = networkmodule.GetComponent<NetworkModule>().getSocket();
        socket.On("loginSuccess", loginSuccess);
        socket.On("loginFail", loginFail);
        socket.Emit("login",userData);
        //Debug.Log(strPlayerName);
    }

    public void loginSuccess(SocketIOEvent obj)
    {
        Debug.Log("Login Success!" + obj);

        User localPlayer = new User();
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
        GameObject.Find("Me").GetComponent<LocalPlayer>().setUser(localPlayer);

        //로그인 버튼 -> 로그 아웃 버튼, 큐 입장 버튼 생성
    }

    public void loginFail(SocketIOEvent obj)
    {
        Debug.Log("Login Fail" + obj);
    }
}
