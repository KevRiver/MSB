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
using UnityEngine.SceneManagement;

public class SelectButton : MonoBehaviour, IPointerClickHandler
{
    // 카메라 관련
    GameObject mainCamera;
    float cameraSize;

    public GameObject scrollView_Character;
    public GameObject scrollView_Weapon;
    public GameObject joinQueue;

    Vector3 nextPos;
    bool nextOn = false;

    public bool characterChoice;

    int skinID;
    int weaponID;

    // 배경 캐릭터
    GameObject character;



    // Start is called before the first frame update
    void Start()
    {
        characterChoice = false;
        mainCamera = GameObject.Find("Main Camera");
        nextPos = new Vector3(10, 0, -10);

        character = GameObject.Find("LobbyPlayer");

        cameraSize = mainCamera.GetComponent<Camera>().orthographicSize;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (nextOn)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, nextPos, Time.deltaTime * 2f);
<<<<<<< HEAD
            mainCamera.GetComponent<Camera>().orthographicSize = cameraSize;

            if(mainCamera.GetComponent<Camera>().orthographicSize < 8)
            {
                cameraSize += 0.1f;
            }

            if (mainCamera.transform.position.x > 9.9f)
            {
                nextOn = false;
                scrollView_Weapon.SetActive(true);
                Debug.Log(mainCamera.GetComponent<Camera>().orthographicSize);
            }
        }
    }

=======
            if (mainCamera.transform.position.x > 9.9f)
            {
                nextOn = false;
                scrollView_Weapon.SetActive(true);
            }
        }
    }

>>>>>>> ft_Gone
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(characterChoice == false)
        { 
            // 캐릭터 스킨 선택
            nextOn = true;
            scrollView_Character.SetActive(false);
            characterChoice = true;
<<<<<<< HEAD

            // 캐릭터 이동 시작
            character.GetComponent<LobbyPlayer>().start = true;
        }
        else
=======
        }
        else
>>>>>>> ft_Gone
        {
            // 무기 선택
            sendSkinWeaponID();

            // 큐 선택 팝업 
            //joinQueue.SetActive(true);
<<<<<<< HEAD

            // 클라이언트 씬 전환 
            SceneManager.LoadScene("ClientTest");
=======
>>>>>>> ft_Gone
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
