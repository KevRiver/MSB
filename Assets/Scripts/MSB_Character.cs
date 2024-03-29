﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using MSBNetwork;
using UnityEngine.Serialization;

public class MSB_Character : Character,MMEventListener<MMGameEvent>
{
    [FormerlySerializedAs("c_userData")] [Header("MSB Custom")]
    public ClientUserData cUserData;

    public int SpawnerIndex;
    public int bushID = 0;

    public int RoomNum { get; set; }
    public int UserNum { get; internal set; }
    public bool IsRemote { get; set; }
    public bool IsEnemy = false;

    public MSB_GameManager.Team team;

    private InputManager _inputManager;

    protected override void Initialization()
    {
        MovementState = new MMStateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        ConditionState = new MMStateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);
        _inputManager = InputManager.Instance;

        if (InitialFacingDirection == FacingDirections.Left)
        {
            IsFacingRight = false;
        }
        else
        {
            IsFacingRight = true;
        }

        // instantiate camera target
        if (CameraTarget == null)
        {
            CameraTarget = new GameObject();
        }
        CameraTarget.transform.SetParent(this.transform);
        CameraTarget.transform.localPosition = Vector3.zero;
        CameraTarget.name = "CameraTarget";
        _cameraTargetInitialPosition = CameraTarget.transform.localPosition;

        // we get the current input manager
        SetInputManager(_inputManager);
        // we get the main camera
        if (Camera.main != null)
        {
            SceneCamera = Camera.main.GetComponent<CameraController>();
        }
        // we store our components for further use 
        CharacterState = new CharacterStates();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<CorgiController>();
        _characterAbilities = GetComponents<CharacterAbility>();
        _health = GetComponent<Health>();
        _damageOnTouch = GetComponent<DamageOnTouch>();
        CanFlip = true;
        IsRemote = false;

        AssignAnimator();

        //_originalGravity = _controller.Parameters.Gravity;

        ForceSpawnDirection();
    }

    public override void Face(FacingDirections facingDirection)
    {
        if (!CanFlip)
        {
            return;
        }

        // Flips the character horizontally
        if (facingDirection == FacingDirections.Right)
        {
            if (!IsFacingRight)
            {
                Flip();
            }
        }
        else
        {
            if (IsFacingRight)
            {
                Flip();
            }
        }
    }

    public override void AssignAnimator()
    {
        if (CharacterAnimator == null)
            return;

        base.AssignAnimator();
    } 

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening();
        
        //Color col = _spriteRenderer.color;
        //col.a = 1.0f;
        //_spriteRenderer.color = col;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening();
    }

    private void ControllerReset()
    {
        _controller.SetForce(Vector2.zero);
    }
    public virtual void AbilityControl(bool active, float duration = 0)
    {
        foreach (var ability in _characterAbilities)
        {
            ability.AbilityPermitted = active;
        }

        if (!active && duration > 0)
            StartCoroutine(AbilityTempDeny(duration));
    }

    public IEnumerator AbilityTempDeny(float duration)
    {
        yield return new WaitForSeconds(duration);
        foreach (var ability in _characterAbilities)
        {
            ability.AbilityPermitted = true;
        }
    }
    
    

    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                AbilityControl(true);
                break;
            
            case "GameOver":
                ControllerReset();
                AbilityControl(false);
                break;
        }
    }
}
