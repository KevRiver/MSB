using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

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
        

        socket = networkmodule.GetComponent<NetworkModule>().get_socket();
        socket.On("loginSuccess", loginSuccess);
        socket.On("loginFail", loginFail);
        socket.Emit("login",userData);
        Debug.Log(strPlayerName);
    }

    public void loginSuccess(SocketIOEvent obj)
    {
        Debug.Log("Login Success!" + obj);
    }

    public void loginFail(SocketIOEvent obj)
    {
        Debug.Log("Login Fail" + obj);
    }
}
