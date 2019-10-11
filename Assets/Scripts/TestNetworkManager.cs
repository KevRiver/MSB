using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;

public class TestNetworkManager : MonoBehaviour
{
    public List<UserData> userData;
    private UserData user1;
    private UserData user2;

    private void Awake()
    {
        //Initialize test users
        user1 = new UserData(1, "Qon", "Qon", 0, 0, 0, 0, 0);
        userData.Add(user1);

        user2 = new UserData(2, "Test", "Test", 0, 0, 0, 0, 0);
        userData.Add(user2);
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
    }   

    // Update is called once per frame
    void Update()
    {
        
    }
}
