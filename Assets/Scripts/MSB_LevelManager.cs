using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine.SceneManagement;
using MSBNetwork;



public class MSB_LevelManager : Singleton<MSB_LevelManager>
{
    /// the prefab you want for your player
    [Header("Playable Characters")]
    [Information("The LevelManager is responsible for handling spawn/respawn, checkpoints management and level bounds. Here you can define one or more playable characters for your level..", InformationAttribute.InformationType.Info, false)]
    /// the list of player prefabs to instantiate
    public List<MSB_Character> PlayerPrefabs;
    /// should the player IDs be auto attributed (usually yes)

    /*[Space(10)]
    //[Header("Intro and Outro durations")]
    //[Information("Here you can specify the length of the fade in and fade out at the start and end of your level. You can also determine the delay before a respawn.", InformationAttribute.InformationType.Info, false)]
    /// duration of the initial fade in (in seconds)
    //public float IntroFadeDuration = 1f;
    /// duration of the fade to black at the end of the level (in seconds)
    //public float OutroFadeDuration = 1f;
    /// the ID to use when triggering the event (should match the ID on the fader you want to use)
    //public int FaderID = 0;
    /// the curve to use for in and out fades
    //public MMTween.MMTweenCurve FadeCurve = MMTween.MMTweenCurve.EaseInCubic;
    /// duration between a death of the main character and its respawn
    //public float RespawnDelay = 2f;*/


    [Space(10)]
    [Header("Level Bounds")]
    [Information("The level bounds are used to constrain the camera's movement, as well as the player character's. You can see it in real time in the scene view as you adjust its size (it's the yellow box).", InformationAttribute.InformationType.Info, false)]
    /// the level limits, camera and player won't go beyond this point.
    public Bounds LevelBounds = new Bounds(Vector3.zero, Vector3.one * 10);

    [InspectorButton("GenerateColliderBounds")]
    public bool ConvertToColliderBoundsButton;
    public Collider BoundsCollider { get; protected set; }

