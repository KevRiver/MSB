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
    public GameObject top_Profile;
    public GameObject top_Profile_Text;
    public GameObject top_Setting;
    public GameObject mid_Right;
    public GameObject bot_ChangeCharacter;

    // ScrollView UI
    Transform t_Sv_Skin;
    public GameObject sv_Skin;
    Transform t_HomeButton;
    public GameObject homeButton;
    Transform t_CenterSlot;
    public GameObject centerSlot;

    // Play UI
    Transform middle;
    Transform t_Single_Button;
    public GameObject single_Button;
    Transform t_Multi_Button;
    public GameObject multi_Button;

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
        top_Profile = GameObject.Find("ProfileButton");
        top_Profile_Text = top_Profile.transform.GetChild(0).GetChild(0).gameObject;
        top_Profile_Text.GetComponent<Text>().text = userNick;
        top_Setting = GameObject.Find("SettingButton");

        
        mid_Right = GameObject.Find("RightButton");

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
        middle = transform.Find("Middle");
        t_Single_Button = middle.Find("SinglePlayButton");
        single_Button = t_Single_Button.gameObject;
        t_Multi_Button = middle.Find("MultiPlayButton");
        multi_Button = t_Multi_Button.gameObject;

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
