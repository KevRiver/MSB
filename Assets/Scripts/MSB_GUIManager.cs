#define GUI_LOG_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using MSBNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public enum MessageBoxStyles
{
    Count,
    Mission,
    Alarm
}
public class MSB_GUIManager : Singleton<MSB_GUIManager>,MMEventListener<MMGameEvent>
{
    // 타이머
    // 중앙 메세지 박스
    // 스코어 전광판
    public int initialTime;
    private int _curTime;
    public List<string> msgSequence;

    public Canvas rootCanvas;
    private CanvasScaler scaler;
    
    public Text Timer;
    private bool _timeStop;
    private int _min;
    private string _minString;
    private int _sec;
    private string _secString;
    
    public Text ScoreSign;
    public Text BlueScore, RedScore;
    public Text CountTextBox;
    public Text MissionTextBox;
    public Text AlarmTextBox;
    public Text YoureBlueTeam;
    public Text YoureRedTeam;
    public Image Joystick;
    public Image AttackButton;
    public Image Cover;
    public Image SecondCover;
    public LoadingView LoadingViewModel;
    public GameResultView GameResultViewModel;
    public AchievementView AchievementViewModel;
    public RespawnView RespawnViewModel;
    
    public GameObject[] _viewContainer;
    
    private List<GameObject> _uiContainer;
    private List<Text> _messageBoxes;

    private int screenWidth;
    private int screenHeight;

    private float multiplier;
    protected  override  void Awake()
    {
        base.Awake();
        gameObject.name = "GUIManager";
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        multiplier = DetermineScreenMultiplier();
        if (!rootCanvas)
            return;
        scaler = rootCanvas.GetComponent<CanvasScaler>();
        ScreenRescale(multiplier);
        JoyStickResize();
    }
    
    void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        // Init Score
        BlueScore.text = "0";
        RedScore.text = "0";
        
        // Init Timer
        _min = initialTime / 60;
        _minString = (_min >= 10) ? _min.ToString() : "0" + _min.ToString();
        _sec = initialTime % 60;
        _secString = (_sec >= 10) ? _sec.ToString() : "0" + _sec.ToString();
        Timer.text = _minString + ":" + _secString;
        _timeStop = false;

        _messageBoxes = new List<Text>();
        _messageBoxes.Add(CountTextBox);
        _messageBoxes.Add(MissionTextBox);
        _messageBoxes.Add(AlarmTextBox);
        
        _uiContainer = new List<GameObject>();
        _uiContainer.Add(Timer.gameObject);
        _uiContainer.Add(ScoreSign.gameObject);
        _uiContainer.Add(Joystick.gameObject);
        _uiContainer.Add(AttackButton.gameObject);
    }

    public void CoverActive(bool active)
    {
        Cover.gameObject.SetActive(active);
    }
    
    
    public IEnumerator SecondCoverFadeIn(float duration)
    {
        if(!SecondCover.gameObject.activeInHierarchy)
            SecondCover.gameObject.SetActive(true);
        
        float progress = 0;
        Color color = SecondCover.color;
        while (progress / duration < 1)
        {
            progress += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1.0f, progress / duration);
            SecondCover.color = color;
            yield return null;
        }
    }

    public void SetCover( Color color, float alpha)
    {
        Color colorApply = color;
        colorApply.a = alpha;
        Cover.color = colorApply;
    }

    private float DetermineScreenMultiplier()
    {
        if (screenWidth < 1300)
            return 1;

        if (screenWidth < 2000)
            return 1.5f;

        if (screenWidth < 2600)
            return 2.5f;
        return 2.8f;
    }

    private void ScreenRescale(float multiplier)
    {
        if (!scaler)
            return;
        scaler.scaleFactor = multiplier;
    }

    private void JoyStickResize()
    {
        var joystickRt = Joystick.GetComponent<RectTransform>();
        if (!joystickRt)
            return;
        joystickRt.SetLeft(-(screenWidth/(4*multiplier)));
        joystickRt.SetBottom(-(screenHeight * (0.8f / multiplier)));
    }
    
    public void UIActive(bool active)
    {
        foreach (var ui in _uiContainer)
        {
            ui.SetActive(active);
        }
    }

    public void ViewActive(int viewIndex,bool active)
    {
        if (viewIndex >= _viewContainer.Length)
            return;
        _viewContainer[viewIndex].SetActive(active);
    }

    public void UpdateScoreSign(int b, int r)
    {
        BlueScore.text = b.ToString();
        RedScore.text = r.ToString();
    }
    public void UpdateTimer(int time)
    {
        if (_timeStop)
            return;
        
        _curTime = time;
        _min = _curTime / 60;
        _minString = (_min >= 10) ? _min.ToString() : "0" + _min.ToString();
        _sec = _curTime % 60;
        _secString = (_sec >= 10) ? _sec.ToString() : "0" + _sec.ToString();
        Timer.text = _minString + ":" + _secString;
    }

    public void ChangeTimerColor(Color color)
    {
        Timer.color = color;
    }

    public void ActiveMessageBox(bool activate)
    {
        CountTextBox.gameObject.SetActive(activate);
    }

    public void UpdateMessageBox(int _seq)
    {
        CountTextBox.text = msgSequence[_seq];
        if (_seq == 0)
            Invoke("MessageBoxReset", 0.5f);
    }
    
    /// <summary>
    /// 메세지 박스에 메세지를 출력합니다
    /// </summary>
    /// <param name="message"> 출력할 메세지 </param>
    /// <param name="duration"> -1 메세지 출력 유지 / 메세지 출력 유지 시간</param>
    public void UpdateMessageBox(string message, float duration)
    {
        if (!CountTextBox.enabled)
            CountTextBox.enabled = true;
        
        CountTextBox.text = message;
        if (duration > 0)
            Invoke("MessageBoxReset",duration);
    }

    public void UpdateMessageBox(MessageBoxStyles style, string message, float duration)
    {
        Text messagebox = _messageBoxes[(int) style];
        if (!messagebox.gameObject.activeInHierarchy)
            messagebox.gameObject.SetActive(true);

        messagebox.text = message;
        if (duration > 0)
            StartCoroutine(MessageBoxReset(style, duration));

    }

    public delegate void TextBoxDelegate(MessageBoxStyles style, string msg);

    private void MessageBoxReset()
    {
        CountTextBox.text = "";
    }

    private void MessageBoxReset(MessageBoxStyles type)
    {
        _messageBoxes[(int) type].text = "";
    }

    private IEnumerator MessageBoxReset(MessageBoxStyles style,float duration)
    {
        yield return new WaitForSeconds(duration);
        _messageBoxes[(int) style].text = "";
    }

    public void ChangeMessageBoxColor(MessageBoxStyles type, Color color)
    {
        _messageBoxes[(int) type].color = color;
    }

    private void SetPlayerTeamSign(MSB_GameManager.Team team)
    {
        if(team == MSB_GameManager.Team.Blue)
            YoureBlueTeam.gameObject.SetActive(true);
        else if (team == MSB_GameManager.Team.Red)
        {
            YoureRedTeam.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
        _instance = null;
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                Cover.gameObject.SetActive(false);
                SetPlayerTeamSign(MSB_LevelManager.Instance.TargetPlayer.team);
                MissionTextBox.text = "";
                break;
            
            case "HurryUp":
                ChangeTimerColor(Color.red);
                UpdateMessageBox(MessageBoxStyles.Alarm,"GAME ENDS AFTER 10 SECONDS!",1.5f);
                break;
            
            case "GameOver":
                Cover.gameObject.SetActive(true);
                break;
        }
    }
}
