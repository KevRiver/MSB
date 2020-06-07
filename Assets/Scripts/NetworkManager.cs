#define NETMANAGER_LOG_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using MSBNetwork;
using Newtonsoft.Json.Linq;

public class NetworkManager : MonoBehaviour
{
    NetworkModule _networkManager;
    OnGameMatched gameMatchedListener;


    private class OnGameMatched : NetworkModule.OnGameMatchedListener
    {
        public LobbyButton lobbyButton;

        void NetworkModule.OnGameMatchedListener.OnGameMatched(bool _result, int _room, string _message)
        {
            lobbyButton.matchedLoading(_room);
        }
    }
    private class OnGameInfo : NetworkModule.OnGameInfoListener
    {
        void NetworkModule.OnGameInfoListener.OnGameInfo(bool _result, int _room, int _mode, LinkedList<UserData> _users, string _message)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(LoadScene(_mode, _room, _users));
        }

        private IEnumerator LoadScene(int _mode, int _room, LinkedList<UserData> _users)
        {
            // make "gameinfo" object
            GameInfo gameInfo = GameInfo.Instance;
            gameInfo.room = _room;
            gameInfo.mode = _mode;
#if NETMANAGER_LOG_ON
            Debug.LogFormat("GameInfo room :{0} mode :{1} created", gameInfo.room, gameInfo.mode);
#endif
            foreach (UserData user in _users)
            {
                PlayerInfo player = new PlayerInfo(_room, user.userNumber, user.userID, user.userNick, user.userWeapon, user.userSkin);
                gameInfo.players.Add(player);
            }
            
            // load play scene
            SceneManager.LoadSceneAsync("Scenes/PlayScene");
            yield return null;
        }
    }
    /*
    private class OnGameStatus : NetworkModule.OnGameStatusListener
    {
        private MSB_GUIManager _guiManager;
        public OnGameStatus()
        {
            _guiManager = null;
        }

        public OnGameStatus(MSB_GUIManager guiManager)
        {
            _guiManager = guiManager;
        }

        // 플레이어가 다 준비가 되면 서버에서 뿌려주는 이벤트
        public void OnGameEventCount(int count)
        {
            Debug.LogWarning(count);
            _guiManager.UpdateMessageBox(count);
            if (count == 0)
            {
#if NETMANAGER_LOG_ON
                Debug.LogWarning("GameStart Triggered");
#endif
                MMGameEvent.Trigger("GameStart");
            }
        }

        public void OnGameEventMessage(int type, string message)
        {
            if (type == 2)
            {
                int medalIndex = Convert.ToInt32(message);
                AchievementViewData data = new AchievementViewData(medalIndex);
                _guiManager.AchievementViewModel.Initialize(data);
            }
        }

        public void OnGameEventReady(string readyData)
        {
            JArray jArray = JArray.Parse(readyData);
            bool isAllPlayerReady = true;
            foreach (var token in jArray.Children())
            {
                JObject o = (JObject) token;
                var ready = o.Properties().Select(p=>p.Value).FirstOrDefault();
                isAllPlayerReady &= (bool)ready;
            }

            if (!isAllPlayerReady)
                return;
            
            _guiManager.LoadingViewModel.PlayOutroAnimation();
            _guiManager.MessageBox.gameObject.SetActive(true);
            _guiManager.SmallMessageBox.gameObject.SetActive(true);
        }

        public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
        {
            MSB_GameManager.Instance.ScoreUpdate(blueDeath, redDeath, bluePoint, redPoint);
        }
        public void OnGameEventTime(int time)
        {
            _guiManager.UpdateTimer(time);
            
            if(time == 10)
                MMGameEvent.Trigger("HurryUp");
            if (time == 0)
                MMGameEvent.Trigger("GameOver");
        }
    }
    */
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _networkManager = NetworkModule.GetInstance();

        gameMatchedListener = new OnGameMatched();
        _networkManager.AddOnEventGameQueue(gameMatchedListener);
        _networkManager.AddOnEventGameInfo(new OnGameInfo());
        //_networkManager.AddOnEventGameStatus(new OnGameStatus());
    }

    // Update is called once per frame
    void Update()
    {
        _networkManager.OnUpdate();
    }

    public void connectServer()
    {
        _networkManager.Connect("203.250.148.113", 9993);
    }

    private void OnDestroy()
    {
        _networkManager.Disconnect();
    }

    public void loadScene(int _room)
    {
        NetworkModule.GetInstance().RequestGameInfo(_room);
    }

    public void getLobbyButton(LobbyButton _lobbyButton)
    {
        gameMatchedListener.lobbyButton = _lobbyButton;
    }
}
