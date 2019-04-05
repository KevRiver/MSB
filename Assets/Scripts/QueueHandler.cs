using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
<<<<<<< HEAD
using System;
using UnityEngine.SceneManagement;
=======
using UnityEngine.SceneManagement;
using System;
>>>>>>> e0dbe51c7fac89ce442190f238b1a71e7bef343f

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
<<<<<<< HEAD
        //SceneManager.LoadScene("SampleScene");

        JSONObject data = e.data;

        int gameRoomIndex;
        try
        {
            gameRoomIndex = (int)data[0].n;
        }
        catch (Exception err) { };

=======
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

>>>>>>> e0dbe51c7fac89ce442190f238b1a71e7bef343f
        int position;
        try
        {
            position = (int)data[1].n;
        }
<<<<<<< HEAD
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

        
=======
        catch (Exception err)
        {

        }

        Debug.Log(data[2]);
        //JSONObject userlist;
        //Debug.Log(data)
        //SceneManager.LoadScene("GameScene");
>>>>>>> e0dbe51c7fac89ce442190f238b1a71e7bef343f
    }

    public void soloRoomFull(SocketIOEvent e)
    {
        Debug.Log(e);
    }
}
