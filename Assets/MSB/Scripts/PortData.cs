using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortData : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public int port = 8888;

    public void SetPort(int _port)
    {
        port = _port;
    }
}
