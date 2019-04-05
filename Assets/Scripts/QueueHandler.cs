using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

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

    }

    public void soloRoomFull(SocketIOEvent e)
    {

    }
}
