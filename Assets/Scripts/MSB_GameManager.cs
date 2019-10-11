using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

public class MSB_GameManager : GameManager
{
    new private static MSB_GameManager _instance;
    new public static MSB_GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MSB_GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GameManager";
                    _instance = obj.AddComponent<MSB_GameManager>();
                }
            }
            return _instance;
        }
    }

    public List<ClientUserData> c_userData;   
    public int roomIndex;
    /*
    public bool isGameStart = false;
    public bool isGameOver = false;*/

    public int playerIndex;
    public int kill;
    public int death;
    public int score;

    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(gameObject);        
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        MSB_LevelManager levelManager = MSB_LevelManager.Instance;
        InputManager inputManager = InputManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Reset()
    {
        base.Reset();
    }
}
