//
//  ManageLobbyObject
//  Created by 문주한 on 29/07/2019.
//
//  Manage Lobby`s Object
//

using System.Collections;
using System.Collections.Generic;
using MSBNetwork;
using UnityEngine;
using UnityEngine.UI;

public class ManageLobbyObject : MonoBehaviour
{
    //Network Manager
    public NetworkManager networkManager;

    // Lobby UI
    public GameObject top_RankImg;
    public Text top_RankText;
    public GameObject top_SettingButton;
    public GameObject bot_PlayButton;
    public GameObject mid_LeaderBoard;
    public GameObject mid_StatBoard;



    // ScrollView UI
    Transform t_ChangeCharacterView;
    public GameObject changeCharacterView;
    Transform t_HomeButton;
    public GameObject homeButton;

    // Play UI
    public GameObject soloButton;
    public GameObject multiButton;

    // Lobby
    public GameObject mainLobby;
    public GameObject playLobby;
    public GameObject settingLobby;

    // CharacterSelect
    public GameObject characterSelectButton;
    public GameObject lobbyCharacter;
    public GameObject statsPanel;
    public GameObject explainPanel;

    // CharacterID
    public int skinID;
    public int weaponID;

    // Rank
    public Sprite[] rankSprite = new Sprite[3];

    // Queue Loading
    public bool queueLoading;
    public Sprite backButtonSprite;
    public Sprite homeButtonSprite;
    public GameObject loadingCharacter;

    public GameObject background;
    public GameObject planet;

    public GameObject mainCamera;

    // LeaderBoard Lobby
    public RankManager leaderBoardLobby;
    
    // Medal Lobby
    public MedalManager medalLobby;

    // Stat Bar
    public Image[] statBar = new Image[4];

    // Explain Text
    public GameObject[] characterText = new GameObject[3];

    private class UserStatusListener : MSBNetwork.NetworkModule.OnStatusResultListener
    {
        private ManageLobbyObject instance;
        public UserStatusListener(ManageLobbyObject _instance)
        {
            instance = _instance;
        }
        public void OnStatusResult(bool _result, UserData _user, int _game, string _message)
        {
            if (instance == null) return;
            instance.OnStatusRenewed(_user);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UserStatusListener userStatusListener = new UserStatusListener(this);
        MSBNetwork.NetworkModule.GetInstance().AddOnEventUserStatus(userStatusListener);
        NetworkModule.GetInstance().RequestUserStatus(LocalUser.Instance.localUserData.userID);

        leaderBoardLobby = GameObject.Find("LeaderBoardLobby").GetComponent<RankManager>();
        leaderBoardLobby.RequestRank();

        medalLobby = GameObject.Find("MedalLobby").GetComponent<MedalManager>();
        medalLobby.RequestMedal();

        //string userNick = LocalUser.Instance.localUserData.userNick;

        // NetworkManager
        networkManager = FindObjectOfType<NetworkManager>();

        // Lobby UI
        mainLobby = GameObject.Find("MainLobby");

        top_RankImg = GameObject.Find("RankImg");
        top_SettingButton = GameObject.Find("SettingButton");

        bot_PlayButton = GameObject.Find("PlayButton");

        mid_LeaderBoard = GameObject.Find("RankArea");
        mid_StatBoard = GameObject.Find("StatArea");


        homeButton = GameObject.Find("HomeButton");

        // Play UI
        playLobby = GameObject.Find("PlayLobby");
        soloButton = GameObject.Find("SoloPlayButton");
        multiButton = GameObject.Find("MultiPlayButton");

        // CharacterSelect
        characterSelectButton = GameObject.Find("CharacterSelectButton");
        lobbyCharacter = GameObject.Find("LobbyCharacter");
        statsPanel = GameObject.Find("StatsPanel");
        explainPanel = GameObject.Find("ExplainPanel");


        statsPanel.SetActive(false);
        explainPanel.SetActive(false);


        // Get Rank
        //setRank(LocalUser.Instance.localUserData.userRank);

        // Queue Loading
        queueLoading = false;
        loadingCharacter = GameObject.Find("QueueLoading");
        loadingCharacter.SetActive(false);

        // Background
        background = GameObject.Find("Background");
        planet = GameObject.Find("Planet");

        // Main Camera
        mainCamera = GameObject.Find("Main Camera");

        

        Debug.Log(Screen.width); // 3040
        Debug.Log(Screen.height); // 1440

        if(Screen.width > 2200)
        {
            Debug.Log(background);
            float num = Screen.width / ((2200f * Screen.height) / 1080f);
            Debug.Log(num);
            background.transform.localScale = new Vector3(num, num, 1);
        }
    }

    public void OnStatusRenewed(UserData user)
    {
        setRank(user.userRank);
    }

    public void getSkinID(int id)
    {
        skinID = id;
    }

    public void getWeaponID(int id)
    {
        weaponID = id;
    }

    void setRank(int rank)
    {
        top_RankText.text = rank.ToString();
        if (rank < 800)
        {
            //Debug.Log("Bronze");
            top_RankImg.GetComponent<Image>().sprite = rankSprite[0];
        }
        else if(rank >= 800 && rank < 1200)
        {
            //Debug.Log("Silver");
            top_RankImg.GetComponent<Image>().sprite = rankSprite[1];
        }
        else if(rank >= 1200)
        {
            //Debug.Log("Gold");
            top_RankImg.GetComponent<Image>().sprite = rankSprite[2];
        }
    }

    public void loadScene(int _room)
    {
        networkManager.loadScene(_room);
    }

    public void showMainCharacter()
    {
        lobbyCharacter.GetComponent<SpriteRenderer>().enabled = true;
    }

    void playButtonOn()
    {
        bot_PlayButton.SetActive(true);
    }

    void playButtonOff()
    {
        bot_PlayButton.SetActive(false);
    }
}
