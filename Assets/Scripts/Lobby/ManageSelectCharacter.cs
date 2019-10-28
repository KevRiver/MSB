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

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<ManageLobbyObject>();

        if(skinID == canvas.skinID)
        {
            GetComponent<Image>().sprite = colorCharacter;
        }

        Debug.Log(skinID);
    }

    // Update is called once per frame
    void Update()
    {
        if (skinID != canvas.skinID && GetComponent<Image>().sprite != greyCharacter)
        {
            Debug.Log("yq");
            GetComponent<Image>().sprite = greyCharacter;
        }
    }

    public void characterSelectButton()
    {
        GetComponent<Image>().sprite = colorCharacter;
        canvas.skinID = skinID;
        canvas.weaponID = weaponID;
        FindObjectOfType<LobbyCharacter>().changeSprite(skinID);
    }
}
