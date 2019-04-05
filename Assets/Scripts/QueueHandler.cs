using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using System;

public class QueueHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnQueueButtonClick()
    {
        Debug.Log("Queue Button Clicked");
        GameObject networkmodule = GameObject.Find("NetworkModule");
        SocketIOComponent socket;
        JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);

        socket = networkmodule.GetComponent<NetworkModule>().getSocket();
        socket.On("userConnected", userConnected);
        socket.On("soloMatched",soloMatched);
        socket.On("soloRoomFull",soloRoomFull);
        socket.Emit("gameRoomEnter");
    }

    private void userConnected(SocketIOEvent e)
    {
        Debug.Log(e);
    }

    public void soloMatched(SocketIOEvent e)
    {
        JSONObject data = e.data;

        int gameRoomIndex;
        try
        {
            gameRoomIndex = (int)data[0].n;
        }
        catch (Exception err)
        {
            gameRoomIndex = -1;
            Debug.Log("Game Room Index : -1");
        }

        int position;
        try
        {
            position = (int)data[1].n;
        }
        catch (Exception err)
        {

        }

        Debug.Log(data[2]);
        //JSONObject userlist;
        //Debug.Log(data)
        //SceneManager.LoadScene("GameScene");
    }

    public void soloRoomFull(SocketIOEvent e)
    {
        Debug.Log(e);
    }
}
