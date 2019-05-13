using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    private User localPlayer;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void setLocalPlayer(User user)
    {
        localPlayer = user;
    }

    public User getLocalPlayer()
    {
        return localPlayer;
    }
}
    

