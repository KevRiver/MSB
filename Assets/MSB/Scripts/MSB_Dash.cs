using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MSBNetwork;

public class MSB_Dash : CharacterAbility
{
    /// 캐릭터가 바라보는 방향으로 일정거리를 돌진하며 충돌하는 적에게 피해를 입히고 기절상태를 만든다
    
    /// This method is only used to display a helpbox text at the beginning of the ability's inspector
    public override string HelpBoxText() { return "캐릭터가 바라보는 방향으로 일정거리를 돌진하며 충돌하는 적에게 피해를 입히고 기절상태를 만든다"; }

    [Header("Dash")]
    /// the duration of dash (in seconds)
    public float DashDistance = 3f;
    /// the force of the dash
    public float DashForce = 40f;
    /// the duration of the cooldown between 2 dashes (in seconds)
    public float DashCooldown = 1f;

    protected float _cooldownTimeStamp = 0;

    protected float _startTime;
    protected Vector3 _initialPosition;
    protected float _dashDirection;
    protected float _distanceTraveled = 0f;
    protected bool _shouldKeepDashing = true;
    protected float _computedDashForce;
    protected float _slopeAngleSave = 0f;
    protected bool _dashEndedNaturally = true;
    protected IEnumerator _dashCoroutine;


    //Dash 충돌체    
    /// the size of the damage area
    public Vector2 AreaSize = new Vector2(1, 1);
    /// the offset to apply to the damage area (from the weapon's attachment position
    public Vector2 AreaOffset = new Vector2(1, 0);

    [Header("Damage Area Timing")]
    /// the initial delay to apply before triggering the damage area
    public float InitialDelay = 0f;
    /// the duration during which the damage area is active
    public float ActiveDuration = 1f;

    [Header("Damage Caused")]
    // the layers that will be damaged by this object
    //public LayerMask TargetLayerMask;

    //MSB Custom
    public LayerMask EnemyLayerMask;
    public LayerMask PlayerLayerMask;
    /// The amount of health to remove from the player's health
    public int DamageCaused = 10;
    /// the kind of knockback to apply
    public DamageOnTouch.KnockbackStyles Knockback;
    /// The force to apply to the object that gets damaged
    public Vector2 KnockbackForce = new Vector2(10, 2);
    /// The duration of the invincibility frames after the hit (in seconds)
    public float InvincibilityDuration = 0.5f;

    protected Collider2D _damageAreaCollider;
    protected bool _attackInProgress = false;    

    protected CircleCollider2D _circleCollider2D;    
    protected DamageOnTouch _damageOnTouch;
    protected GameObject _damageArea;

    //충돌영역 Gizmo
    protected Color _gizmosColor;
    protected Vector3 _gizmoSize;
    protected Vector3 _gizmoOffset;

    public Transform WeaponAttachment { get; set; }

    [Header("MSB Custom")]
    public bool collideOnce;
    public Vector2 _dashVector;
    public float vectorY;

    protected override void Initialization()
    {
        base.Initialization();

        WeaponAttachment = transform.GetChild(0);
        if (_damageArea == null)
        {
            CreateDashCollisionArea();
            DisableDashCollisionArea();
        }
        _damageOnTouch.Owner = gameObject;
    }

    /// <summary>
    /// At the start of each cycle, we check if we're pressing the dash button. If we
    /// </summary>
    protected override void HandleInput()
    {
        if (_inputManager.ActiveSkillButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            StartDash();
        }
    }

    protected override void MSBRemoteControl(ActionType _actionType, bool _actionAimable, float _actionDirX, float _actionDirY)
    {
        base.MSBRemoteControl(_actionType, _actionAimable,_actionDirX, _actionDirY);
        if (_actionType == ActionType.ActiveSkill)
        {
            InitiateDash();
            ResetRecievedData();
        }
    }

    protected override void ResetRecievedData()
    {
        base.ResetRecievedData();
        _MSB_character.RecievedActionType = ActionType.NULL;
        _MSB_character.RecievedActionState = "";
    }

