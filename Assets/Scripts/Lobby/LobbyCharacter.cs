//
//  LobbyCharacter
//  Created by 문주한 on 20/05/2019.
//
//  로비에서 뒷 배경의 플레이어가 움직이는 기능
//  플레이어 애니메이션 관리
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacter : MonoBehaviour
{
    Animator animator;
    public Sprite[] character = new Sprite[3];

    public GameObject[] shuriken = new GameObject[2];

    float firstPosY;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        shuriken[0] = GameObject.Find("Shuriken01");
        shuriken[1] = GameObject.Find("Shuriken02");

        for(int x = 0; x < 2; x++)
        {
            shuriken[x].SetActive(false);
        }

        firstPosY = transform.position.y;
    }
    float speed = 1;
    // Update is called once per frame
    void Update()
    {

        
    }

    private void FixedUpdate()
    {
        if (shuriken[0].active)
        {
            shuriken[0].transform.position = new Vector3(speed * -26, 3f + speed * -6, -0.5f);
            shuriken[0].transform.Rotate(0, 0, 20);

            shuriken[1].transform.position = new Vector3(speed * -26, 3f + speed * 2, -0.5f);
            shuriken[1].transform.Rotate(0, 0, 20);
        }
        else
        {
            speed = 0;
        }
        speed += 0.01f;
    }


    public void changeSprite(int id){
        Debug.Log(firstPosY);
        GetComponent<SpriteRenderer>().sprite = character[id];
        switch (id)
        {
            case 0:
                transform.position = new Vector3(transform.position.x, firstPosY, -1);
                Debug.Log(transform.position.y);
                animator.SetTrigger("CharacterID_01");
                break;
            case 1:
                transform.position = new Vector3(transform.position.x, firstPosY - 0.7f, -1);
                Debug.Log(transform.position.y);
                animator.SetTrigger("CharacterID_02");
                
                break;
            case 2:
                transform.position = new Vector3(transform.position.x, firstPosY - 0.7f, -1);
                Debug.Log(transform.position.y);
                animator.SetTrigger("CharacterID_03");

                break;
        }
    }

    void orangAttack()
    {
        if (GetComponent<SpriteRenderer>().enabled)
        {
            for (int x = 0; x < 2; x++)
            {
                shuriken[x].SetActive(true);
            }
        }
    }

    public void orangStop()
    {
        for (int x = 0; x < 2; x++)
        {
            shuriken[x].SetActive(false);
            shuriken[x].transform.position = new Vector3(0, 2f , -0.5f);
            shuriken[x].transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}
