using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {

    //190309 깃 테스트 1
    public float stayTime;  //Player가 점프패드에 닿아있는 시간을 담는 변수
    public float maxTime;   //점프를 하기 위해 플레이어가 점프패드에 접촉을 유지해야하는 시간
	// Use this for initialization
	void Start () {
        maxTime = 0.5f;
	}

	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 접촉 중 일때만 실행
        if (collision.gameObject.GetComponent("Player") != null)
        {
            stayTime = 0;
            // Debug.Log("Entered");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어와 접촉 중 일때만 실행
        if (collision.gameObject.tag == "LocalPlayer")
        {
            //접촉을 유지하고 있을 동안 stayTime 변수의 값을 증가시켜 maxTime 까지 증가되면 Player.jump() 를 실행
            if (stayTime > maxTime)
            {
                stayTime = 0;
                // Debug.Log("Player have stayed over 2secs");
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 700);
            }
            else
            {
                stayTime += Time.deltaTime;
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        stayTime = 0;
    }
}
