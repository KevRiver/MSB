using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class GameManager : MonoBehaviour
{
    int gameRoomIndex;
    int gamePlayerIndex;
    ArrayList userlist;
    Vector3[] spawnPoints = new Vector3[6];
    public GameObject PlayerPrefab;
    public ArrayList players = new ArrayList();

    private SocketIOComponent socketIO;

    private void Awake()
    {
        //Spawning users
        UserData userData = GameObject.Find("UserData").GetComponent<UserData>();
        userlist = userData.getUserlist();
        gameRoomIndex = userData.getRoomIndex();
        gamePlayerIndex = userData.getPlayerIndex();

        GameObject networkmodule = GameObject.Find("NetworkModule");
        socketIO = networkmodule.GetComponent<NetworkModule>().get_socket();
    }

    void Start()
    {
        //Initialize Spawning Points
        spawnPoints[0] = new Vector3(1, 0, 0);
        spawnPoints[1] = new Vector3(-1, 0, 0);
        spawnPoints[2] = new Vector3(4, 0, 0);
        spawnPoints[3] = new Vector3(1, 0, 0);
        spawnPoints[4] = new Vector3(2, 0, 0);
        spawnPoints[5] = new Vector3(3, 0, 0);


        //Debug.Log("AAAAA"+userlist);
        //Debug.Log(userlist[0]);
        //Debug.Log(userlist[1]);
        int i = 0;
        foreach(User userData in userlist)
        {
            GameObject player = new GameObject();
            Debug.Log(userData.Num);
            player = Instantiate(PlayerPrefab, new Vector3(0, 0, 100), Quaternion.identity);
            player.GetComponent<PlayerDetail>().Controller = userData;
            players.Add(player);

            if (userData.Num == GameObject.Find("LocalPlayer").GetComponent<LocalPlayer>().getLocalPlayer().Num) //localPlayer의 index와 생성한 Player의 인덱스가 같으면
            {
                Debug.Log("Check local player");
                Debug.Log(player.GetComponent<PlayerDetail>().Controller.Num);
                player.AddComponent<Player>();
                GameObject.Find("Main Camera").GetComponent<followCamera>().setTarget(player.transform);
            }
        }

        socketIO.On("userGameMove", OnUserMove);
        socketIO.On("userGameAction", OnUserAction);

    }

    void Update()
    {
        
    }

    void OnUserMove(SocketIOEvent e)
    {
        //Debug.Log(e.name + " / " + e.data);
        JSONObject data = e.data;
        int userIndex = (int)data[0].n;
        JSONObject moveData = data[1];
        Vector3 newPosition = new Vector3(0, 0, 0);
        float toward;
        float velocityX, velocityY;
        newPosition.x = moveData[1].n;
        newPosition.y = moveData[2].n;
        newPosition.z = moveData[3].n;
        toward = moveData[4].n;
        velocityX = moveData[5].n;
        velocityY = moveData[6].n;
        
        //GameObject.Find
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerDetail>().Controller.Num == userIndex)
            {
                player.transform.SetPositionAndRotation(newPosition, Quaternion.identity);
                player.transform.localScale = new Vector3(toward, 1.5f);
                player.GetComponent<Rigidbody>().AddForce(new Vector3(velocityX, velocityY, 0));
                Debug.Log("called transform translate");
            }
            
        }
    }

    void OnUserAction(SocketIOEvent e)
    {
        Debug.Log(e.name + " / " + e.data);
    }

}
