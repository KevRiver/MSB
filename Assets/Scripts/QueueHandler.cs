using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.SceneManagement;

public class QueueHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnQueueButtonClick()
    {
        GameObject networkmodule = GameObject.Find("NetworkModule");
        SocketIOComponent socket;
        JSONObject userData = new JSONObject(JSONObject.Type.OBJECT);

        socket = networkmodule.GetComponent<NetworkModule>().get_socket();
        socket.On("soloMatched",soloMatched);
        socket.On("soloRoomFull",soloRoomFull);
        socket.Emit("matchMakeSolo");
    }

    public void soloMatched(SocketIOEvent e)
    {
        //SceneManager.LoadScene("SampleScene");

        JSONObject data = e.data;

        int gameRoomIndex;
        try
        {
            gameRoomIndex = (int)data[0].n;
        }
        catch (Exception err) { };

        int position;
        try
        {
            position = (int)data[1].n;
        }
        catch (Exception err) { };

        JSONObject userListJSON;
        try
        {
            userListJSON = data[2];

            GameObject.Find("UserData").GetComponent<UserData>().clearUserData();
            int i = 0;
            foreach (JSONObject userData in userListJSON.list)
            {
                Debug.Log("Queue Handler log" + ++i);
                User player = new User();
                player.Num = (int)userData[0].n;
                player.Id = userData[1].str;
                GameObject.Find("UserData").GetComponent<UserData>().addUser(player);
            }

            SceneManager.LoadScene("SampleScene");
        }
        catch (Exception err) { }

        
    }

    public void soloRoomFull(SocketIOEvent e)
    {

    }
}
