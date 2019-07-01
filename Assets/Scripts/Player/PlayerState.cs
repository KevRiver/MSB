using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Die,    //죽음
    Idle,   //기본 상태 달리거나, 
    Stun,   //기절
    Slow,   //둔화
    SuperArmor, //상태이상 Stun, Slow 무시 체력은 깎임
    Invincible  //상태이상 면역 및 체력 깎이지 않음
}

public class PlayerState : MonoBehaviour
{
    public State currentState;
    private IEnumerator StateDuration;
    public bool isAttackable;
    public bool isMoveable;
    public bool isGrounded;

    public void ChangeState(State _state, float _duration)
    {
        if (currentState != State.SuperArmor || currentState != State.Invincible)
            return;

        currentState = _state;
        StateDuration = StateDurationInstance(_duration);
        StartCoroutine("StateDuration");
    }

    public IEnumerator StateDurationInstance(float _durations)
    {
        yield return new WaitForSeconds(_durations);
    }

    private void CheckCurrentState()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Idle;
        isAttackable = true;
        isMoveable = true;
        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
