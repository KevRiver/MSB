using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform tr;

    private float moveSpeed;    //고정 이동속도
    public float MoveSpeed
    {
        set { moveSpeed = value; }
        get { return moveSpeed; }
    }

    private float jumpForce;    //고정 점프력
    public float JumpForce
    {
        set { jumpForce = value; }
        get { return jumpForce; }
    }

    private float moveSpeedMultiplier;
    private float jumpForceMultiplier;

    public float currentMoveSpeed;
    public float currentJumpForce;
    public float maxMoveSpeed;
    public float realSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        tr = gameObject.transform;

        //moveSpeedMultiplier = gameObject.GetComponentInChildren<WeaponBase>().wepaonInfo.moveSpeedMultiplier;
        //jumpForceMultiplier = gameObject.GetComponentInChildren<WeaponBase>().wepaonInfo.jumpForceMultiplier;

        MoveSpeed = 25.0f;  //초기화
        JumpForce = 300.0f;

        //MoveSpeed *= moveSpeedMultiplier;   //무기의 이동속도 계수와 점프력 계수 계산
        //JumpForce *= jumpForceMultiplier;
        currentJumpForce = JumpForce;
        maxMoveSpeed = 10.0f;
        
        //gameObject.GetComponent<PlayerNetwork>
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentMoveSpeed = rb.velocity.x;
    }

    public void Move(Vector3 moveVector)
    {
        //bool spriteFlip = (gameObject.GetComponent<SpriteRenderer>().flipX ? (moveVector.x > 0f) : (moveVector.x < 0f));
        

        if (gameObject.GetComponent<PlayerState>().isMoveable == false)
        {
            Debug.Log("return");
            return;
        }

        if (moveVector.x > 0f)
        {
           if (rb.velocity.x > maxMoveSpeed)
                return;

            rb.AddForce(Vector3.right * MoveSpeed);    //AddForce는 Time.deltaTime을 곱해줄 필요가 없다
            tr.localScale = new Vector3(0.5f, 0.5f, 0f);
        }
        else if (moveVector.x < 0f)
        {
            if (rb.velocity.x < -maxMoveSpeed)
                return;

            rb.AddForce(Vector3.left * MoveSpeed);
            tr.localScale = new Vector3(-0.5f, 0.5f, 0f);
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                if (rb.velocity.x > maxMoveSpeed)
                    return;

                rb.AddForce(Vector3.right * moveSpeed);
                tr.localScale = new Vector3(0.5f, 0.5f, 0f);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                //오른쪽으로 이동
                if (rb.velocity.x < -maxMoveSpeed)
                    return;

                rb.AddForce(Vector3.left * moveSpeed);
                tr.localScale = new Vector3(-0.5f, 0.5f, 0f);
            }
        }
    }
    public void Jump()
    {
        //Debug.Log("Jump!");
        //animator.SetBool("Grounded", false);
        if (gameObject.GetComponent<PlayerState>().isMoveable == false)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && gameObject.GetComponent<PlayerState>().isGrounded)
        {
            rb.AddForce(Vector3.up * currentJumpForce);
        }
    }
}
