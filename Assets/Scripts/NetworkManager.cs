using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using MSBNetwork;

public class NetworkManager : MonoBehaviour
{
    NetworkModule _networkManager;
    OnGameMatched gameMatchedListener;


    private class OnGameMatched : NetworkModule.OnGameMatchedListener
    {
        public LobbyButton lobbyButton;

        void NetworkModule.OnGameMatchedListener.OnGameMatched(bool _result, int _room, string _message)
        {
            //Debug.LogWarning("OnGameMatched Called");
            lobbyButton.matchedLoading(_room);
            //Debug.LogWarning("RequestGameInfo");
        }
    }
    private class OnGameInfo : NetworkModule.OnGameInfoListener
    {
        void NetworkModule.OnGameInfoListener.OnGameInfo(bool _result, int _room, int _mode, LinkedList<UserData> _users, string _message)
        {
            //Debug.LogWarning("OnGameInfo Called");
            UnityMainThreadDispatcher.Instance().Enqueue(LoadScene(_mode, _room, _users));
        }

        private IEnumerator LoadScene(int _mode, int _room, LinkedList<UserData> _users)
        {
            // make "gameinfo" object
            GameInfo gameInfo = GameInfo.Instance;
            gameInfo.room = _room;
            gameInfo.mode = _mode;
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
    private class OnGameStatus : NetworkModule.OnGameStatusListener
    {
        // 플레이어가 다 준비가 되면 서버에서 뿌려주는 이벤트
        public void OnGameEventCount(int count)
        {
            MSB_GUIManager.Instance.UpdateMessageBox(count);
            if (count == 0)
                MMGameEvent.Trigger("GameStart");
        }

        public void OnGameEventMessage(int type, string message)
        {
            if (type == 2)
            {
                //MSB_GameManager.Instance.
            }
        }

        public void OnGameEventReady(string readyData)
        {

        }

        public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
        {
            MSB_GameManager.Instance.ScoreUpdate(blueDeath, redDeath, bluePoint, redPoint);
        }
        public void OnGameEventTime(int time)
        {
            MSB_GUIManager.Instance.UpdateTimer(time);
            
            if(time == 10)
                MMGameEvent.Trigger("HurryUp");
            if (time == 0)
                MMGameEvent.Trigger("GameOver");
        }
    }
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
        _networkManager.AddOnEventGameStatus(new OnGameStatus());
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
