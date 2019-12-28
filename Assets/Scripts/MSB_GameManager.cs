using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;
using MSBNetwork;
using UnityEngine.SceneManagement;


public class MSB_GameManager : Singleton<MSB_GameManager>,
    MMEventListener<MMGameEvent>,
    MMEventListener<CorgiEngineEvent>,
    MMEventListener<CorgiEnginePointsEvent>
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
       if (blueDeath == TRUE && redDeath == TRUE)
       {
           Debug.LogWarning("Have some error");
           return;
       }

       int blueScore = score[0];
       int redScore = score[1];
       string message = "";
       MessageBoxStyles messageBoxStyle;
       
       const float duration = 5.0f;
       if (blueDeath == TRUE)
       {
           messageBoxStyle = MessageBoxStyles.Red;
           message = "RED TEAM\nWIN";
       }
       else if (redDeath == TRUE)
       {
           messageBoxStyle = MessageBoxStyles.Blue;
           message = "BLUE TEAM\nWIN";
       }
       else
       {
           if (blueScore > redScore)
           {
               messageBoxStyle = MessageBoxStyles.Blue;
               message = "BLUE TEAM\nWIN";
           }
           else if (blueScore == redScore)
           {
               messageBoxStyle = MessageBoxStyles.Green;
               message = "DRAW";
           }
           else
           {
               messageBoxStyle = MessageBoxStyles.Red;
               message = "RED TEAM\nWIN";
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
        SceneManager.LoadSceneAsync("Scenes/Lobby");
    }


    public int RoomNum { get; set; }
    [Header("Settings")]
    /// the target frame rate for the game
    public int TargetFrameRate = 300;
    /*the maximum amount of lives the character can currently have
    public int MaximumLives = 0;
    /// the current number of lives 
    public int CurrentLives = 0;
    /// the name of the scene to redirect to when all lives are lost
    public string GameOverScene;

    /// the current number of game points
    public int Points { get; private set; }
    /// true if the game is currently paused
    public bool Paused { get; set; }
    // true if we've stored a map position at least once
    public bool StoredLevelMapPosition { get; set; }
    /// the current player
    public Vector2 LevelMapPosition { get; set; }
    /// the stored selected character
    public Character StoredCharacter { get; set; }
    /// the list of points of entry and exit
    public List<PointsOfEntryStorage> PointsOfEntry { get; set; }*/

    /*protected bool _inventoryOpen = false;
    protected bool _pauseMenuOpen = false;
    protected InventoryInputManager _inventoryInputManager;
    protected int _initialMaximumLives;
    protected int _initialCurrentLives;*/

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
       // _initialCurrentLives = CurrentLives;
        //_initialMaximumLives = MaximumLives;
    }

    /// <summary>
    /// this method resets the whole game manager
    /// </summary>
    public virtual void Reset()
    {
        //Points = 0;
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Set, 1f, 0f, false, 0f, false);
        //Paused = false;
        //GUIManager.Instance.RefreshPoints();
        //PointsOfEntry.Clear();
    }

    /// <summary>
    /// Use this method to decrease the current number of lives
    /// </summary>
    /*public virtual void LoseLife()
    {
        CurrentLives--;
    }

    /// <summary>
    /// Use this method when a life (or more) is gained
    /// </summary>
    /// <param name="lives">Lives.</param>
    public virtual void GainLives(int lives)
    {
        CurrentLives += lives;
        if (CurrentLives > MaximumLives)
        {
            CurrentLives = MaximumLives;
        }
    }

    /// <summary>
    /// Use this method to increase the max amount of lives, and optionnally the current amount as well
    /// </summary>
    /// <param name="lives">Lives.</param>
    /// <param name="increaseCurrent">If set to <c>true</c> increase current.</param>
    public virtual void AddLives(int lives, bool increaseCurrent)
    {
        MaximumLives += lives;
        if (increaseCurrent)
        {
            CurrentLives += lives;
        }
    }

    /// <summary>
    /// Resets the number of lives to their initial values.
    /// </summary>
    public virtual void ResetLives()
    {
        CurrentLives = _initialCurrentLives;
        MaximumLives = _initialMaximumLives;
    }

    /// <summary>
    /// Adds the points in parameters to the current game points.
    /// </summary>
    /// <param name="pointsToAdd">Points to add.</param>
    public virtual void AddPoints(int pointsToAdd)
    {
        Debug.Log("AddPoints called");
        //Points += pointsToAdd;
        //GUIManager.Instance.RefreshPoints();
    }

    /// <summary>
    /// use this to set the current points to the one you pass as a parameter
    /// </summary>
    /// <param name="points">Points.</param>
    public virtual void SetPoints(int points)
    {
        Debug.Log("SetPoints called");
        Points = points;
        //GUIManager.Instance.RefreshPoints();
    }

    protected virtual void SetActiveInventoryInputManager(bool status)
    {
        _inventoryInputManager = GameObject.FindObjectOfType<InventoryInputManager>();
        if (_inventoryInputManager != null)
        {
            _inventoryInputManager.enabled = status;
        }
    }

    /// <summary>
    /// Pauses the game or unpauses it depending on the current state
    /// </summary>
    public virtual void Pause(PauseMethods pauseMethod = PauseMethods.PauseMenu)
    {
        Debug.Log("Pause called");
        if ((pauseMethod == PauseMethods.PauseMenu) && _inventoryOpen)
        {
            return;
        }

        // if time is not already stopped		
        /*
        if (Time.timeScale > 0.0f)
        {
            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, false, 0f, true);
            Instance.Paused = true;
            if ((GUIManager.Instance != null) && (pauseMethod == PauseMethods.PauseMenu))
            {
                GUIManager.Instance.SetPause(true);
                _pauseMenuOpen = true;
                SetActiveInventoryInputManager(false);
            }
            if (pauseMethod == PauseMethods.NoPauseMenu)
            {
                _inventoryOpen = true;
            }
        }
        else
        {
            UnPause(pauseMethod);
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);
        }
        LevelManager.Instance.ToggleCharacterPause();
        
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    public virtual void UnPause(PauseMethods pauseMethod = PauseMethods.PauseMenu)
    {
        Debug.Log("UnPause called");
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
        Instance.Paused = false;
        if ((GUIManager.Instance != null) && (pauseMethod == PauseMethods.PauseMenu))
        {
            GUIManager.Instance.SetPause(false);
            _pauseMenuOpen = false;
            SetActiveInventoryInputManager(true);
        }
        if (_inventoryOpen)
        {
            _inventoryOpen = false;
        }
    }

    /// <summary>
    /// Deletes all save files
    /// </summary>
    public virtual void ResetAllSaves()
    {
        SaveLoadManager.DeleteSaveFolder("InventoryEngine");
        SaveLoadManager.DeleteSaveFolder("CorgiEngine");
        SaveLoadManager.DeleteSaveFolder("MMAchievements");
        SaveLoadManager.DeleteSaveFolder("MMRetroAdventureProgress");
    }

    /// <summary>
    /// Stores the points of entry for the level whose name you pass as a parameter.
    /// </summary>
    /// <param name="levelName">Level name.</param>
    /// <param name="entryIndex">Entry index.</param>
    /// <param name="exitIndex">Exit index.</param>
    public virtual void StorePointsOfEntry(string levelName, int entryIndex, Character.FacingDirections facingDirection)
    {
        if (PointsOfEntry.Count > 0)
        {
            foreach (PointsOfEntryStorage point in PointsOfEntry)
            {
                if (point.LevelName == levelName)
                {
                    point.PointOfEntryIndex = entryIndex;
                    return;
                }
            }
        }

        PointsOfEntry.Add(new PointsOfEntryStorage(levelName, entryIndex, facingDirection));
    }

    /// <summary>
    /// Gets point of entry info for the level whose scene name you pass as a parameter
    /// </summary>
    /// <returns>The points of entry.</returns>
    /// <param name="levelName">Level name.</param>
    public virtual PointsOfEntryStorage GetPointsOfEntry(string levelName)
    {
        if (PointsOfEntry.Count > 0)
        {
            foreach (PointsOfEntryStorage point in PointsOfEntry)
            {
                if (point.LevelName == levelName)
                {
                    return point;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Clears the stored point of entry infos for the level whose name you pass as a parameter
    /// </summary>
    /// <param name="levelName">Level name.</param>
    public virtual void ClearPointOfEntry(string levelName)
    {
        if (PointsOfEntry.Count > 0)
        {
            foreach (PointsOfEntryStorage point in PointsOfEntry)
            {
                if (point.LevelName == levelName)
                {
                    PointsOfEntry.Remove(point);
                }
            }
        }
    }

    /// <summary>
    /// Clears all points of entry.
    /// </summary>
    public virtual void ClearAllPointsOfEntry()
    {
        PointsOfEntry.Clear();
    }

    /// <summary>
    /// Stores the selected character for use in upcoming levels
    /// </summary>
    /// <param name="selectedCharacter">Selected character.</param>
    public virtual void StoreSelectedCharacter(Character selectedCharacter)
    {
        StoredCharacter = selectedCharacter;
    }

    /// <summary>
    /// Clears the selected character.
    /// </summary>
    public virtual void ClearSelectedCharacter()
    {
        StoredCharacter = null;
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
                MSB_GUIManager.Instance.OnGameOver();
                GameSet(_death,_score);
                Destroy(GameObject.Find("GameInfo"));
                //StartCoroutine(ChangeScene(5.0f));
                Invoke("ChangeScene",5.0f);
                break;
        }
    }

    /// <summary>
    /// Catches CorgiEngineEvents and acts on them, playing the corresponding sounds
    /// </summary>
    /// <param name="engineEvent">CorgiEngineEvent event.</param>
    public virtual void OnMMEvent(CorgiEngineEvent engineEvent)
    {
        Debug.Log("On CorgiEngine Event : " + engineEvent.EventType);
        switch (engineEvent.EventType)
        {
            case CorgiEngineEventTypes.Pause:
                //Pause();
                break;

            case CorgiEngineEventTypes.UnPause:
                //UnPause();
                break;
        }
    }

    /// <summary>
    /// Catches CorgiEnginePointsEvents and acts on them, playing the corresponding sounds
    /// </summary>
    /// <param name="pointEvent">CorgiEnginePointsEvent event.</param>
    public virtual void OnMMEvent(CorgiEnginePointsEvent pointEvent)
    {
        Debug.Log("On Points Event");
        switch (pointEvent.PointsMethod)
        {
            case PointsMethods.Set:
                //SetPoints(pointEvent.Points);
                break;

            case PointsMethods.Add:
                //AddPoints(pointEvent.Points);
                break;
        }
    }

    /// <summary>
    /// OnDisable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        this.MMEventStartListening<CorgiEngineEvent>();
        this.MMEventStartListening<CorgiEnginePointsEvent>();
        Cursor.visible = true;
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        this.MMEventStopListening<CorgiEngineEvent>();
        this.MMEventStopListening<CorgiEnginePointsEvent>();
        NetworkModule.GetInstance().ClearOnEventGameEvent();
        NetworkModule.GetInstance().ClearOnEventGameUserMove();
        NetworkModule.GetInstance().ClearOnEventGameUserSync();
    }
}