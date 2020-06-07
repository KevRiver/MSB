using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MSBNetwork;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;


public class MSB_GameManager : Singleton<MSB_GameManager>,
    MMEventListener<MMGameEvent>
{
    public enum Team
    {
        Blue = 0,
        Red
    }

    private int[] _score = new int[2];
    public int[] _death = new int[2];
    private int _index;
    private GameInfo _gameInfo;
    private UserRankListener _userRankListener;
    private OnGameStatus _onGameStatus;
    public void ScoreUpdate(int blueDeath, int redDeath,int blueScore, int redScore)
    {
        _death[0] = blueDeath;
        _death[1] = redDeath;
        _score[0] = blueScore;
        _score[1] = redScore;
        
        MSB_GUIManager.Instance.UpdateScoreSign(blueScore,redScore);
    }

    private const int FALSE = 0;
    private const int TRUE = 1;
    
    public string GameSet(int blueScore, int redScore)
    {
        string message = "";

        var team = MSB_LevelManager.Instance.TargetPlayer.team;

        int alliesScore = (team == Team.Blue) ? blueScore : redScore;
        int enemyScore = (team == Team.Blue) ? redScore : blueScore;
        if (alliesScore == enemyScore)
            message = "Draw";
        else
        {
            message = alliesScore > enemyScore ? "VICTORY" : "DEFEAT";
        }

        return message;
    }

    private IEnumerator ChangeScene(float duration)
    {
        yield return  new WaitForSeconds(duration);
        SceneManager.LoadScene("Lobby");
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Scenes/Lobby");
    }


    public int RoomNum { get; set; }
    [Header("Settings")]
    /// the target frame rate for the game
    public int TargetFrameRate = 300;

    protected override void Awake()
    {
        base.Awake();
        _gameInfo = GameInfo.Instance;
    }

    /// <summary>
    /// On Start(), sets the target framerate to whatever's been specified
    /// </summary>
    protected virtual void Start()
    {
        Application.targetFrameRate = TargetFrameRate;
    }

    /// <summary>
    /// this method resets the whole game manager
    /// </summary>
    public virtual void Reset()
    {
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Set, 1f, 0f, false, 0f, false);
    }

    public class UserRankListener : NetworkModule.OnStatusResultListener
    {
        private readonly MSB_GameManager _gameManager;
        public UserRankListener(MSB_GameManager gameManager)
        {
            this._gameManager = gameManager;
        }

        public void OnStatusResult(bool _result, UserData _user, int _game, string _message)
        {
            int savedRank = LocalUser.Instance.localUserData.userRank;
            int newRank = _user.userRank;
            int blueScore = _gameManager._score[0];
            int redScore = _gameManager._score[1];
            string result = _gameManager.GameSet(blueScore, redScore);
            GameResultViewData data = new GameResultViewData(savedRank,newRank,blueScore,redScore,result);
            MSB_GUIManager guiManager = MSB_GUIManager.Instance;
            guiManager.ViewActive(RESPAWNVIEW,false);
            guiManager.UIActive(false);
            guiManager.GameResultViewModel.Initialize(data);
            guiManager.ViewActive(0, true);
        }
    }

    public class OnGameStatus : NetworkModule.OnGameStatusListener
    {
        private readonly MSB_GUIManager _guiManager;
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
            Instance.ScoreUpdate(blueDeath, redDeath, bluePoint, redPoint);
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

    /*public class MedalDataListener : NetworkModule.OnGameStatusListener
    {
        public void OnGameEventCount(int count)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameEventTime(int time)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameEventReady(string readyData)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameEventMessage(int type, string message)
        {
            Debug.Log("OnGameEventMessage");
            if (type == 2)
            {
                int medalIndex = Convert.ToInt32(message);
                AchievementViewData data = new AchievementViewData(medalIndex);
                MSB_GUIManager.Instance.AchievementViewModel.Initialize(data);
            }
        }
    }*/

    /// <summary>
    /// Catches MMGameEvents and acts on them, playing the corresponding sounds
    /// </summary>
    /// <param name="gameEvent">MMGameEvent event.</param>
    public virtual void OnMMEvent(MMGameEvent gameEvent)
    {
        Debug.Log("On GameEvent : " + gameEvent.EventName);
        // Game 종료 이벤트에 대한 처리는 게임 매니저에서 다 처리
        switch (gameEvent.EventName)
        {
            case "GameStart":
                break;

            case "GameOver":
                OnGameOver();
                break;
        }
    }

    private const int RESPAWNVIEW = 2;
    private void OnGameOver()
    {
        string id = LocalUser.Instance.localUserData.userID;
        NetworkModule.GetInstance().RequestUserStatus(id);
        Destroy(GameInfo.Instance.gameObject);
    }
    

    /// <summary>
    /// OnDisable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        _userRankListener = new UserRankListener(_instance);
        _onGameStatus = new OnGameStatus(MSB_GUIManager.Instance);
        
        NetworkModule.GetInstance().AddOnEventUserStatus(_userRankListener);
        NetworkModule.GetInstance().AddOnEventGameStatus(_onGameStatus);
        
        Cursor.visible = true;
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        
        // MoveSync, ActionSync Listener Added when each RCReceiver's initialization
        // Clear when GameOver
        NetworkModule.GetInstance().SetOnEventGameUserMove(null);
        NetworkModule.GetInstance().SetOnEventGameUserSync(null);
        NetworkModule.GetInstance().SetOnEventUserStatus(null);
        NetworkModule.GetInstance().SetOnEventGameStatus(null);
        
        _instance = null;
    }
}