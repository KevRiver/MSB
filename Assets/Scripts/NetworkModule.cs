using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkModule : MonoBehaviour
{
    public GameObject m_SocketManager;

    private SocketIOComponent m_SocketIOComponet;

    private void Awake()
    {
        Debug.Log("NetworkModule Awaked");
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_SocketIOComponet = m_SocketManager.GetComponent<SocketIOComponent>();
        m_SocketIOComponet.On("open", onSocketOpen);
        m_SocketIOComponet.On("error", onSocketError);
        m_SocketIOComponet.On("close", onSocketClose);

    }

    public SocketIOComponent getSocket()
    {
        if (m_SocketIOComponet == null)
        {
            Debug.Log("socket is null");
        }
        return m_SocketIOComponet;
    }

    private void onSocketOpen(SocketIOEvent e)
    {
        Debug.Log("socket opened" + e);
    }

    private void onSocketError(SocketIOEvent e)
    {
        Debug.Log("socket error" + e);
    }

    private void onSocketClose(SocketIOEvent e)
    {
        Debug.Log("socket closed" + e);
    }
}
