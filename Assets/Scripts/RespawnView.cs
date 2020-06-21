using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnView : MonoBehaviour
{
    public Text Title;
    public Text RespawnCount;

    public string[] Triggers; // 0 TitleScaleOut / 1 CountScaleOut
    
    private const int TITLETRIGGER = 0;
    private const int COUNTTRIGGER = 1;
    
    private RectTransform _respawnCountRectTransform;
    private Animator _animator;

    public void Initialization(int time)
    {
        // Initialize animator
        _animator = gameObject.GetComponent<Animator>();
        // Initialize Count RectTransform
        _respawnCountRectTransform = RespawnCount.rectTransform;
        
        // Set cover
        MSB_GUIManager guiManager = MSB_GUIManager.Instance;
        guiManager.SetCover(Color.black,0.5f);
        guiManager.CoverActive(true);
        
        // Set Time
        SetCount(time);
    }

    public void PlayCountAnimation(int time)
    {
        _respawnCountRectTransform.localScale = Vector3.zero;
        SetCount(time);
        RespawnViewTrigger(Triggers[COUNTTRIGGER]);
    }
    
    

    // Start is called before the first frame update
    public void SetCount(int count)
    {
        RespawnCount.text = (count + 1).ToString();
    }

    public void RespawnViewTrigger(string trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
