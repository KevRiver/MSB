using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class FloatingMessageState
{
    public enum State
    {
        Idle,
        Start,
        Destroy
    }
}

public enum FloatingMessageStartAnimation
{
    PopSlideUp,
    Pop
}

public enum FloatingMessageDestroyAnimation
{
    PopOut,
    FadeOut
}

public class FloatingMessage : MonoBehaviour
{
    /*private Animator _animator;
    private MMStateMachine<FloatingMessageState.State> _messageState;
    
    public string idleParam;
    public string startParam;
    public string destroyParam;

    public const string Pop = "Pop";
    public const string PopSlideUp = "PopSlideUp";
    public const string PopOut = "PopOut";
    public const string FadeOut = "FadeOut";

    public float Duration;

    public float StartAnimationDuration;
    public float DestroyAnimationDuration;

    private List<int> _animationParams { get; set; }
    private int _idleParam;
    private int _startParam;
    private int _destroyParam;*/

    public float DestroyDelay = 2.0f;

    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, DestroyDelay);
        //Initialization();
    }

    /*protected override void OnEnable()
    {
        base.OnEnable();
        _messageState = new MMStateMachine<FloatingMessageState.State>(gameObject,true);
        StartCoroutine(ProeccessAnimation(Duration));
    }

    private void Initialization()
    {
        //Debug.LogWarning("FloatingMessage Initializing");
        if (GetComponent<Animator>() != null)
            _animator = GetComponent<Animator>();
        
        
        InitializeAnimatorParams();
        
        //Debug.LogWarning("FloatingMessage Finished");
    }

    public void SetAnimation(FloatingMessageStartAnimation startType, FloatingMessageDestroyAnimation destroyType)
    {
        idleParam = "Idle";
        switch (startType)
        {
            case FloatingMessageStartAnimation.Pop:
                startParam = Pop;
                StartAnimationDuration = 1.0f;
                break;
            case FloatingMessageStartAnimation.PopSlideUp:
                startParam = PopSlideUp;
                StartAnimationDuration = 0.25f;
                break;
        }
        switch (destroyType)
        {
            case FloatingMessageDestroyAnimation.PopOut:
                destroyParam = PopOut;
                DestroyAnimationDuration = 0.25f;
                break;
            case FloatingMessageDestroyAnimation.FadeOut:
                destroyParam = FadeOut;
                DestroyAnimationDuration = 1.0f;
                break;
        }
    }

    private IEnumerator ProeccessAnimation(float duration)
    {
        if (duration < 0)
            duration = 1.0f;
        
        Debug.LogWarning("FloatingMessage Process Start");
        _messageState.ChangeState(FloatingMessageState.State.Start);
        UpdateAnimator();
        yield return new WaitForSeconds(StartAnimationDuration);
        
        yield return new WaitForSeconds(duration - StartAnimationDuration);
        
        _messageState.ChangeState(FloatingMessageState.State.Destroy);
        UpdateAnimator();
        yield return new WaitForSeconds(DestroyAnimationDuration);
        
        Debug.LogWarning("FloatingMessage Process End Destory object");
        Destroy();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void InitializeAnimatorParams()
    {
        if (_animator == null)
            return;

        _animationParams = new List<int>();

        MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, idleParam, out _idleParam, AnimatorControllerParameterType.Bool, _animationParams);
        MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, startParam, out _startParam, AnimatorControllerParameterType.Bool, _animationParams);
        MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, destroyParam, out _destroyParam, AnimatorControllerParameterType.Bool, _animationParams);
    }

    private void UpdateAnimator()
    {
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleParam,
            (_messageState.CurrentState == FloatingMessageState.State.Idle), _animationParams);
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _startParam,
            (_messageState.CurrentState == FloatingMessageState.State.Start), _animationParams);
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _destroyParam,
            (_messageState.CurrentState == FloatingMessageState.State.Destroy), _animationParams);
    }*/
}
