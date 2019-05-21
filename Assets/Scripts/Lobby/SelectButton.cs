//
//  SelectButton
//  Created by 문주한 on 20/05/2019.
//
//  로비 화면에서 투명 버튼의 기능
//  버튼 클릭시 카메라 이동
//  스킨 ID와 무기 ID 값을 받아와 전달함 
//  

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour, IPointerClickHandler
{
    GameObject mainCamera;

    public GameObject scrollView_Character;
    public GameObject scrollView_Weapon;
    public GameObject joinQueue;

    Vector3 nextPos;
    bool nextOn = false;

    public bool characterChoice;

    int skinID;
    int weaponID;

    // Start is called before the first frame update
    void Start()
    {
        characterChoice = false;
        mainCamera = GameObject.Find("Main Camera");
        nextPos = new Vector3(10, 0, -10);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (nextOn)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, nextPos, Time.deltaTime * 2f);
            if (mainCamera.transform.position.x > 9.9f)
            {
                nextOn = false;
                scrollView_Weapon.SetActive(true);
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(characterChoice == false)
        { 
            // 캐릭터 스킨 선택
            nextOn = true;
            scrollView_Character.SetActive(false);
            characterChoice = true;
        }
        else
        {
            // 무기 선택
            sendSkinWeaponID();

            // 큐 선택 팝업 
            //joinQueue.SetActive(true);
        }

    }

    public void sendSkinWeaponID()
    {
        // SkinID and WeaponID 전송
        Debug.Log("Skin ID : " + skinID);
        Debug.Log("Weapon ID : " + weaponID);

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
