//
//  ManageLobbyObject
//  Created by 문주한 on 29/07/2019.
//
//  Manage Lobby`s Object
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageLobbyObject : MonoBehaviour
{
    // Lobby UI
    public GameObject top_Coin;
    public GameObject top_Cash;
    public GameObject top_Profile;
    public GameObject mid_Left;
    public GameObject mid_Right;
    public GameObject bot_Store;
    public GameObject bot_Setting;

    // ScrollView UI
    Transform t_Sv_Weapon;
	public GameObject sv_Weapon;
    Transform t_Sv_Skin;
    public GameObject sv_Skin;
    Transform t_HomeButton;
    public GameObject homeButton;
    Transform t_CenterSlot;
    public GameObject centerSlot;

    // CharacterID
    public int skinID;
    public int weaponID;

    // Start is called before the first frame update
    void Start()
    {
        // Lobby UI
        top_Coin = GameObject.Find("Coin");
        top_Cash = GameObject.Find("Cash");
        top_Profile = GameObject.Find("ProfileButton");
        mid_Left = GameObject.Find("MiddleLeft");
        mid_Right = GameObject.Find("RightButton");
        bot_Setting = GameObject.Find("SettingButton");
        bot_Store = GameObject.Find("StoreButton");

        // Scroll View UI
        t_Sv_Weapon = transform.Find("Scroll View_Weapon");
        t_Sv_Skin = transform.Find("Scroll View_Skin");
        t_CenterSlot = transform.Find("CenterSlot");
        sv_Weapon = t_Sv_Weapon.gameObject;
        sv_Skin = t_Sv_Skin.gameObject;
        centerSlot = t_CenterSlot.gameObject;


        t_HomeButton = transform.Find("HomeButton");
        homeButton = t_HomeButton.gameObject;
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
