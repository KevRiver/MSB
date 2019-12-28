using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using MSBNetwork;

public class NetworkManager : MonoBehaviour
{
    NetworkModule _networkManager;
    private class OnGameMatched : NetworkModule.OnGameMatchedListener
    {
        void NetworkModule.OnGameMatchedListener.OnGameMatched(bool _result, int _room, string _message)
        {
            Debug.LogWarning("OnGameMatched Called");
            NetworkModule.GetInstance().RequestGameInfo(_room);
            Debug.LogWarning("RequestGameInfo");
        }
    }
    private class OnGameInfo : NetworkModule.OnGameInfoListener
    {
        void NetworkModule.OnGameInfoListener.OnGameInfo(bool _result, int _room, int _mode, LinkedList<UserData> _users, string _message)
        {
            Debug.LogWarning("OnGameInfo Called");
            UnityMainThreadDispatcher.Instance().Enqueue(LoadScene(_mode, _room, _users));
        }

        private IEnumerator LoadScene(int _mode, int _room, LinkedList<UserData> _users)
        {
            // make "gameinfo" object
            GameInfo gameInfo = GameInfo.Instance;
            gameInfo.room = _room;
            foreach (UserData user in _users)
            {
                PlayerInfo player = new PlayerInfo(_room, user.userNumber, user.userID, user.userNick, user.userWeapon, user.userSkin);
                gameInfo.players.Add(player);
            }
            
            foreach (var player in gameInfo.players)
            {
                Debug.LogWarning(player.number + " " + player.id);
            }
            // load play scene
            SceneManager.LoadSceneAsync("Scenes/PlayScene");
            yield return null;
        }
    }
    private class OnGameStatus : NetworkModule.OnGameStatusListener
    {       
        public void OnGameEventCount(int count)
        {
            MSB_GUIManager.Instance.UpdateMessageBox(count);
            if(count == 0)
                MMGameEvent.Trigger("GameStart");
        }
        
        /*public void OnGameEventMessage(object _data)
        {

        }*/

        public void OnGameEventMessage(int type, string message)
        {
        }
        
        public void OnGameEventReady(string readyData)
        {
            
        }

        public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
        {
            MSB_GameManager.Instance.ScoreUpdate(blueDeath,redDeath,bluePoint, redPoint);
            Debug.LogWarning("Event Score - blue : " + bluePoint + " red : " + redPoint);

        }
        public void OnGameEventTime(int time)
        {
            MSB_GUIManager.Instance.UpdateTimer(time);
            if(time == 0)
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
        _networkManager.Connect("203.250.148.113",9993);
        
        _networkManager.AddOnEventGameQueue(new OnGameMatched());
        _networkManager.AddOnEventGameInfo(new OnGameInfo());
        _networkManager.AddOnEventGameStatus(new OnGameStatus());
    }

    // Update is called once per frame
    void Update()
    {
        _networkManager.OnUpdate();
    }

    private void OnDestroy()
    {
        _networkManager.Disconnect();
    }
}