    /// the elapsed time since the start of the level
    public TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }
    public CameraController LevelCameraController { get; set; }

    // private stuff
    public List<MSB_Character> Players { get; protected set; }
    private MSB_Character TargetPlayer;
    //public List<CheckPoint> Checkpoints { get; protected set; }
    public List<MSB_SpawnPoint> Spawnpoints { get; protected set; }
    protected DateTime _started;
    private GameInfo gameInfo;

    /// <summary>
    /// On awake, instantiates the player
    /// </summary>
    protected override void Awake()
    {
        //Debug.Log("MSB_LevelManager Awake");
        base.Awake();

        gameInfo = GameInfo.Instance;
        if (gameInfo != null)
        {
            //Debug.Log("Check GameInfo exist");
            foreach (PlayerInfo player in gameInfo.players)
            {
                //Debug.Log(player.nick + " " + player.number);
            }
        }
        else
            return;

        Debug.LogWarning("PlayerInfo : " + gameInfo.players);

        InstantiatePlayableCharacters(gameInfo.players);
    }

    /// <summary>
    /// Instantiate playable characters based on UserData.userWeapon
    /// </summary>

    const int WEAPON_SWORD = 0;
    const int WEAPON_SHURIKEN = 1;
    protected virtual void InstantiatePlayableCharacters(List<PlayerInfo> users)
    {
        int localUserNum = LocalUser.Instance.localUserData.userNumber;
        Debug.Log("local user number : " + localUserNum);

        Players = new List<MSB_Character>();        
        if (PlayerPrefabs == null) { return; }
      
        // player instantiation
        if (PlayerPrefabs.Count != 0)
        {
            foreach (PlayerInfo user in users)
            {
                MSB_Character newPlayer = (MSB_Character)Instantiate(PlayerPrefabs[user.weapon], new Vector3(0, 0, 0), Quaternion.identity);
                newPlayer.UserNum = user.number;

                //check remote players and add RCReciever component to its gameobject
                if (newPlayer.UserNum != localUserNum)
                {
                    newPlayer.gameObject.AddComponent<RCReciever>();
                    Debug.Log("RCReciever added to " + newPlayer.name);
                }
                else
                {
                    newPlayer.SetPlayerID("LocalPlayer");
                    TargetPlayer = newPlayer;

                    RCSender rcSender = RCSender.Instance;
                    rcSender.Initialize(newPlayer);
                }

                Players.Add(newPlayer);
            }
        }
        else
        {
            //Debug.LogWarning ("LevelManager : The Level Manager doesn't have any Player prefab to spawn. You need to select a Player prefab from its inspector.");
            return;
        }
    }

    /// <summary>
    /// Initialization
    /// </summary>
    public virtual void Start()
    {            
        if (Players == null || Players.Count == 0) { return; }

        Initialization();

        SpawnPlayers();

        //LevelGUIStart();

        // we trigger a level start event
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LevelStart);
        MMGameEvent.Trigger("Load");        
      
        MMCameraEvent.Trigger(MMCameraEventTypes.SetConfiner, null, BoundsCollider);
        MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, TargetPlayer);
        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
    }

    protected virtual void SpawnPlayers()
    {
        int index = 0;
        foreach (MSB_Character character in Players)
        {
            Spawnpoints[index].SpawnPlayer(character);
            index++;
        }
    }

    /// <summary>
    /// Gets current camera, points number, start time, etc.
    /// </summary>
    protected virtual void Initialization()
    {
        Debug.Log("MSB_LevelManager Start");
        LevelCameraController = FindObjectOfType<CameraController>();
        _started = DateTime.UtcNow;

        // if we don't find a bounds collider we generate one
        BoundsCollider = this.gameObject.GetComponent<Collider>();
        if (BoundsCollider == null)
        {
            GenerateColliderBounds();
            BoundsCollider = this.gameObject.GetComponent<Collider>();
        }

        // Hierachy에 있는 MSB_SpawnPoint 오브젝트들을 List에 저장
        // Find Spawnpoints which in level and sort by its index (ascending)
        Spawnpoints = FindObjectsOfType<MSB_SpawnPoint>().OrderBy(o=>o.SpawnerIndex).ToList();
    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// Initializes GUI stuff
    /// </summary>
    /*protected virtual void LevelGUIStart()
    {
        // set the level name in the GUI
        LevelNameEvent.Trigger(SceneManager.GetActiveScene().name);
        // fade in
        if (Players.Count > 0)
        {
            MMFadeOutEvent.Trigger(IntroFadeDuration, FadeCurve, FaderID, false, Players[0].transform.position);
        }
        else
        {
            MMFadeOutEvent.Trigger(IntroFadeDuration, FadeCurve, FaderID, false, Vector3.zero);
        }
    }*/

    /// <summary>
    /// Spawns a playable character into the scene
    /// </summary>
    /*protected virtual void SpawnSingleCharacter()
    {
        // in debug mode we spawn the player on the debug spawn point
#if UNITY_EDITOR
        if (DebugSpawn != null)
        {
            DebugSpawn.SpawnPlayer(Players[0]);
            return;
        }
        else
        {
            RegularSpawnSingleCharacter();
        }
#else
				RegularSpawnSingleCharacter();
#endif
    }

    /// <summary>
    /// Spawns the character at the selected entry point if there's one, or at the selected checkpoint.
    /// </summary>
    protected virtual void RegularSpawnSingleCharacter()
    {
        PointsOfEntryStorage point = GameManager.Instance.GetPointsOfEntry(SceneManager.GetActiveScene().name);
        if ((point != null) && (PointsOfEntry.Length >= (point.PointOfEntryIndex + 1)))
        {
            Players[0].RespawnAt(PointsOfEntry[point.PointOfEntryIndex], point.FacingDirection);
            return;
        }

        if (CurrentCheckPoint != null)
        {
            CurrentCheckPoint.SpawnPlayer(Players[0]);
            return;
        }
    }
    */
    /// <summary>
    /// Spawns multiple playable characters into the scene
    /// </summary>
    /*protected virtual void SpawnMultipleCharacters()
    {
        int checkpointCounter = 0;
        int characterCounter = 1;
        bool spawned = false;
        foreach (Character player in Players)
        {
            spawned = false;

            /*if (AutoAttributePlayerIDs)
            {
                player.SetPlayerID("Player" + characterCounter);
            }

            player.name += " - " + player.PlayerID;

            if (Checkpoints.Count > checkpointCounter + 1)
            {
                if (Checkpoints[checkpointCounter] != null)
                {
                    Checkpoints[checkpointCounter].SpawnPlayer(player);
                    characterCounter++;
                    spawned = true;
                    checkpointCounter++;
                }
            }
            if (!spawned)
            {
                Checkpoints[checkpointCounter].SpawnPlayer(player);
                characterCounter++;
            }
        }
    }*/

    /// <summary>
    /// Instantiates (if needed) the no going back object that will prevent back movement in the level
    /// </summary>
    /*protected virtual void InstantiateNoGoingBack()
    {
        if (OneWayLevelMode == OneWayLevelModes.None)
        {
            return;
        }
        // we instantiate the new no going back object
        GameObject newObject = new GameObject();
        newObject.name = "NoGoingBack";
        newObject.layer = LayerMask.NameToLayer("PlatformsPlayerOnly");
        Vector3 newPosition = Players[0].transform.position;
        if (OneWayLevelMode == OneWayLevelModes.Left) { newPosition.x -= NoGoingBackThreshold + NoGoingBackColliderSize.x / 2f; }
        if (OneWayLevelMode == OneWayLevelModes.Right) { newPosition.x += NoGoingBackThreshold + NoGoingBackColliderSize.x / 2f; }
        if (OneWayLevelMode == OneWayLevelModes.Down) { newPosition.y -= NoGoingBackThreshold + NoGoingBackColliderSize.y / 2f; }
        if (OneWayLevelMode == OneWayLevelModes.Top) { newPosition.y += NoGoingBackThreshold + NoGoingBackColliderSize.y / 2f; }
        newObject.transform.position = newPosition;

        WallClingingOverride wallClingingOverride = newObject.AddComponent<WallClingingOverride>();
        wallClingingOverride.CanWallClingToThis = false;

        BoxCollider2D collider2D = newObject.AddComponent<BoxCollider2D>();
        collider2D.size = NoGoingBackColliderSize;

        NoGoingBackObject = newObject.AddComponent<NoGoingBack>();
        NoGoingBackObject.ThresholdDistance = NoGoingBackThreshold;
        NoGoingBackObject.Target = Players[0].transform;
        NoGoingBackObject.OneWayLevelMode = OneWayLevelMode;
        NoGoingBackObject.NoGoingBackColliderSize = NoGoingBackColliderSize;
        NoGoingBackObject.MinDistanceFromBounds = NoGoingBackMinDistanceFromBounds;

        if (OneWayLevelKillMode == OneWayLevelKillModes.Kill)
        {
            newObject.AddComponent<KillPlayerOnTouch>();
        }
    }

    /// <summary>
    /// Every frame we check for checkpoint reach
    /// </summary>
    public virtual void Update()
    {
        if (Players == null)
        {
            return;
        }

        _savedPoints = GameManager.Instance.Points;
        _started = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the current checkpoint.
    /// </summary>
    /// <param name="newCheckPoint">New check point.</param>
    public virtual void SetCurrentCheckpoint(CheckPoint newCheckPoint)
    {
        CurrentCheckPoint = newCheckPoint;
    }

    public virtual void SetNextLevel(string levelName)
    {
        _nextLevel = levelName;
    }

    public virtual void GotoNextLevel()
    {
        GotoLevel(_nextLevel);
        _nextLevel = null;
    }

    /// <summary>
    /// Gets the player to the specified level
    /// </summary>
    /// <param name="levelName">Level name.</param>
    public virtual void GotoLevel(string levelName)
    {
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LevelEnd);
        MMGameEvent.Trigger("Save");
        if (Players.Count > 0)
        {
            MMFadeInEvent.Trigger(OutroFadeDuration, FadeCurve, FaderID, true, Players[0].transform.position);
        }
        else
        {
            MMFadeInEvent.Trigger(OutroFadeDuration, FadeCurve, FaderID, true, Vector3.zero);
        }
        StartCoroutine(GotoLevelCo(levelName));
    }

    /// <summary>
    /// Waits for a short time and then loads the specified level
    /// </summary>
    /// <returns>The level co.</returns>
    /// <param name="levelName">Level name.</param>
    protected virtual IEnumerator GotoLevelCo(string levelName)
    {
        if (Players != null && Players.Count > 0)
        {
            foreach (Character player in Players)
            {
                player.Disable();
            }
        }

        if (Time.timeScale > 0.0f)
        {
            yield return new WaitForSeconds(OutroFadeDuration);
        }
        else
        {
            yield return new WaitForSecondsRealtime(OutroFadeDuration);
        }
        // we trigger an unPause event for the GameManager (and potentially other classes)
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);

        if (string.IsNullOrEmpty(levelName))
        {
            LoadingSceneManager.LoadScene("StartScreen");
        }
        else
        {
            LoadingSceneManager.LoadScene(levelName);
        }
    }*/

    /// <summary>
    /// Kills the player.
    /// </summary>
    public virtual void KillPlayer(Character player)
    {
        Health characterHealth = player.GetComponent<Health>();
        if (characterHealth == null)
        {
            return;
        }
        else
        {
            // we kill the character
            characterHealth.Kill();
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.PlayerDeath);

            // if we have only one player, we restart the level
            if (Players.Count < 2)
            {
                //StartCoroutine(SoloModeRestart());
            }
        }
    }

    /// <summary>
    /// Coroutine that kills the player, stops the camera, resets the points.
    /// </summary>
    /// <returns>The player co.</returns>
    /*protected virtual IEnumerator SoloModeRestart()
    {
        if (PlayerPrefabs.Count() <= 0)
        {
            yield break;
        }

        // if we've setup our game manager to use lives (meaning our max lives is more than zero)
        if (GameManager.Instance.MaximumLives > 0)
        {
            // we lose a life
            GameManager.Instance.LoseLife();
            // if we're out of lives, we check if we have an exit scene, and move there
            if (GameManager.Instance.CurrentLives <= 0)
            {
                CorgiEngineEvent.Trigger(CorgiEngineEventTypes.GameOver);
                if ((GameManager.Instance.GameOverScene != null) && (GameManager.Instance.GameOverScene != ""))
                {
                    LoadingSceneManager.LoadScene(GameManager.Instance.GameOverScene);
                }
            }
        }

        if (LevelCameraController != null)
        {
            LevelCameraController.FollowsPlayer = false;
        }

        yield return new WaitForSeconds(RespawnDelay);

        if (LevelCameraController != null)
        {
            LevelCameraController.FollowsPlayer = true;
        }

        if (CurrentCheckPoint != null)
        {
            CurrentCheckPoint.SpawnPlayer(Players[0]);
        }
        _started = DateTime.UtcNow;
        // we send a new points event for the GameManager to catch (and other classes that may listen to it too)
        CorgiEnginePointsEvent.Trigger(PointsMethods.Set, 0);
        // we trigger a respawn event
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.Respawn);
    }*/

    /// <summary>
    /// Freezes the character(s)
    /// </summary>
    /*public virtual void FreezeCharacters()
    {
        foreach (Character player in Players)
        {
            player.Freeze();
        }
    }*/

    /// <summary>
    /// Unfreezes the character(s)
    /// </summary>
    /*public virtual void UnFreezeCharacters()
    {
        foreach (Character player in Players)
        {
            player.UnFreeze();
        }
    }*/

    /// <summary>
    /// Toggles Character Pause
    /// </summary>
    /*public virtual void ToggleCharacterPause()
    {
        foreach (Character player in Players)
        {

            CharacterPause characterPause = player.GetComponent<CharacterPause>();
            if (characterPause == null)
            {
                break;
            }

            if (GameManager.Instance.Paused)
            {
                characterPause.PauseCharacter();
            }
            else
            {
                characterPause.UnPauseCharacter();
            }
        }
    }*/

    /// <summary>
    /// A temporary method used to convert level bounds from the old system to actual collider bounds
    /// </summary>
    [ExecuteAlways]
    protected virtual void GenerateColliderBounds()
    {
        // set transform
        this.transform.position = LevelBounds.center;

        // remove existing collider
        if (this.gameObject.GetComponent<BoxCollider>() != null)
        {
            DestroyImmediate(this.gameObject.GetComponent<BoxCollider>());
        }

        // create collider
        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
        // set size
        collider.size = LevelBounds.extents * 2f;

        // set layer
        this.gameObject.layer = LayerMask.NameToLayer("NoCollision");
    }

}