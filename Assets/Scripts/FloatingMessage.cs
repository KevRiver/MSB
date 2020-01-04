using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class FloatingMessageState
{
    public enum State
    {
        Idle,
        Start,
        Destroy
    }
}

public enum FloatingMessageStartAnimation
{
    PopSlideUp,
    Pop
}

public enum FloatingMessageDestroyAnimation
{
    PopOut,
    FadeOut
}

public class FloatingMessage : MonoBehaviour
{

    public float DestroyDelay = 2.0f;
    
    private void Start()
    {
        Destroy(gameObject, DestroyDelay);
    }
}
