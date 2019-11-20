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
    public List<MSB_SpawnPoint> Spawnpoints { get; protected set; }
    public List<Item> Items { get; protected set; }
    public Dictionary<int, MSB_Character> _allPlayersCharacter;
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
        //Debug.Log("local user number : " + localUserNum);

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
        NetworkModule.GetInstance().RequestGameUserActionReady(gameInfo.room);
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
        
        NetworkModule.GetInstance().AddOnEventGameEvent(new OnGameEvent(this));
    }

    protected virtual void SpawnPlayers()
    {
        int index = 0;
        int mid = Spawnpoints.Count - 1;
        MSB_GameManager.Team team = MSB_GameManager.Team.Blue;
        foreach (var character in Players)
        {
            character.team = team;
            character.SpawnerIndex = index;
            Spawnpoints[index].SpawnPlayer(character);
            team = (index < mid) ? MSB_GameManager.Team.Blue : MSB_GameManager.Team.Red;
            index++;
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
        Spawnpoints = FindObjectsOfType<MSB_SpawnPoint>().OrderBy(o=>o.SpawnerIndex).ToList();
        Items = FindObjectsOfType<Item>().OrderBy(o => o.ItemIndex).ToList();
        Debug.LogWarning(Items);
    }

    public void RespawnPlayer(int userNum)
    {
        _allPlayersCharacter.TryGetValue(userNum, out MSB_Character target);
        if (target != null)
        {
            if(!target.gameObject.activeInHierarchy)
                target.gameObject.SetActive(true);
            Spawnpoints[target.SpawnerIndex].SpawnPlayer(target);
        }
    }

    private class OnGameEvent : NetworkModule.OnGameEventListener
    {
        private MSB_LevelManager _levelManager;
        public OnGameEvent(MSB_LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        
        private char[] spliter = {','};
        
        private Health _targetHealth;
        private FloatingMessageController _floatingMessageController;
        private FloatingMessageType _floatingMessageType;
        private string message = "";
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
            _targetHealth = target.GetComponent<Health>();
            _floatingMessageController = target.GetComponentInChildren<FloatingMessageController>();
            if (_floatingMessageController != null && ccType != CausedCCType.Non)
            {
                if (target._controller != null)
                {
                    switch (ccType)
                    {
                        case CausedCCType.KnockBack:
                            message = "KNOCK BACK";
                            break;
                        
                        case CausedCCType.Stun:
                            message = "STUN";
                            break;
                        
                        case CausedCCType.Root:
                            message = "ROOTED";
                            break;
                    }
                    _floatingMessageType = FloatingMessageType.Text0;
                    FloatingMessageEvent.Trigger(target.UserNum, _floatingMessageType, message, duration);
                    target._controller.SetForce(new Vector2(xForce, yForce));
                }
            }
            if(_targetHealth!=null)
                _targetHealth.DamageFeedbacks?.PlayFeedbacks();
        }

        private int previousHealth;
        public void OnGameEventHealth(int num, int health)
        {
            _levelManager._allPlayersCharacter.TryGetValue(num, out MSB_Character target);
            _targetHealth = target.GetComponent<Health>();
            if (_targetHealth != null)
            {
                previousHealth = _targetHealth.CurrentHealth;
                message = (health - previousHealth).ToString();
                _floatingMessageType =
                    (previousHealth > health) ? FloatingMessageType.Damage : FloatingMessageType.Heal;
                
                FloatingMessageEvent.Trigger(num, _floatingMessageType, message, 0f);
                _targetHealth.ChangeHealth(health);
            }
        }
        
        public void OnGameEventItem(int type, int num, int action)
        {
            Item item;
            Debug.LogWarning("Item event called");
            Debug.Log("OnEventItem : " + type +", " + num + "," + action);
            
            item = _levelManager.Items[num];
            if (action == 0)
            {
                item.gameObject.SetActive(true);
            }
        }

        public void OnGameEventKill(int from, int to, string option)
        {
            Debug.LogWarning(to + " slained by" + from);
            _levelManager._allPlayersCharacter.TryGetValue(to, out MSB_Character target);
            if (target == null)
            {
                Debug.LogError("Killed Character doesnt exist");
                return;
            }

            _targetHealth = target.gameObject.GetComponent<Health>();
            if (_targetHealth != null)
            {
                Debug.LogWarning("Access target health .kill");
                _targetHealth.Kill();
            }

        }

        public void OnGameEventObject(int num, int health)
        {
            
        }

        public void OnGameEventRespawn(int num, int time)
        {
            if(time == 0)
                _levelManager.RespawnPlayer(num);
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

}