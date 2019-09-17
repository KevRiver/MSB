using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timer;
    public int initialTime;
    public int currenttime;

    private string min;
    private string sec;

    private void Start()
    {
        currenttime = initialTime;
        UpdateTime(initialTime);
    }

    public void UpdateTime(int time)
    {
        currenttime = time;
        min = (currenttime / 60 >= 10) ? (currenttime / 60).ToString() : "0" + (currenttime / 60).ToString();
        sec = (currenttime % 60 >= 10) ? (currenttime % 60).ToString() : "0" + (currenttime % 60).ToString();
        ShowCurrentTime();
    }

    public void ShowCurrentTime()
    {
        timer.text = min + ":" + sec;
    }
}
