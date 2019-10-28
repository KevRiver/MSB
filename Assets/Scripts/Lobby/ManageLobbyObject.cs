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
    // Lobby UI
    public GameObject top_ProfileButton;
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

    // Start is called before the first frame update
    void Start()
    {
        string userNick = LocalUser.Instance.localUserData.userNick;

        // Lobby UI
        top_ProfileButton = GameObject.Find("ProfileButton");
        // top_Profile_Text = top_Profile.transform.GetChild(0).GetChild(0).gameObject;
        // top_Profile_Text.GetComponent<Text>().text = userNick;
        top_SettingButton = GameObject.Find("SettingButton");



        mainLobby = GameObject.Find("MainLobby");
        playLobby = GameObject.Find("PlayLobby");



        bot_PlayButton = GameObject.Find("PlayButton");

        bot_ChangeCharacter = GameObject.Find("Button_Character");
        /*
        // Scroll View UI
        t_Sv_Skin = transform.Find("Scroll View_Skin");
        t_CenterSlot = transform.Find("CenterSlot");
        sv_Skin = t_Sv_Skin.gameObject;
        centerSlot = t_CenterSlot.gameObject;
        */

        t_HomeButton = transform.Find("HomeButton");
        homeButton = t_HomeButton.gameObject;

        // Play UI
        soloButton = GameObject.Find("SoloPlayButton");
        multiButton = GameObject.Find("MultiPlayButton");

        // Character
        lobbyCharacter = GameObject.Find("LobbyCharacter");
    }


    public void getSkinID(int id)
    {
        skinID = id;
    }

    public void getWeaponID(int id)
    {
        weaponID = id;
    }
}
