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

            switch (skinID)
            {
                case 0:
                    canvas.statBar[0].fillAmount = 0.5f;
                    canvas.statBar[1].fillAmount = 0.3f;
                    canvas.statBar[2].fillAmount = 0.8f;
                    canvas.statBar[3].fillAmount = 0.3f;
                    canvas.characterText[0].SetActive(true);
                    canvas.characterText[1].SetActive(false);
                    canvas.characterText[2].SetActive(false);
                    Debug.Log("0");
                    break;
                case 1:
                    canvas.statBar[0].fillAmount = 0.3f;
                    canvas.statBar[1].fillAmount = 0.5f;
                    canvas.statBar[2].fillAmount = 0.6f;
                    canvas.statBar[3].fillAmount = 0.5f;
                    canvas.characterText[0].SetActive(false);
                    canvas.characterText[1].SetActive(true);
                    canvas.characterText[2].SetActive(false);
                    Debug.Log("1");
                    break;
                case 2:
                    canvas.statBar[0].fillAmount = 0.6f;
                    canvas.statBar[1].fillAmount = 0.7f;
                    canvas.statBar[2].fillAmount = 1.0f;
                    canvas.statBar[3].fillAmount = 0.9f;
                    canvas.characterText[0].SetActive(false);
                    canvas.characterText[1].SetActive(false);
                    canvas.characterText[2].SetActive(true);
                    Debug.Log("2");
                    break;
            }
        }
    }

    void selectCharacterAnimation()
    {
        selectAnimator.SetTrigger("Select");
    }
}
