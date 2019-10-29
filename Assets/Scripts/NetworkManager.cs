using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using MSBNetwork;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class NetworkManager : MonoBehaviour
{
    NetworkModule _networkManager;
    private class OnSoloMatched : NetworkModule.OnSoloMatchedListener
    {
        void NetworkModule.OnSoloMatchedListener.OnSoloMatched(bool _result, int _room, string _message)
        {
            Debug.LogWarning("OnSoloMatched Called");
            NetworkModule.GetInstance().RequestGameInfo(_room);
        }
    }
    private class OnGameInfo : NetworkModule.OnGameInfoListener
    {
        void NetworkModule.OnGameInfoListener.OnGameInfo(bool _result, int _room, int _mode, LinkedList<UserData> _users, string _message)
        {
            Debug.LogWarning("OnGameInfo Called");

            //UnityMainThreadDispatcher.Instance().Enqueue(LoadPlayScene(_room, _users));
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

            // load play scene
            SceneManager.LoadScene("Test");
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
                Debug.Log((JObject) jObject);
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
                Debug.LogWarning("All Players Ready");
        }

        public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
        {
            Debug.LogWarning("Event Score - blue : " + bluePoint + " red : " + redPoint);
        }
        public void OnGameEventTime(int time)
        {
            MSB_GUIManager.Instance.UpdateTimer(time);
        }
    }

    /*private class OnGameUserMove : NetworkModule.OnGameUserMoveListener
    {
        char[] delimiterChars = { ',' };
        void NetworkModule.OnGameUserMoveListener.OnGameUserMove(object _data)
        {
            //Debug.Log("OnGameUserMove called");
            //UnityMainThreadDispatcher.Instance().Enqueue(PlayerMove(_data));
        }

        public IEnumerator PlayerMove(object _data)
        {
            string[] dataArray = ((string)_data).Split(delimiterChars);
            int target = int.Parse(dataArray[0]);
            float posX = float.Parse(dataArray[1]);
            float posY = float.Parse(dataArray[2]);
            float posZ = float.Parse(dataArray[3]);
            float speedX = float.Parse(dataArray[4]);
            float speedY = float.Parse(dataArray[5]);
            //bool isGrounded = bool.Parse(dataArray[6]);
            bool isFacingRight = bool.Parse(dataArray[6]);
            float rotZ = float.Parse(dataArray[7]);

            if (LocalUser.Instance.localUserData.userNumber == target)
            {
                yield break;
            }
            else
            {
                foreach (MSB_Character player in MSB_LevelManager.Instance.Players)
                {
                    RCReciever rc = player.gameObject.GetComponent<RCReciever>();
                    if (rc == null)
                        Debug.Log(player.UserNum + "'s RCReciever null");

                    if (player.UserNum == target)
                    {
                        rc.SetTargetPos(posX, posY, speedX, speedY, isFacingRight, rotZ);
                    }
                }
            }
            yield return null;
        }
    }*/
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = NetworkModule.GetInstance();        
        _networkManager.Connect("203.250.148.113",9993);
        
        _networkManager.AddOnEventSoloQueue(new OnSoloMatched());
        _networkManager.AddOnEventGameInfo(new OnGameInfo());
        _networkManager.AddOnEventGameStatus(new OnGameStatus());
        //networkManager.AddOnEventGameUserMove(new OnGameUserMove());
        //networkManager.AddOnEventGameInfo(new OnGameInfo());
        //networkManager.AddOnEventGameEvent(new OnGameAction());
        //networkManager.AddOnEventGameUserSync(new OnGameUserSync());              
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
