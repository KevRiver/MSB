using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Controller controller;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerPrefab");
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        //controller.SetTargetObj(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
