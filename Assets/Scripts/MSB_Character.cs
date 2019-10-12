using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MSBNetwork;
using UnityEngine.UI;

public class MSB_Character : Character
{
    [Header("MSB Custom")]
    public ClientUserData c_userData;
    public int bushID = 0;
    public int UserNum { get; internal set; }
    private InputManager inputManager;

    protected override void Initialization()
    {
        MovementState = new MMStateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        ConditionState = new MMStateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);
        inputManager = InputManager.Instance;

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
        SetInputManager(inputManager);
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

        AssignAnimator();

        //_originalGravity = _controller.Parameters.Gravity;

        ForceSpawnDirection();
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
        Color col = _spriteRenderer.color;
        col.a = 1.0f;
        _spriteRenderer.color = col;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void OnDestroy()
    {

    }

    
}
