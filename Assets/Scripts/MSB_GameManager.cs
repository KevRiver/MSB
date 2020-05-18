using System;
using System.Collections;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MSBNetwork;
using UnityEngine.SceneManagement;


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
    private MedalDataListener _medalDataListener;
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
        _userRankListener = new UserRankListener(_instance);
        _medalDataListener = new MedalDataListener();
        NetworkModule.GetInstance().AddOnEventUserStatus(_userRankListener);
        NetworkModule.GetInstance().AddOnEventGameStatus(_medalDataListener);
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
            guiManager.UIActive(false);
            guiManager.GameResultViewModel.Initialize(data);
            guiManager.ViewActive(0, true);
        }
    }

    public class MedalDataListener : NetworkModule.OnGameStatusListener
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
            if (type == 2)
            {
                int medalIndex = Convert.ToInt32(message);
                AchievementViewData data = new AchievementViewData(medalIndex);
                MSB_GUIManager.Instance.AchievementViewModel.Initialize(data);
            }
        }
    }

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
                
                /*Destroy(_gameInfo.gameObject);
                Invoke("ChangeScene",3.0f);*/
                break;
        }
    }
    private void OnGameOver()
    {
        string id = LocalUser.Instance.localUserData.userID;
        NetworkModule.GetInstance().RequestUserStatus(id);
    }

    IEnumerator WaitForChangeScene(float delay)
    {
        float time = 0f;
        while (time < delay)
        {
            time += Time.deltaTime;
            yield return null;
        }

        ChangeScene();
    }

    /// <summary>
    /// OnDisable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        Cursor.visible = true;
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        NetworkModule.GetInstance().SetOnEventGameEvent(null);
        NetworkModule.GetInstance().SetOnEventGameUserMove(null);
        NetworkModule.GetInstance().SetOnEventGameUserSync(null);
        NetworkModule.GetInstance().SetOnEventUserStatus(null);
        NetworkModule.GetInstance().SetOnEventGameStatus(null);
    }
}