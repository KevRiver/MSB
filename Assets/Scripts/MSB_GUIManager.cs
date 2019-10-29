using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using MSBNetwork;
using Newtonsoft.Json.Linq;

public class MSB_GUIManager : Singleton<MSB_GUIManager>
{
    // 타이머
    // 중앙 메세지 박스
    // 스코어 전광판

    public int initialTime;
    private int _curTime;
    public List<string> msgSequence;
    
    public Text timer;
    private string _min;
    private string _sec;
    public Text blueScore, redScore;
    public Text MessageBox;

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
        blueScore.text = "0";
        redScore.text = "0";
        _min = (initialTime / 60).ToString();
        _sec = (initialTime % 60).ToString();
        timer.text = _min + " : " + _sec;
    }
    
    public void UpdateScoreSign(int b, int r)
    {
        blueScore.text = b.ToString();
        redScore.text = r.ToString();
    }

    public void UpdateTimer(int time)
    {
        _curTime = time;
        _min = (_curTime / 60).ToString();
        _sec = (_curTime % 60).ToString();
        timer.text = _min + " : " + _sec;
    }

    public void UpdateMessageBox(int _seq)
    {
        MessageBox.text = msgSequence[_seq];
        if (_seq == 0)
            Invoke("MessageBoxReset", 0.5f);

    }
    private void MessageBoxReset()
    {
        MessageBox.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
