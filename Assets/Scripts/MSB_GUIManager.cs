﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using MSBNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine.SocialPlatforms.Impl;

public class MSB_GUIManager : Singleton<MSB_GUIManager>,MMEventListener<MMGameEvent>
{
    // 타이머
    // 중앙 메세지 박스
    // 스코어 전광판

    public int initialTime;
    private int _curTime;
    public List<string> msgSequence;
    
    public Text Timer;
    public Image TimerImage;
    private bool _timeStop;
    private string _min;
    private string _sec;
    public Text ScoreSign;
    public Text BlueScore, RedScore;
    public Text MessageBox;
    public Image Joystick;
    public Image AttackButton;

    private List<GameObject> _uiContainer;
    /*private class OnGameStatus : NetworkModule.OnGameStatusListener
    {
        public void OnGameEventCount(int count)
        {
            Debug.LogWarning("OnGameStatus Count : " + count);
        }

        public void OnGameEventTime(int time)
        {
            throw new System.NotImplementedException();
        }

        private int playerCount;
        public int expectedPlayers = 2;
        private bool PlayerReady;
        private int userNum;
        public void OnGameEventReady(string readyData)
        {
            JArray jArray = JArray.Parse(readyData);
            playerCount = 0;
            userNum = 0;
            foreach (var jObject in jArray)
            {
                PlayerReady = (bool)((JObject)jObject).GetValue((++userNum).ToString());
                if(!PlayerReady)
                    continue;
                playerCount++;
            }

            if (playerCount < expectedPlayers)
                Debug.LogWarning("Waiting another player loading");
            else if (playerCount > expectedPlayers)
                Debug.LogWarning("More Player Loaded than expected");
            else
            {
                Debug.Log("All Players Ready");
            }
        }

        public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameEventMessage(int type, string message)
        {
            throw new System.NotImplementedException();
        }
    }*/

    protected  override  void Awake()
    {
        base.Awake();
        gameObject.name = "GUIManager";
    }
    
    void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        BlueScore.text = "0";
        RedScore.text = "0";
        _min = (initialTime / 60).ToString();
        _sec = (initialTime % 60).ToString();
        Timer.text = _min + ":" + _sec;
        _timeStop = false;

        // MessageBox를 제외한 UI 들을 저장
        _uiContainer = new List<GameObject>();
        _uiContainer.Add(Timer.gameObject);
        _uiContainer.Add(TimerImage.gameObject);
        _uiContainer.Add(ScoreSign.gameObject);
        _uiContainer.Add(Joystick.gameObject);
        _uiContainer.Add(AttackButton.gameObject);
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void UIActive(bool active)
    {
        foreach (var ui in _uiContainer)
        {
            ui.SetActive(active);
        }
    }
    public void UpdateScoreSign(int b, int r)
    {
        BlueScore.text = b.ToString();
        RedScore.text = r.ToString();
    }
    public void UpdateTimer(int time)
    {
        if (_timeStop)
            return;
        
        _curTime = time;
        _min = (_curTime / 60).ToString();
        _sec = (_curTime % 60).ToString();
        Timer.text = _min + ":" + _sec;
    }
    public void UpdateMessageBox(int _seq)
    {
        MessageBox.text = msgSequence[_seq];
        if (_seq == 0)
            Invoke("MessageBoxReset", 0.5f);
    }
    

    /// <summary>
    /// 메세지 박스에 메세지를 출력합니다
    /// </summary>
    /// <param name="message"> 출력할 메세지 </param>
    /// <param name="duration"> -1 메세지 출력 유지 / 메세지 출력 유지 시간</param>
    public void UpdateMessageBox(string message, float duration)
    {
        if (!MessageBox.enabled)
            MessageBox.enabled = true;
        
        MessageBox.text = message;
        if (duration > 0)
            Invoke("MessageBoxReset",duration);
    }

    private void MessageBoxReset()
    {
        MessageBox.text = "";
    }
    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                break;
            
            case "GameOver":
                _timeStop = true;
                UIActive(false);
                break;
        }
    }
}
