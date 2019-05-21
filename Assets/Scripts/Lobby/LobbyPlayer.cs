//
//  LobbyPlayer
//  Created by 문주한 on 20/05/2019.
//
//  로비에서 뒷 배경의 플레이어가 움직이는 기능
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator animator;

    public bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {

        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        SetAnimatorParam(Mathf.Abs(rb.velocity.x), isGrounded);
    }

    public void SetAnimatorParam(float _speed, bool _isGrounded)
    {
        this.animator.SetFloat("Speed", _speed);
        this.animator.SetBool("Grounded", _isGrounded);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

}
