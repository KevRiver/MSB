//
//  ManageSelectCharacter
//  Created by 문주한 on 23/10/2019.
//
//  Manage character in select character window
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageSelectCharacter : MonoBehaviour
{
    public int skinID;
    public int weaponID;

    ManageLobbyObject canvas;

    public Sprite colorCharacter;
    public Sprite greyCharacter;

    public bool selected;

    Animator selectAnimator;

    // Lobby Character
    LobbyCharacter lobbyCharacter;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<ManageLobbyObject>();

        lobbyCharacter = FindObjectOfType<LobbyCharacter>();

        selected = false;

        selectAnimator = GetComponent<Animator>();

        if (skinID == canvas.skinID)
        {
            selected = true;
        }
        else
        {
            selectAnimator.SetTrigger("UnSelected");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (skinID != canvas.skinID && selected)
        {
            selected = false;
            selectAnimator.SetTrigger("UnSelected");
        }
    }

    public void characterSelectButton()
    {
        lobbyCharacter.orangStop();
        if (!selected)
        {
            selected = true;
            selectCharacterAnimation();

            canvas.skinID = skinID;
            canvas.weaponID = weaponID;
            FindObjectOfType<LobbyCharacter>().changeSprite(skinID);
        }
    }

    void selectCharacterAnimation()
    {
        selectAnimator.SetTrigger("Select");
    }
}
