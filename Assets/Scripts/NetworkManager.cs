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
    NetworkModule networkManager;

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

        public IEnumerator LoadPlayScene(int _room, LinkedList<UserData> _users)
        {
            Debug.LogWarning("LoadPlayScene Called");
            /*MSB_GameManager.Instance.roomIndex = _room;
            MSB_GameManager.Instance.c_userData = new List<ClientUserData>();
            foreach (UserData user in _users)
            {
                //받아온 UserData들을 ClientUserData로 옮기고 옮긴 Data를 GameManager의 c_userData 리스트에 추가한다
                ClientUserData c_user = new ClientUserData();
                c_user.userID = user.userID;
                c_user.userNick = user.userNick;
                c_user.userNumber = user.userNumber;
                c_user.userSkin = user.userSkin;
                c_user.userWeapon = user.userWeapon;
                c_user.userRank = user.userRank;
                c_user.userMoney = user.userMoney;
                c_user.userCash = user.userCash;

                MSB_GameManager.Instance.c_userData.Add(c_user);
            }*/
            SceneManager.LoadScene("PlayScene");

            yield return null;
        }
    }
    /*
        private class OnGameStatus : NetworkModule.OnGameStatusListener
        {       
            public void OnGameEventCount(int count)
            {
                Debug.LogWarning("CountDown : " + count);

                if (count == 0)
                {

                    Debug.LogWarning("Game Start, Permit input");
                    UnityMainThreadDispatcher.Instance().Enqueue(PermitInput());
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(CountDown(count));
                }
            }

            public IEnumerator PermitInput()
            {
                //MSB_GUIManager.Instance.GameStartCounter.gameObject.SetActive(false); TODO
                InputManager.Instance.inputPermitted = true;
                yield return null;
            }

            public IEnumerator CountDown(int _count)
            {
                //MSB_GUIManager.Instance.GameStartCounter.text = _count.ToString(); TODO
                yield return null;
            }

            public void OnGameEventMessage(object _data)
            {

            }

            public void OnGameEventMessage(int type, string message)
            {

            }

            public void OnGameEventReady(string readyData)
            {            
                //Debug.Log("OnGameEventReady : " + readyData);
                //Debug.LogWarning("Before Parse JArray");
                JArray jarr = JArray.Parse(readyData);
                Debug.LogWarning("After Parse JArray");
                int userNum = 0;
                bool allPlayerReady = true;
                foreach (JObject jobj in jarr)
                {
                    ++userNum;
                    Debug.LogWarning("User " + userNum + " " + (bool)jobj.GetValue((userNum).ToString()));
                    if (!((bool)jobj.GetValue( (userNum).ToString() )))
                    {
                        allPlayerReady = false;
                    }
                }

                if (allPlayerReady)
                {
                    Debug.LogWarning("All Player Ready");
                }
            }

            public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
            {
                Debug.LogWarning("Blue K/D/S: " + blueKill + "/" + blueDeath + "/" + bluePoint + " " + "Red K/D/S: " + redKill + "/" + redDeath + "/" + redPoint);
                UnityMainThreadDispatcher.Instance().Enqueue(ScoreUpdate(blueKill, blueDeath, bluePoint, redKill, redDeath, redPoint));
            }

            public IEnumerator ScoreUpdate(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
            {
                List<ScoreSign> scoreSigns = MSB_GUIManager.Instance.scoreSigns;
                foreach (ScoreSign scoreSign in scoreSigns)
                {
                    if (scoreSign.team == ScoreSign.Team.Blue)
                    {
                        scoreSign.UpdateScore(bluePoint);
                        scoreSign.kill = blueKill;
                        scoreSign.death = blueDeath;
                    }
                    if (scoreSign.team == ScoreSign.Team.Red)
                    {
                        scoreSign.UpdateScore(redPoint);
                        scoreSign.kill = redKill;
                        scoreSign.death = redDeath;
                    }
                }
                yield return null;
            }       

            public void OnGameEventTime(int time)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(TimeCount(time));
            }

            public IEnumerator TimeCount(int time)
            {
                MSB_GUIManager GUIManager = MSB_GUIManager.Instance;
                Timer timer = GUIManager.timer;
                timer.UpdateTime(time);
                if (time == 0)
                {
                    //게임 시간이 끝나면 입력 안되게 하고 컨트롤러 UI를 끈다
                    Debug.LogWarning("TimeCount 0 input permitted ? " + InputManager.Instance.inputPermitted);
                    InputManager.Instance.inputPermitted = false;
                    foreach (CanvasGroup controller in MSB_GUIManager.Instance.controllers)
                    {
                        controller.gameObject.SetActive(false);
                    }

                    //Level 안의 캐릭터들에게 게임이 끝났음을 알린다
                    MSB_LevelManager levelManager = MSB_LevelManager.Instance;
                    List<MSB_Character> players = levelManager.MSB_Players;
                    int localPlayerIndex = 0;
                    foreach (MSB_Character player in players)
                    {
                        if (player.isLocalUser)
                        {
                            localPlayerIndex = player.playerIndex;
                        }
                        player.OnGameOver();
                    }
                    Debug.LogWarning("LocalPlayerIndex : " + localPlayerIndex);
                    int blueScore = MSB_GUIManager.Instance.scoreSigns[(int)ScoreSign.Team.Blue].currentScore;
                    int bluekill = MSB_GUIManager.Instance.scoreSigns[(int)ScoreSign.Team.Blue].kill;
                    int blueDeath = MSB_GUIManager.Instance.scoreSigns[(int)ScoreSign.Team.Blue].death;
                    Debug.LogWarning("Blue Team Score/Kill/Death  : " + blueScore + " / " + bluekill + " / " + blueDeath);

                    int redScore = MSB_GUIManager.Instance.scoreSigns[(int)ScoreSign.Team.Red].currentScore;
                    int redkill = MSB_GUIManager.Instance.scoreSigns[(int)ScoreSign.Team.Red].kill;
                    int redDeath = MSB_GUIManager.Instance.scoreSigns[(int)ScoreSign.Team.Red].death;
                    Debug.LogWarning("Red Team Score/Kill/Death  : " + redScore + " / " + redkill + " / " + redDeath);

                    GameObject resultObj = new GameObject();
                    resultObj.name = "ResultData";
                    resultObj.AddComponent<ResultData>();
                    ResultData resultData = resultObj.GetComponent<ResultData>();

                    if (blueScore > redScore)
                    {
                        if (localPlayerIndex == (int)ScoreSign.Team.Blue)
                        {
                            resultData.result = ResultData.Result.WIN;
                            resultData.point = blueScore;
                            resultData.kill = bluekill;
                            resultData.death = blueDeath;
                        }
                        else
                        {
                            resultData.result = ResultData.Result.LOSE;
                            resultData.point = redScore;
                            resultData.kill = redkill;
                            resultData.death = redDeath;
                        }


                    }
                    else if (blueScore < redScore)
                    {
                        if (localPlayerIndex == (int)ScoreSign.Team.Blue)
                        {
                            resultData.result = ResultData.Result.LOSE;
                            resultData.point = blueScore;
                            resultData.kill = bluekill;
                            resultData.death = blueDeath;
                        }
                        else
                        {
                            resultData.result = ResultData.Result.WIN;
                            resultData.point = redScore;
                            resultData.kill = redkill;
                            resultData.death = redDeath;
                        }

                    }
                    else
                    {
                        resultData.result = ResultData.Result.DRAW;
                        if (localPlayerIndex == (int)ScoreSign.Team.Blue)
                        {
                            resultData.point = blueScore;
                            resultData.kill = bluekill;
                            resultData.death = blueDeath;
                        }
                        else
                        {
                            resultData.point = redScore;
                            resultData.kill = redkill;
                            resultData.death = redDeath;
                        }
                    }
                    DontDestroyOnLoad(resultObj);
                    SceneManager.LoadScene("ResultScene");
                }
                yield return null;
            }
        }

        */
    private class OnGameUserMove : NetworkModule.OnGameUserMoveListener
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
                        rc.SyncUserPos(posX, posY, speedX, speedY, isFacingRight);
                    }
                }
            }
            yield return null;
        }
    }

    /*
        private class OnGameAction : NetworkModule.OnGameEventListener
        {                         
            public void OnGameEventHealth(int num, int health)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(ChangeHealth(num, health));
            }

            public IEnumerator ChangeHealth(int _num, int _health)
            {
                List<MSB_Character> players = MSB_LevelManager.Instance.MSB_Players;
                foreach (MSB_Character player in players)
                {
                    int userNum = player.c_userData.userNumber;
                    Health health = player.GetComponent<Health>();
                    if (userNum == _num)
                    {
                        health.ChangeHealth(_health);
                    }
                }
                yield return null;
            }

            public void OnGameEventDamage(int from, int to, int amount, string option)
            {
                Debug.LogWarning("OnGameEventDamage Called");
                UnityMainThreadDispatcher.Instance().Enqueue(OptionApply(to, option));
            }

            char[] delimiterChars = { ',' };
            public IEnumerator OptionApply( int _to, string _option)
            {
                string[] dataArray = ((string)_option).Split(delimiterChars);
                DamageOnTouch.MSBCCStyles causedCCType = (DamageOnTouch.MSBCCStyles)int.Parse(dataArray[0]);
                if (causedCCType == DamageOnTouch.MSBCCStyles.NoCC)
                {
                    Debug.LogWarning("OptionApply : NoCC Condition");
                    yield break;
                }

                CharacterStates.CharacterConditions activateCondition = (CharacterStates.CharacterConditions)int.Parse(dataArray[1]);

                MSB_Character target = new MSB_Character();
                Health targetHealth = new Health();
                CharacterStates.CharacterConditions targetCondition;
                foreach (MSB_Character player in MSB_LevelManager.Instance.MSB_Players)
                {
                    if (player.c_userData.userNumber == _to)
                    {
                        target = player;
                    }
                }
                /*
                if (target.GetComponent<Health>() == null)
                {
                    Debug.LogWarning("OptionApply : Health Null Condition");
                    yield break;
                }
                targetHealth = target.GetComponent<Health>();

                if (targetHealth.ImmuneToKnockback)
                {
                    Debug.LogWarning("OptionApply : Immune Condition");
                    yield break;
                }

                targetCondition = target.ConditionState.CurrentState;
                if (activateCondition != targetCondition)
                {
                    Debug.LogWarning("OptionApply : Target Coniditon not matched : " + targetCondition.ToString());
                    yield break;
                }


                float _duration = 0;
                float _movementMultiplier = 1.0f;
                float _dirX = 0;
                float _dirY = 0;

                switch (causedCCType)
                {
                    case DamageOnTouch.MSBCCStyles.KnockBack:
                        Debug.LogWarning("CC KnockBack");
                        _dirX = float.Parse(dataArray[3]);
                        _dirY = float.Parse(dataArray[4]);

                        Debug.LogWarning("KnockBack Dir : " + _dirX + " , " + _dirY);
                        //target.Stun();
                        target._controller.SetForce(new Vector2(_dirX, _dirY));
                        break;
                    case DamageOnTouch.MSBCCStyles.Slow:
                        Debug.LogWarning("CC Slow");
                        _duration = float.Parse(dataArray[2]);
                        _movementMultiplier = float.Parse(dataArray[3]);

                        target._duration = _duration;
                        target.Slow(_movementMultiplier);
                        break;
                    case DamageOnTouch.MSBCCStyles.Stun:
                        Debug.LogWarning("CC Stun");
                        _duration = float.Parse(dataArray[2]);
                        target._duration = _duration;
                        target.Stun();
                        break;

                }

                yield return null;
            }

            public void OnGameEventObject(int num, int health)
            {

            }

            public void OnGameEventItem(int type, int num, int action)
            {

            }

            public void OnGameEventKill(int from, int to, string option)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(KillPlayer(from, to, option)); 
            }

            public IEnumerator KillPlayer(int _from, int _to, string _option)
            {            
                MSB_LevelManager levelManager = MSB_LevelManager.Instance;
                MSB_Character target = new MSB_Character();
                MSB_Character attacker = new MSB_Character();
                List<MSB_Character> users = levelManager.MSB_Players;
                foreach (MSB_Character user in users)
                {
                    if (user.c_userData.userNumber == _to)
                    {
                        target = user;
                    }

                    if (user.c_userData.userNumber == _from)
                    {
                        attacker = user;
                    }
                }

                levelManager.KillPlayer(target);
                GameObject.Find("MessageObject").GetComponent<MessageHandler>().DisplayKillLog(attacker, target);
                yield return null;
            }

            public void OnGameEventRespawn(int num, int time)
            {
                Debug.LogWarning("User : " + num + " " + "Respawn Time : " + time);
                if (time == 0)
                {               
                    MSB_LevelManager.Instance.RespawnPlayer(num);
                }
            }
        }

        private class OnGameUserSync : NetworkModule.OnGameUserSyncListener
        {
            char[] delimiterChars = { ',' };
            void NetworkModule.OnGameUserSyncListener.OnGameUserSync(object _data)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(PlayerActionSync(_data));
            }

            public IEnumerator PlayerActionSync(object _data)
            {
                string[] dataArray = ((string)_data).Split(delimiterChars);
                int userNumber = int.Parse(dataArray[0]);
                int actionType = int.Parse(dataArray[1]);
                bool actionAimable = bool.Parse(dataArray[2]);
                float actionDirX = float.Parse(dataArray[3]);
                float actionDirY = float.Parse(dataArray[4]);

                if (userNumber == LocalUser.Instance.localUserData.userNumber)
                    yield break;

                foreach (MSB_Character player in MSB_LevelManager.Instance.MSB_Players)
                {
                    if (player.c_userData.userNumber == userNumber)
                    {
                        //Debug.LogWarning(player.c_userData.userNick + " Recieved Action Data : " + actionType + " , " + actionState);
                        player.RecievedActionType = (CharacterAbility.ActionType)actionType;
                        player.RecievedActionAimable = actionAimable;
                        player.RecievedActionDirX = actionDirX;
                        player.RecievedActionDirY = actionDirY;
                        //player.RecievedActionState = actionState;
                    }
                }
                yield return null;
            }
        }


        */
        private void Awake()
        {
            //Make NetworkManager don't destroyed on load
            DontDestroyOnLoad(gameObject);

            /*portData = FindObjectOfType<PortData>();
            if (portData == null)
            {
                Debug.LogWarning("PortData not found");
                //Port = 8888;
                Port = 80;
                return;
            }

            Debug.Log("LobbyScene PortData : " + portData.port);
            Port = portData.port;
            Destroy(portData.gameObject);
            */
        }
    
    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkModule.GetInstance();        
        networkManager.Connect("203.250.148.113",9993);
        
        networkManager.AddOnEventSoloQueue(new OnSoloMatched());
        networkManager.AddOnEventGameInfo(new OnGameInfo());
        //networkManager.AddOnEventGameStatus(new OnGameStatus());
        networkManager.AddOnEventGameUserMove(new OnGameUserMove());
        //networkManager.AddOnEventGameInfo(new OnGameInfo());
        //networkManager.AddOnEventGameEvent(new OnGameAction());
        //networkManager.AddOnEventGameUserSync(new OnGameUserSync());              
    }

    // Update is called once per frame
    void Update()
    {
        networkManager.OnUpdate();
    }

    private void OnDestroy()
    {
        networkManager.Disconnect();
    }
}
