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
    public int roomIndex;
    public FacingDirections CharacterFacingDirection { get; set; }
    public Transform weaponAttachment { get; set; }
    public int playerIndex;
    public Vector3 targetPos;
    public Vector2 RecievedSpeed { get; set; }
    public bool RecievedGroundCheck { get; set; }   
    public bool RecievedFacingDirection { get; set; }

    //Action Sync
    public CharacterAbility.ActionType RecievedActionType { get; set; }
    public bool RecievedActionAimable { get; set; }
    public float RecievedActionDirX { get; set; }
    public float RecievedActionDirY { get; set; }
    public string RecievedActionState { get; set; }

    protected override void Awake()
    {
        Initialization();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Color col = _spriteRenderer.color;
        col.a = 1.0f;
        _spriteRenderer.color = col;
    }
    public void OnBushEvent()
    {
        if (bushID == 0)
        {
            foreach (MSB_Character player in MSB_LevelManager.Instance.MSB_Players)
            {
                // Player SpriteRenderer
                SpriteRenderer sprite = player.gameObject.GetComponent<SpriteRenderer>();
                // Player's Children SpriteRenderer(Weapon, etc...)
                SpriteRenderer[] playerChildrenObjects = player.gameObject.GetComponentsInChildren<SpriteRenderer>();
                // Player's HP Bar
                Image[] hpBarImages = player.GetComponentsInChildren<Image>();

                // Player's Color
                Color col = sprite.color;

                if (player.isLocalUser)
                {
                    col.a = 1f;                    
                    sprite.color = col;
                    foreach (SpriteRenderer child in playerChildrenObjects) // Weapon, etc(Sprite Renderer)...
                    {
                        Color colChild = child.color;
                        colChild.a = 1f;
                        child.color = colChild;
                    }
                    for (int i = 0; i < hpBarImages.Length; i++) // HP Bar, etc(Image)...
                    {
                        hpBarImages[i].color = new Color(hpBarImages[i].color.r, hpBarImages[i].color.g, hpBarImages[i].color.b, 1f);
                    }
                    continue;
                }

                if (player.bushID != 0)
                    col.a = 0f; // 투명
                else
                    col.a = 1f; // 불투명

                sprite.color = col;

                foreach (SpriteRenderer child in playerChildrenObjects) // Weapon, etc(Sprite Renderer)...
                {
                    Color colChild = child.color;

                    if (player.bushID != 0)
                        colChild.a = 0f; // 투명
                    else
                        colChild.a = 1f; // 불투명

                    child.color = colChild;
                }

                for (int i = 0; i < hpBarImages.Length; i++) // HP Bar, etc(Image)...
                {
                    if (player.bushID != 0)
                        hpBarImages[i].color = new Color(hpBarImages[i].color.r, hpBarImages[i].color.g, hpBarImages[i].color.b, 0f); // 투명
                    else
                        hpBarImages[i].color = new Color(hpBarImages[i].color.r, hpBarImages[i].color.g, hpBarImages[i].color.b, 1f); // 불투명
                }
                
            }
        }
        else
        {
            foreach (MSB_Character player in MSB_LevelManager.Instance.MSB_Players)
            {
                // Player SpriteRenderer
                SpriteRenderer sprite = player.gameObject.GetComponent<SpriteRenderer>();
                // Player's Children SpriteRenderer(HP bar, Weapon)
                SpriteRenderer[] playerChildrenObjects = player.gameObject.GetComponentsInChildren<SpriteRenderer>();
                // Player's HP Bar
                Image[] hpBarImages = player.gameObject.GetComponentsInChildren<Image>();

                // Player's Color
                Color col = sprite.color;                

                if (player.isLocalUser)
                {
                    col.a = 0.5f;                   
                    sprite.color = col;
                    foreach (SpriteRenderer child in playerChildrenObjects) // Weapon, etc(Sprite Renderer)...
                    {
                        Color colChild = child.color;
                        colChild.a = 0.5f;
                        child.color = colChild;
                    }

                    for (int i = 0; i < hpBarImages.Length; i++) // HP Bar, etc(Image)...
                    {
                        hpBarImages[i].color = new Color(hpBarImages[i].color.r, hpBarImages[i].color.g, hpBarImages[i].color.b, 1f); // 불투명
                    }
                    continue;
                }

                if (player.bushID != 0)
                {
                    if (player.bushID != bushID)
                        col.a = 0f; // 투명
                    else
                        col.a = 0.5f; // 반투명
                }
                else
                {
                    // 불투명
                    col.a = 1f;                   
                }

                sprite.color = col;

                foreach (SpriteRenderer child in playerChildrenObjects) // Weapon, etc(Sprite Renderer)...
                {
                    Color colChild = child.color;

                    if (player.bushID != 0)
                    {
                        if (player.bushID != bushID)
                            colChild.a = 0f; // 투명
                        else
                            colChild.a = 0.5f; // 반투명
                    }
                    else
                        colChild.a = 1f; // 불투명

                    child.color = colChild;
                }

                for (int i = 0; i < hpBarImages.Length; i++) // HP Bar, etc(Image)...
                {
                    if (player.bushID != 0)
                    {
                        if (player.bushID != bushID)
                            hpBarImages[i].color = new Color(hpBarImages[i].color.r, hpBarImages[i].color.g, hpBarImages[i].color.b, 0f); // 투명
                        else
                            hpBarImages[i].color = new Color(hpBarImages[i].color.r, hpBarImages[i].color.g, hpBarImages[i].color.b, 1f); // 불투명
                    }
                    else
                        hpBarImages[i].color = new Color(hpBarImages[i].color.r, hpBarImages[i].color.g, hpBarImages[i].color.b, 1f); // 불투명
                }

            }
        }
    }

    protected override void Initialization()
    {
        MovementState = new MMStateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        ConditionState = new MMStateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);

        c_userData = new ClientUserData();

        if (InitialFacingDirection == FacingDirections.Left)
        {
            CharacterFacingDirection = FacingDirections.Left;
            IsFacingRight = false;
        }
        else
        {
            CharacterFacingDirection = FacingDirections.Right;
            IsFacingRight = true;
        }

        // Get roomIndex from MSB_GameManager
        roomIndex = MSB_GameManager.Instance.roomIndex;

        // we get the current input manager
        SetInputManager();

        //로컬플레이어라면 recievedSpeed 값이 변하지 않습니다
        RecievedSpeed = Vector2.zero;

        // we get the main camera
        if (Camera.main == null) { return; }
        SceneCamera = Camera.main.GetComponent<CameraController>();
        // we store our components for further use 
        CharacterState = new CharacterStates();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<CorgiController>();
        _characterAbilities = GetComponents<CharacterAbility>();
        _aiBrain = GetComponent<AIBrain>();
        _health = GetComponent<Health>();
        weaponAttachment = transform.GetChild(0);   //PlayerPrefab의 첫번째 자식은 WeaponAttachment 이다
        _damageOnTouch = GetComponent<DamageOnTouch>();
        CanFlip = true;
        RecievedActionType = CharacterAbility.ActionType.NULL;
        RecievedActionState = null;
        AssignAnimator();
        _characterhorizontalMovement = GetComponent<CharacterHorizontalMovement>();
        _characterJump = GetComponent<CharacterJump>();
        originmalJumpHeight = _characterJump.JumpHeight;
        originalWalkSpeed = _characterhorizontalMovement.WalkSpeed;

        _originalGravity = _controller.Parameters.Gravity;

        ForceSpawnDirection();
    }

    protected override void EarlyProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.EarlyProcessAbility();
            }
        }
    }

    public override void RespawnAt(Transform spawnPoint, FacingDirections facingDirection)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("Spawn : your Character's gameobject is inactive");
            return;
        }

        // we make sure the character is facing right
        Face(facingDirection);
        //FlipWeaponDirection(facingDirection, true);

        // we raise it from the dead (if it was dead)
        ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        // we re-enable its 2D collider
        GetComponent<Collider2D>().enabled = true;
        // we make it handle collisions again
        _controller.CollisionsOn();
        transform.position = spawnPoint.position;
        if (_health != null)
        {
            _health.ResetHealthToMaxHealth();
            _health.Revive();
        }
    }

    public override void SetInputManager()
    {
        if (!string.IsNullOrEmpty(PlayerID))
        {
            LinkedInputManager = null;
            InputManager[] foundInputManagers = FindObjectsOfType(typeof(InputManager)) as InputManager[];
            foreach (InputManager foundInputManager in foundInputManagers)
            {
                Debug.Log("SetInputManager");
                if (isLocalUser)
                {
                    foundInputManager.PlayerID = this.PlayerID;
                    LinkedInputManager = foundInputManager;
                    UpdateInputManagersInAbilities();
                }
            }
        }
        
    }

    public void Start()
    {
        //현재는 Start에서 RequestUserMove 코루틴을 시작하지만 
        //앞으로 GameStatus CountDown이 0이 되었을 때 시작해야한다

        ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        _characterhorizontalMovement.WalkSpeed = originalWalkSpeed;
        _characterJump.JumpHeight = originmalJumpHeight;
        foreach (CharacterAbility ability in _characterAbilities)
        {
            ability.AbilityPermitted = true;
        }
        if (isLocalUser)
        {
            StartCoroutine(RequestUserMove());
        }
        else
        {
            Debug.LogWarning("MSB_Character Start RecievdSpeed: " + RecievedSpeed);
            Debug.LogWarning("MSB_Character Start RecievedPos: " + targetPos);
        }
    }


    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        if (!isLocalUser)
        {
            SyncUserPos(RecievedSpeed);           
        }
    }

    /// <summary>
    /// 연결된 플레이어의 Position과 그 캐릭터가 바라보고 있는 방향 동기화
    /// </summary>
    /// <param name="_Speed"></param>
    private void SyncUserPos(Vector2 _Speed)
    {
        float smoothTime = 0.1f;
        float xVelocity = _Speed.x;
        float yVelocity = _Speed.y;

        //Flip SpriteRenderer and WeaponAttachment Rotation
        if (IsFacingRight)
        {
            if (!RecievedFacingDirection)
            {
                Flip();               
            }
        }
        else
        {
            if (RecievedFacingDirection)
            {
                Flip();
            }
        }

        float newPosX = Mathf.SmoothDamp(transform.position.x, targetPos.x, ref xVelocity, smoothTime);
        float newPosY = transform.position.y;

        //Original Player는 착지해 있지만 Remote의 캐릭터가 착지해 있지 않은 경우 중력적용을 멈추고 Remote의 Position을 targetPos로 싱크 후 중력을 적용한다
        if (!_controller.State.IsGrounded && (RecievedGroundCheck == true))
        {
            _controller.GravityActive(false);
            transform.position = new Vector3(targetPos.x, targetPos.y);
            _controller.GravityActive(true);
            return;
        }

        //Original Player가 바닥에 닿아있지 않을동안 Original Player가 받고있는 Speed.y 값을 Remote에 적용시킨다
        if (!RecievedGroundCheck)
        {
            _controller.SetVerticalForce(yVelocity);
        }        
        transform.position = new Vector3(newPosX, newPosY);
    }    

    IEnumerator RequestUserMove()
    {
        while (true)
        {
            Vector2 speed = _controller.Speed;
            bool isGrounded = _controller.State.IsGrounded;
            string data = c_userData.userNumber.ToString() +
                "," + transform.position.x.ToString() +
                "," + transform.position.y.ToString() +
                "," + transform.position.z.ToString() +
                "," + speed.x.ToString() +
                "," + speed.y.ToString() +
                "," + isGrounded.ToString() +
                "," + IsFacingRight.ToString();                

            NetworkModule.GetInstance().RequestGameUserMove(roomIndex, data);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RequestStop()
    {
        StopCoroutine(RequestUserMove());
    }

    public void OnGameOver()
    {
        if (isLocalUser)
        {
            _controller.SetForce(Vector2.zero);
            RequestStop();
        }
        else
        {
            RecievedSpeed = Vector2.zero;
        }
    }

    private void OnDestroy()
    {
        Debug.LogWarning("MSB_Character OnDestroy");
        RecievedSpeed = Vector2.zero;
        if (isLocalUser)
        {
            RequestStop();
        }
    }
}
