using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    User me;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {

    }

    public void setUser(User user)
    {
        me = user;
    }
}
    

