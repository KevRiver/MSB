using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Animator animator;
    //이동, 점프, 상태에 따른 애니메이션
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(gameObject.GetComponent<PlayerMovement>().currentMoveSpeed));
        animator.SetBool("Grounded", gameObject.GetComponent<PlayerState>().isGrounded);
    }
}
