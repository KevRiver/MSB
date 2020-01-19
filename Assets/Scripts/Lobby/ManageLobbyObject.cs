//
//  ManageLobbyObject
//  Created by 문주한 on 29/07/2019.
//
//  Manage Lobby`s Object
//

using System.Collections;
using System.Collections.Generic;
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
    public GameObject bot_ChangeCharacter;

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

    // Character
    public GameObject lobbyCharacter;

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

    public GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            string userNick = LocalUser.Instance.localUserData.userNick;

            // Get Rank
            setRank(LocalUser.Instance.localUserData.userRank);

        }
        catch
        {

        };
        

        // NetworkManager
        networkManager = FindObjectOfType<NetworkManager>();

        // Lobby UI
        mainLobby = GameObject.Find("MainLobby");

        top_RankImg = GameObject.Find("RankImg");
        top_SettingButton = GameObject.Find("SettingButton");

        bot_PlayButton = GameObject.Find("PlayButton");

        bot_ChangeCharacter = GameObject.Find("Button_Character");

        homeButton = GameObject.Find("HomeButton");

        // Play UI
        playLobby = GameObject.Find("PlayLobby");
        soloButton = GameObject.Find("SoloPlayButton");
        multiButton = GameObject.Find("MultiPlayButton");

        // Character
        lobbyCharacter = GameObject.Find("LobbyCharacter");

        
        // Queue Loading
        queueLoading = false;
        loadingCharacter = GameObject.Find("QueueLoading");
        loadingCharacter.SetActive(false);

        // Background
        background = GameObject.Find("Background");

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
            Debug.Log("Bronze");
            top_RankImg.GetComponent<Image>().sprite = rankSprite[0];
        }
        else if(rank >= 800 && rank < 1200)
        {
            Debug.Log("Silver");
            top_RankImg.GetComponent<Image>().sprite = rankSprite[1];
        }
        else if(rank >= 1200)
        {
            Debug.Log("Gold");
            top_RankImg.GetComponent<Image>().sprite = rankSprite[2];
        }
    }

    public void loadScene(int _room)
    {
        networkManager.loadScene(_room);
    }

    public void endPlayLobbyOutTransition()
    {
        lobbyCharacter.GetComponent<SpriteRenderer>().enabled = true;
    }
}
