#define LEVMANAGER_LOG_ON
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MSBNetwork;

public class MSB_LevelManager : Singleton<MSB_LevelManager>
{
    /// the prefab you want for your player
    [Header("Playable Characters")]
    [Information("The LevelManager is responsible for handling spawn/respawn, checkpoints management and level bounds. Here you can define one or more playable characters for your level..", InformationAttribute.InformationType.Info, false)]
    /// the list of player prefabs to instantiate
    public List<MSB_Character> PlayerPrefabs;

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
    
    public List<MSB_Character> Players { get; protected set; }
    public MSB_Character TargetPlayer;
    public List<MSB_SpawnPoint> Spawnpoints;
    public List<Item> Items;
    public Dictionary<int, MSB_Character> _allPlayersCharacter;
    protected DateTime _started;
    private GameInfo gameInfo;

    /// <summary>
    /// On awake, instantiates the player
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        gameInfo = GameInfo.Instance;
        if (gameInfo == null)
        {
            Debug.LogWarning("GameInfo is null");
            return;
        }

        InstantiatePlayableCharacters(gameInfo.players);
    }
    /// <summary>
    /// Instantiate playable characters based on UserData.userWeapon
    /// </summary>
    protected virtual void InstantiatePlayableCharacters(List<PlayerInfo> users)
    {
        int localUserNum = LocalUser.Instance.localUserData.userNumber;

        Players = new List<MSB_Character>();        
        if (PlayerPrefabs == null) { return; }
      
        // player instantiation
        if (PlayerPrefabs.Count != 0)
        {
            foreach (PlayerInfo user in users)
            {
                MSB_Character newPlayer = (MSB_Character)Instantiate(PlayerPrefabs[user.weapon], new Vector3(0, 0, 0), Quaternion.identity);
                newPlayer.cUserData = new ClientUserData(user.number, user.id, user.nick, user.weapon, user.skin);
                newPlayer.UserNum = user.number;

                //check remote players and add RCReciever component to its gameobject
                if (newPlayer.UserNum != localUserNum)
                {
                    newPlayer.gameObject.AddComponent<RCReciever>();
                    newPlayer.IsRemote = true;
                }
                else
                {
                    newPlayer.SetPlayerID("Player1");
                    TargetPlayer = newPlayer;

                    RCSender rcSender = RCSender.Instance;
                    rcSender.Initialize(newPlayer, gameInfo.room);
                }
                Players.Add(newPlayer);
            }
        }
        else
        {
            //Debug.LogWarning ("LevelManager : The Level Manager doesn't have any Player prefab to spawn. You need to select a Player prefab from its inspector.");
            return;
        }

        NetworkMethod requestReady = NetworkModule.GetInstance().RequestGameUserActionReady;
        StartCoroutine(CoroutineTimer.InvokeAfter(3.0f, requestReady, gameInfo.room));
    }
    
    
    /// <summary>
    /// Initialization
    /// </summary>
    public virtual void Start()
    {            
        if (Players == null || Players.Count == 0) { return; }

        Initialization();

        SpawnPlayers();

        _allPlayersCharacter = new Dictionary<int, MSB_Character>();
        StoreAllPlayersCharacter();
        
        //LevelGUIStart();

        // we trigger a level start event
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LevelStart);
        MMGameEvent.Trigger("Load");        
      
        MMCameraEvent.Trigger(MMCameraEventTypes.SetConfiner, null, BoundsCollider);
        MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, TargetPlayer);
        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
        
        OnGameEvent onGameEvent = new OnGameEvent(Instance);
        NetworkModule.GetInstance().AddOnEventGameEvent(onGameEvent);
    }

    protected virtual void SpawnPlayers()
    {
        const int SOLO = 0;
        const int TEAM = 1;
        int index = 0;
        int mode = gameInfo.mode;
        MSB_GameManager.Team team = MSB_GameManager.Team.Blue;
        foreach (var character in Players)
        {
            if (mode == SOLO && index == 1)
            {
                team = MSB_GameManager.Team.Red;
                index = 3;
            }
            else if (mode == TEAM && index == 3)
                team = MSB_GameManager.Team.Red;
            
            character.team = team;
            character.SpawnerIndex = index;
            index++;
        }

        foreach (var player in Players)
        {
            if (player.team != TargetPlayer.team)
                player.IsEnemy = true;
            Spawnpoints[player.SpawnerIndex].SpawnPlayer(player);
        }
        
    }
    /// <summary>
    /// MSB Custom
    /// key : UserNum Value : Health
    /// OnGameEvent에서 Player의 Health에 접근하는 경우가 많으므로 Dictionary에 모든 플레이어의 Health 객체를 캐싱한다
    /// </summary>
    protected virtual void StoreAllPlayersCharacter()
    {
        foreach (var character in Players)
        {
            _allPlayersCharacter.Add(character.UserNum, character);
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
    }
    
    public void RespawnPlayer(int userNum)
    {
        _allPlayersCharacter.TryGetValue(userNum, out MSB_Character target);
        if (target != null)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                Spawnpoints[target.SpawnerIndex].SpawnPlayer(target);
                target.gameObject.SetActive(true);
                target.GetComponent<MMHealthBar>().Initialization();
            }

            Spawnpoints[target.SpawnerIndex].SpawnPlayer(target);
        }
    }

    private class OnGameEvent : NetworkModule.OnGameEventListener
    {
        private readonly MSB_LevelManager _levelManager;
        public OnGameEvent(MSB_LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        
        private char[] spliter = {','};
        
        private Health _targetHealth;
        private FloatingMessageController _floatingMessageController;
        private FloatingMessageType _floatingMessageType;
        
        private int amount;

        public void OnGameEventDamage(int from, int to, int amount, string option)
        {
            string[] options = option.Split(spliter);
            CausedCCType ccType = CausedCCType.Non;
            float xForce = 0f;
            float yForce = 0f;
            float duration = 0f;
            ccType = (CausedCCType) int.Parse(options[0]);
            xForce = float.Parse(options[1]);
            yForce = float.Parse(options[2]);
            duration = float.Parse(options[3]);

            _levelManager._allPlayersCharacter.TryGetValue(to, out MSB_Character target);
            if (!target)
            {
                return;
            }

            _targetHealth = target.GetComponent<Health>();
            
            // trigger floating message event
            _floatingMessageController = target.GetComponentInChildren<FloatingMessageController>();
            if (!_floatingMessageController)
            {
                return;
            }

            if (_floatingMessageController != null && ccType != CausedCCType.Non)
            {
                if (target._controller != null)
                {
                    switch (ccType)
                    {
                        case CausedCCType.KnockBack:
                            _floatingMessageType = FloatingMessageType.KnockBack;
                            break;
                        
                        case CausedCCType.Stun:
                            _floatingMessageType = FloatingMessageType.Stun;
                            break;
                    }
                    FloatingMessageEvent.Trigger(target.UserNum, _floatingMessageType, 0);
                    
                    // apply Crowd-Cotrol
                    target.AbilityControl(false,duration);
                    target._controller.SetForce(new Vector2(xForce, yForce));
                }
            }
            
            if(_targetHealth!=null)
                _targetHealth.DamageFeedbacks?.PlayFeedbacks();
        }

        private int _previousHealth;
        public void OnGameEventHealth(int num, int health)
        {
            _levelManager._allPlayersCharacter.TryGetValue(num, out MSB_Character target);
            if (!target)
            {
                return;
            }
            _targetHealth = target.GetComponent<Health>();
            if (_targetHealth != null)
            {
                _previousHealth = _targetHealth.CurrentHealth;
                amount = Mathf.Abs(health - _previousHealth);
                _floatingMessageType =
                    (_previousHealth > health) ? FloatingMessageType.Damage : FloatingMessageType.Heal;
                
                FloatingMessageEvent.Trigger(num, _floatingMessageType, amount);
                _targetHealth.ChangeHealth(health);
            }
        }
        
        public void OnGameEventItem(int type, int num, int action)
        {
#if LEVMANAGER_LOG_ON
            Debug.LogFormat("Item num {0}, Type : {1},Action : {2}", num, type, action);
#endif

            var item = _levelManager.Items[num];
            
            if (action == 0)
            {
                item.gameObject.SetActive(true);
            }
        }

        public void OnGameEventKill(int from, int to, string option)
        {
            _levelManager._allPlayersCharacter.TryGetValue(to, out MSB_Character target);
            if (target == null)
            {
                return;
            }
            target.AbilityControl(true);
            var model = target.transform.GetChild(0);
            var outlineRenderer = model.transform.GetChild(1).GetComponentInChildren<SpriteRenderer>();
            Color color;
            if(!target.IsRemote)
                color = Color.yellow;
            else if (target.team == MSB_LevelManager.Instance.TargetPlayer.team)
                color = Color.green;
            else
            {
                color = Color.red;
            }

            outlineRenderer.material.SetColor("_Color", color);
            _targetHealth = target.gameObject.GetComponent<Health>();
            if (_targetHealth != null)
            {
                Debug.LogWarning("Access target health .kill");
                _targetHealth.Kill();
                target.gameObject.SetActive(false);
            }

        }

        public void OnGameEventObject(int num, int health)
        {
            
        }

        const int GAMERESULTVIEW = 0;
        const int ACHIEVEMENTVIEW = 1;
        const int RESPAWNVIEW = 2;
        public void OnGameEventRespawn(int num, int time)
        {
            MSB_GUIManager guiManager = MSB_GUIManager.Instance;
            if (_levelManager.TargetPlayer.UserNum == num)
            {
                if (!guiManager.RespawnViewModel.gameObject.activeInHierarchy)
                {
                    guiManager.ViewActive(RESPAWNVIEW, true);
                    guiManager.RespawnViewModel.Initialization(time-1);
                }
                guiManager.RespawnViewModel.PlayCountAnimation(time-1);
            }

            if(time == 0)
            {
                if (_levelManager.TargetPlayer.UserNum == num)
                {
                    guiManager.ViewActive(RESPAWNVIEW, false);
                    guiManager.CoverActive(false);
                }

                _levelManager.RespawnPlayer(num);
            }
        }
    }

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

    private void OnDisable()
    {
        #if LEVMANAGER_LOG_ON
        Debug.LogWarning("LevelManager disable");
        #endif
        _instance = null;
        NetworkModule.GetInstance().SetOnEventGameEvent(null);
    }
}