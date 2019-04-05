using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    ArrayList userlist;
    Vector3[] spawnPoints = new Vector3[6];
    public GameObject newPlayer;
    void Start()
    {
        //Initialize Spawning Points
        spawnPoints[0] = new Vector3(1, 0, 0);
        spawnPoints[1] = new Vector3(-1, 0, 0);
        spawnPoints[2] = new Vector3(4, 0, 0);
        spawnPoints[3] = new Vector3(1, 0, 0);
        spawnPoints[4] = new Vector3(2, 0, 0);
        spawnPoints[5] = new Vector3(3, 0, 0);

        //Spawning users
        userlist = GameObject.Find("UserData").GetComponent<UserData>().getUserlist();

        Debug.Log("AAAAA"+userlist);
        Debug.Log(userlist[0]);
        Debug.Log(userlist[1]);
        int i = 0;
        foreach(User userData in userlist)
        {
            Debug.Log(userData);
            Instantiate(newPlayer, new Vector3(0,0,0), Quaternion.identity);
            //newPlayer.GetComponent<Player>().m_userIndex = (int)userData[0].n;
            //newPlayer.GetComponent<Player>().m_userID = userData[1].str;
        }
    }

    void Update()
    {
        
    }
}
