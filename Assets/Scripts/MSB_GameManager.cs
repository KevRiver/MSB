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
    
    public void GameSet(int[] death, int[] score)
    {
        int blueDeath = death[0];
        int redDeath = death[1]; 
        Debug.LogWarning("BlueDeath : " + blueDeath + " RedDeath : " + redDeath);

        int blueScore = score[0];
       int redScore = score[1];
       string message = "";
       MessageBoxStyles messageBoxStyle;
       Team _team;

       _team = MSB_LevelManager.Instance.TargetPlayer.team;
       
       if (blueScore > redScore)
       {
           messageBoxStyle = MessageBoxStyles.Blue;
           message = "YOU WIN";
           if (_team == Team.Red)
           {
               messageBoxStyle = MessageBoxStyles.Red;
               message = "YOU LOSE";
           }
       }
       else if (blueScore == redScore)
       {
           messageBoxStyle = MessageBoxStyles.Green;
           message = "DRAW";
       }
       else
       {
           messageBoxStyle = MessageBoxStyles.Blue;
           message = "YOU LOSE";
           if (_team == Team.Red)
           {
               messageBoxStyle = MessageBoxStyles.Red;
               message = "YOU WIN";
           }
        }
       
       MSB_GUIManager.Instance.UpdateMessageBox(messageBoxStyle, message, 0);
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
        //Debug.Log("MSB_GameManager Awake");
        base.Awake();
        //PointsOfEntry = new List<PointsOfEntryStorage>();
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
                MSB_GUIManager.Instance.OnGameOver();
                GameSet(_death,_score);
                Destroy(GameObject.Find("GameInfo"));
                Invoke("ChangeScene",3.0f);
                break;
        }
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
    }
}