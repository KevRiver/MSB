using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using MSBNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine.SocialPlatforms.Impl;

public enum MessageBoxStyles
{
    Plain,
    Blue,
    Red,
    Green,
    Small
}
public class MSB_GUIManager : Singleton<MSB_GUIManager>,MMEventListener<MMGameEvent>
{
    // 타이머
    // 중앙 메세지 박스
    // 스코어 전광판

    

    public int initialTime;
    private int _curTime;
    public List<string> msgSequence;
    
    public Text Timer;
    public Image TimerImage;
    private bool _timeStop;
    private int _min;
    private string _minString;
    private int _sec;
    private string _secString;
    public Text ScoreSign;
    public Text BlueScore, RedScore;
    public Text MessageBox;
    public Text MessageBox1;
    public Text MessageBox2;
    public Text MessageBox3;
    public Text MessageBoxSmall;
    public Image Joystick;
    public Image AttackButton;

    private List<GameObject> _uiContainer;
    private List<Text> _messageBoxes;

    protected  override  void Awake()
    {
        base.Awake();
        gameObject.name = "GUIManager";
    }
    
    void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        //Debug.LogWarning("GUIManager  Init");
        BlueScore.text = "0";
        RedScore.text = "0";
        _min = initialTime / 60;
        _minString = (_min >= 10) ? _min.ToString() : "0" + _min.ToString();
        _sec = initialTime % 60;
        _secString = (_sec >= 10) ? _sec.ToString() : "0" + _sec.ToString();
        Timer.text = _minString + ":" + _secString;
        _timeStop = false;

        _messageBoxes = new List<Text>();
        _messageBoxes.Add(MessageBox);
        _messageBoxes.Add(MessageBox1);
        _messageBoxes.Add(MessageBox2);
        _messageBoxes.Add(MessageBox3);
        _messageBoxes.Add(MessageBoxSmall);
        
        // MessageBox를 제외한 UI 들을 저장
        _uiContainer = new List<GameObject>();
        _uiContainer.Add(Timer.gameObject);
        _uiContainer.Add(TimerImage.gameObject);
        _uiContainer.Add(ScoreSign.gameObject);
        _uiContainer.Add(Joystick.gameObject);
        _uiContainer.Add(AttackButton.gameObject);
    }

    public void OnGameOver()
    {
        //_timeStop = true;
        UIActive(false);
    }

    public void UIActive(bool active)
    {
        foreach (var ui in _uiContainer)
        {
            ui.SetActive(active);
        }
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

    public void UpdateMessageBox(int _seq)
    {
        MessageBox.text = msgSequence[_seq];
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
        if (!MessageBox.enabled)
            MessageBox.enabled = true;
        
        MessageBox.text = message;
        if (duration > 0)
            Invoke("MessageBoxReset",duration);
    }

    public void UpdateMessageBox(MessageBoxStyles style, string message, float duration)
    {
        Text messagebox = _messageBoxes[(int) style];
        if (!messagebox.enabled)
            messagebox.enabled = true;

        messagebox.text = message;
        if (duration > 0)
            StartCoroutine(MessageBoxReset(style,duration));

    }

    private void MessageBoxReset()
    {
        MessageBox.text = "";
    }

    private IEnumerator MessageBoxReset(MessageBoxStyles style,float duration)
    {
        yield return new WaitForSeconds(duration);
        _messageBoxes[(int) style].text = "";
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                break;
            
            case "HurryUp":
                ChangeTimerColor(Color.red);
                UpdateMessageBox(MessageBoxStyles.Small,"게임이 10초 남았습니다",2.5f);
                break;
            
            case "GameOver":
                break;
        }
    }
}