    /// <summary>
    /// The second of the 3 passes you can have in your ability. Think of it as Update()
    /// </summary>
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        // If the character is dashing, we cancel the gravity
        if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
        {
            _controller.GravityActive(true);
        }
        // we reset our slope tolerance if dash didn't end naturally
        if ((!_dashEndedNaturally) && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
        {
            _dashEndedNaturally = true;
            _controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
        }
    }

    /// <summary>
    /// Causes the character to dash or dive (depending on the vertical movement at the start of the dash)
    /// </summary>
    public virtual void StartDash()
    {
        // if the Dash action is enabled in the permissions, we continue, if not we do nothing
        if (!AbilityPermitted
            || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
            || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            return;

        // If the user presses the dash button and is not aiming down
        if (_verticalInput > -_inputManager.Threshold.y)
        {
            // if the character is allowed to dash
            if (_cooldownTimeStamp <= Time.time)
            {
                RequestActionSync();
                InitiateDash();
            }
        }
    }

    bool isAimable = false;
    float dirX = 0f;
    float dirY = 0f;
    public void RequestActionSync()
    {
        string data =
                _MSB_character.c_userData.userNumber +
                "," + ((int)ActionType.ActiveSkill).ToString() +
                "," + isAimable.ToString() +
                "," + dirX.ToString() +
                "," + dirX.ToString();
        NetworkModule.GetInstance().RequestGameUserSync(MSB_GameManager.Instance.roomIndex, data);
    }

    public void CreateDashCollisionArea()
    {
        _damageArea = new GameObject();
        _damageArea.name = this.name + "DamageArea";
        _damageArea.transform.position = WeaponAttachment.position; //WeaponAttachMent에 충돌체를 달아준다
        _damageArea.transform.rotation = WeaponAttachment.rotation;
        _damageArea.transform.SetParent(WeaponAttachment); //WeaponAttachMent의 자식으로 _damageArea를 만든다

        _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
        _circleCollider2D.transform.position = WeaponAttachment.position + WeaponAttachment.rotation * AreaOffset;
        _circleCollider2D.radius = AreaSize.x / 2;
        _damageAreaCollider = _circleCollider2D;
        _damageAreaCollider.isTrigger = true;

        Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
        rigidBody.isKinematic = true;

        _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();

        if (isLocalUser)
        {
            _damageOnTouch.TargetLayerMask = EnemyLayerMask;
        }
        else
        {
            _damageOnTouch.TargetLayerMask = PlayerLayerMask;
        }


        _damageOnTouch.DamageCaused = DamageCaused;
        _damageOnTouch.CausedMSBCCType = DamageOnTouch.MSBCCStyles.Stun;
        _damageOnTouch.CCActivateCondition = CharacterStates.CharacterConditions.Normal;
        _damageOnTouch.StunDuration = 2.0f;
        //_damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
        //_damageOnTouch.InvincibilityDuration = InvincibilityDuration;
        _damageOnTouch.isBelongToLocalUser = isLocalUser;
        _damageOnTouch.collideOnce = collideOnce;
    }

    public void EnableDashCollisionArea()
    {
        _damageAreaCollider.enabled = true;
    }

    public void DisableDashCollisionArea()
    {
        _damageAreaCollider.enabled = false;
    }

    public IEnumerator DashCollisionAreaActivate()
    {
        yield return new WaitForSeconds(InitialDelay);
        EnableDashCollisionArea();
        yield return new WaitForSeconds(ActiveDuration);
        StopDash();               
    }


    public virtual void InitiateDash()
    {
        // we set its dashing state to true
        _movement.ChangeState(CharacterStates.MovementStates.Dashing);
        vectorY = 0;
        if (_controller.Speed.y <= -5)
        {
            vectorY = -1;
        }

        // we start our sounds
        PlayAbilityStartSfx();
        PlayAbilityUsedSfx();

        _cooldownTimeStamp = Time.time + DashCooldown;
        _dashCoroutine = Dash();
        StartCoroutine(_dashCoroutine);
        StartCoroutine(DashCollisionAreaActivate());
    }

    /// <summary>
    /// Coroutine used to move the player in a direction over time
    /// </summary>
    protected virtual IEnumerator Dash()
    {
        // if the character is not in a position where it can move freely, we do nothing.
        if (!AbilityPermitted
            || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
        {
            yield break;
        }

        // we initialize our various counters and checks
        _startTime = Time.time;
        _dashEndedNaturally = false;
        _initialPosition = this.transform.position;
        _distanceTraveled = 0;
        _shouldKeepDashing = true;
        _dashDirection = _character.IsFacingRight ? 1f : -1f;
        _computedDashForce = DashForce * _dashDirection;
        _dashVector = new Vector2(_dashDirection, vectorY).normalized * DashForce;
        //Debug.LogWarning(_dashVector);

        // we prevent our character from going through slopes
        _slopeAngleSave = _controller.Parameters.MaximumSlopeAngle;
        _controller.Parameters.MaximumSlopeAngle = 0;

        // we keep dashing until we've reached our target distance or until we get interrupted
        while (_distanceTraveled < DashDistance && _shouldKeepDashing && _movement.CurrentState == CharacterStates.MovementStates.Dashing)
        {
            _distanceTraveled = Vector3.Distance(_initialPosition, this.transform.position);

            // if we collide with something on our left or right (wall, slope), we stop dashing, otherwise we apply horizontal force
            if ((_controller.State.IsCollidingLeft || _controller.State.IsCollidingRight))
            {
                _shouldKeepDashing = false;
                _controller.SetForce(Vector2.zero);
            }
            else
            {
                //로컬유저가 아니면 SetHorizontalForce 하지 않고 UserMoveSync에 맞춘다
                if (isLocalUser)
                {
                    //Debug.LogWarning("Dash Initiated!");
                    //Debug.LogWarning("DashVector : " + _dashVector);
                    _controller.SetForce(_dashVector);
                    
                }
            }
            yield return null;
        }
        
        StopDash();
    }

    public virtual void StopDash()
    {
        StopCoroutine(_dashCoroutine);
        DisableDashCollisionArea();
        // once our dash is complete, we reset our various states

        _controller.DefaultParameters.MaximumSlopeAngle = _slopeAngleSave;
        _controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
        _controller.GravityActive(true);
        _dashEndedNaturally = true;

        // we play our exit sound
        StopAbilityUsedSfx();
        PlayAbilityStopSfx();

        // once the boost is complete, if we were dashing, we make it stop and start the dash cooldown
        if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
        {
            _movement.RestorePreviousState();
        }
    }

    /// <summary>
    /// Adds required animator parameters to the animator parameters list if they exist
    /// </summary>
    protected override void InitializeAnimatorParameters()
    {
        RegisterAnimatorParameter("Dashing", AnimatorControllerParameterType.Bool);
    }

    /// <summary>
    /// At the end of the cycle, we update our animator's Dashing state 
    /// </summary>
    public override void UpdateAnimator()
    {
        MMAnimator.UpdateAnimatorBool(_animator, "Dashing", (_movement.CurrentState == CharacterStates.MovementStates.Dashing), _character._animatorParameters);
    }

    
    protected virtual void OnDrawGizmos()
    {
        if (_damageAreaCollider == null) { return; }
        if (_character == null) { return; }

        float flipped = _character.IsFacingRight ? 1f : -1f;
        _gizmoOffset = AreaOffset;
        _gizmoOffset.x *= flipped;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position + _gizmoOffset, AreaSize.x / 2);        
    }

    
    protected virtual void OnDrawGizmosSelected()
    {
        if (_damageAreaCollider == null) { return; }
        if (_character == null) { return; }

        float flipped = _character.IsFacingRight ? 1f : -1f;
        _gizmoOffset = AreaOffset;
        _gizmoOffset.x *= flipped;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position + _gizmoOffset, AreaSize.x / 2);
    }
}
