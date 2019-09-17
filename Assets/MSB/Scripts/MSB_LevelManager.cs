using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MSBNetwork;

public class MSB_LevelManager : LevelManager
{
    new private static MSB_LevelManager _instance;
    new public static MSB_LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MSB_LevelManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "LevelManager";
                    _instance = obj.AddComponent<MSB_LevelManager>();
                    //DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }
    
    public List<MSB_Character> MSB_PlayerPrefabs;
    public List<MSB_Character> MSB_Players;
    // Start is called before the first frame update

    new void Awake()
    {
        Debug.Log("MSB_LevelManager Awake");              
    }


    private const int WEAPON_SWORD = 0;
    private const int WEAPON_SHURIKEN = 1;
    private const int WEAPON_RIFLE = 2;
    protected override void InstantiatePlayableCharacters()
    {
        MSB_Players = new List<MSB_Character>();

        // we check if there's a stored character in the game manager we should instantiate
        if (MSB_GameManager.Instance.StoredCharacter != null)
        {
            MSB_Character newPlayer = (MSB_Character)Instantiate(MSB_GameManager.Instance.StoredCharacter, new Vector3(0, 0, 0), Quaternion.identity);
            newPlayer.name = MSB_GameManager.Instance.StoredCharacter.name;
            MSB_Players.Add(newPlayer);
            return;
        }

        if (MSB_PlayerPrefabs == null) { return; }

        // player instantiation       
        if (MSB_PlayerPrefabs.Count != 0)
        {
            foreach (ClientUserData user in MSB_GameManager.Instance.c_userData)
            {
                MSB_Character newPlayer = new MSB_Character();
                switch (user.userWeapon)
                {
                    case WEAPON_SWORD:
                        newPlayer = Instantiate(MSB_PlayerPrefabs[WEAPON_SWORD], new Vector3(0, 0, 0), Quaternion.identity);
                        break;
                    case WEAPON_SHURIKEN:
                        newPlayer = Instantiate(MSB_PlayerPrefabs[WEAPON_SHURIKEN], new Vector3(0, 0, 0), Quaternion.identity);
                        break;
                    case WEAPON_RIFLE:
                        break;
                }
                newPlayer.c_userData = user;               

                if (newPlayer.c_userData.userNumber == LocalUser.Instance.localUserData.userNumber)
                {
                    newPlayer.isLocalUser = true;
                    newPlayer.gameObject.layer = LayerMask.NameToLayer("Player");
                }
                else
                {
                    newPlayer.isLocalUser = false;
                    newPlayer.gameObject.layer = LayerMask.NameToLayer("Enemies");
                }

                MSB_Players.Add(newPlayer);
            }
        }
        int gameRoom = MSB_GameManager.Instance.roomIndex;
        NetworkModule.GetInstance().RequestGameUserActionReady(gameRoom);
    }

    protected override void Initialization()
    {
        base.Initialization();
    }

    protected override void SpawnMultipleCharacters()
    {
        int checkpointCounter = 0;
        //int characterCounter = 1;
        int characterCounter = 0;
        int checkPointNumber = 0;
        bool spawned = false;
        foreach (MSB_Character player in MSB_Players)
        {
            spawned = false;

            player.SetPlayerID("Player" + characterCounter + 1);
            player.playerIndex = characterCounter;
            checkPointNumber = characterCounter;

            player.name = player.c_userData.userNick;
            if (Checkpoints.Count > 1)
            {
                if (Checkpoints[checkPointNumber] != null)
                {
                    Checkpoints[checkPointNumber].SpawnPlayer(player);                    
                    spawned = true;                  
                }
            }
            if (!spawned)
            {
                Checkpoints[checkpointCounter].SpawnPlayer(player);                
            }
            characterCounter++;
        }
    }
   
    public void KillPlayer(MSB_Character player)
    {
        if (!player.isLocalUser)
        {
            player.RecievedSpeed = Vector2.zero;
            player.targetPos = Checkpoints[player.playerIndex].transform.position;
        }

        Health characterHealth = player.GetComponent<Health>();
        if (characterHealth == null)
        {
            return;
        }

        characterHealth.Kill();
    }

    /// <summary>
    /// 플레이어 번호를 받아서 Respawn 합니다
    /// </summary>
    /// <param name="num"></param>
    public void RespawnPlayer(int num)
    {
        foreach (MSB_Character player in MSB_Players)
        {
            if (player.c_userData.userNumber == num)
            {
                player.gameObject.SetActive(true);
                int checkPointNumber = player.playerIndex;
                if (!player.isLocalUser)
                {
                    player.targetPos = Checkpoints[checkPointNumber].transform.position;
                }
                Checkpoints[checkPointNumber].SpawnPlayer(player);
                player.Start();
            }
        }
    }

    public override void Start()
    {
        InstantiatePlayableCharacters();
        Initialization();
        SpawnMultipleCharacters();
        CheckpointAssignment();

        Bush.LazyLoad();
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LevelStart);
        MMGameEvent.Trigger("Load");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
