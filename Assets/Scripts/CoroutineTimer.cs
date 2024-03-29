﻿#define TIMER_LOG_ON
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public static class CoroutineTimer
{
    public static IEnumerator Wait(float t)
    {
        yield return new WaitForSeconds(t);
    }
    public static IEnumerator InvokeAfter(float t, MSBNetwork.NetworkMethod method,int room)
    {
        yield return new WaitForSeconds(t);
        method(room);
    }
}
