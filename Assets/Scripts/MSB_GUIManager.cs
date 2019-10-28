using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;
using Newtonsoft.Json.Linq;

public class MSB_GUIManager : Singleton<MSB_GUIManager>
{
    // 타이머
    // 중앙 메세지 박스
    // 스코어 전광판

    public string timer;
    public List<string> msgSequence;

    private class OnGameStatus : NetworkModule.OnGameStatusListener
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
            Debug.LogWarning(jArray);
            playerCount = 0;
            userNum = 0;
            foreach (var jToken in jArray)
            {
                var obj = (JObject) jToken;
                if (!(PlayerReady = (bool) obj.GetValue((++userNum).ToString())))
                {
                    Debug.LogWarning("Player " + userNum + "Not Ready");
                    continue;
                }
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
    }

    protected  override  void Awake()
    {
        base.Awake();
        gameObject.name = "GUIManager";
        NetworkModule.GetInstance().AddOnEventGameStatus(new OnGameStatus());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
