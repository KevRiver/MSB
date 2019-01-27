using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public float dir;
    public float moveSpeed;
	
	void Start () {
        dir = 1; // 방향 초기화
        moveSpeed = 10.0f; //이동속도
	}
	
	// Update is called once per frame
	public float move(Rigidbody2D rb, Transform tr) {
        //업데이트 할 때마다 키 입력을 받아서 움직인다.
        if (Input.GetKey(KeyCode.RightArrow)) //오른쪽 화살표를 누르고 있는 동안 dir = 1 이 되고 오른쪽으로 addforce
        {
            dir = 1;
            tr.localScale = new Vector3(2, 2, 2);
            rb.AddForce(new Vector2(dir * moveSpeed,0));
        }
        else if (Input.GetKey(KeyCode.LeftArrow))//왼쪽 화살표를 누르고 있는 동안 dir = -1 이 되고 오른쪽으로 addforce
        {
            dir = -1;
            tr.localScale = new Vector3(-2,2,2);
            rb.AddForce(new Vector2(dir * moveSpeed, 0));
        }

        return dir;
	}
}
