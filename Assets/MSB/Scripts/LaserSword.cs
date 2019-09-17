using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MSBNetwork;

public class LaserSword : Weapon
{     
    /// the possible shapes for the melee weapon's damage area
    public enum MeleeDamageAreaShapes { Rectangle, Circle }

    [Header("Damage Area")]
    /// the shape of the damage area (rectangle or circle)
    public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Rectangle;
    /// the size of the damage area
    public Vector2 AreaSize = new Vector2(1, 1);
    /// the offset to apply to the damage area (from the weapon's attachment position
    public Vector2 AreaOffset = new Vector2(1, 0);

    [Header("MSB Custom : Basic Attack Timing")]
    /// the initial delay to apply before triggering the damage area
    public float BasicAttackInitialDelay = 0f;
    /// the duration during which the damage area is active
    public float BasicAttackActiveDuration = 1f;

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
    public Vector2 KnockbackForce = new Vector2(30, 30);
    /// The duration of the invincibility frames after the hit (in seconds)
    public float InvincibilityDuration = 0.5f;

    protected Collider2D _damageAreaCollider;
    protected bool _attackInProgress = false;

    protected Color _gizmosColor;
    protected Vector3 _gizmoSize;

    protected CircleCollider2D _circleCollider2D;
    protected BoxCollider2D _boxCollider2D;
    protected Vector3 _gizmoOffset;
    protected DamageOnTouch _damageOnTouch;
    protected GameObject _damageArea;

    /// <summary>
    /// Initialization
    /// </summary>

    
    public override void Initialization()
    {
        base.Initialization();
        
        if (_damageArea == null)
        {
            
            CreateDamageArea();
            DisableDamageArea();
        }
        
        _damageOnTouch.Owner = ((MSB_Character)Owner).gameObject;
    }

    /// <summary>
    /// Creates the damage area.
    /// </summary>
    
    const int LAYER_ENEMIES = 13;
    const int LAYER_PLAYER = 9;
    protected virtual void CreateDamageArea()
    {
        _damageArea = new GameObject();
        _damageArea.name = this.name + " DamageArea";
        _damageArea.transform.position = this.transform.position;
        _damageArea.transform.rotation = this.transform.rotation;
        _damageArea.transform.SetParent(this.transform);

        if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
        {
            _boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
            _boxCollider2D.offset = AreaOffset;
            _boxCollider2D.size = AreaSize;
            _damageAreaCollider = _boxCollider2D;
        }
        if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
        {
            _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
            _circleCollider2D.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
            _circleCollider2D.radius = AreaSize.x / 2;
            _damageAreaCollider = _circleCollider2D;
        }
        _damageAreaCollider.isTrigger = true;

        Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
        rigidBody.isKinematic = true;

        _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();

        if (Owner.isLocalUser)
        {
            _damageOnTouch.TargetLayerMask = EnemyLayerMask;
        }
        else
        {
            _damageOnTouch.TargetLayerMask = PlayerLayerMask;
        }

        _damageOnTouch.DamageCaused = DamageCaused;
        _damageOnTouch._damageAreaType = DamageOnTouch.DamageAreaType.Trigger;
        _damageOnTouch.CCActivateCondition = CharacterStates.CharacterConditions.Stun;
        _damageOnTouch.CausedMSBCCType = DamageOnTouch.MSBCCStyles.KnockBack;
        _damageOnTouch.CausedKnockbackForce = new Vector2(30, 5);
        _damageOnTouch.shortStunDuration = 0.5f;
        _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
        _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
        //Debug.Log(((MSB_Character)Owner).c_userData.userNick + "'s LaserSword is belong to local user : " + isBelongToLocalUser);
        _damageOnTouch.isBelongToLocalUser = isBelongToLocalUser;
        _damageOnTouch.collideOnce = collideOnce;
    }

    private void OnEnable()
    {
        
    }

    /// <summary>
    /// When the weapon is used, we trigger our attack routine
    /// </summary>
    public override void WeaponUse()
    {
        //Debug.LogWarning("Overrided WeaponUse Called");
        base.WeaponUse();
        StartCoroutine(LaserSwordBasicAttack());
    }
    
    public IEnumerator LaserSwordBasicAttack()
    {
        if (_attackInProgress) { yield break; }

        _attackInProgress = true;
        yield return new WaitForSeconds(BasicAttackInitialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(BasicAttackActiveDuration);
        DisableDamageArea();
        _attackInProgress = false;

    }
    
    /// <summary>
    /// Enables the damage area.
    /// </summary>
    protected virtual void EnableDamageArea()
    {
        _damageAreaCollider.enabled = true;
    }

    /// <summary>
    /// Disables the damage area.
    /// </summary>
    protected virtual void DisableDamageArea()
    {
        _damageAreaCollider.enabled = false;
    }

    protected virtual void OnDrawGizmos()
    {
        if (_damageAreaCollider == null) { return; }
        if (Owner == null) { return; }

        float flipped = Owner.IsFacingRight ? 1f : -1f;
        _gizmoOffset = AreaOffset;
        _gizmoOffset.x *= flipped;

        Gizmos.color = Color.white;
        if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
        {
            Gizmos.DrawWireSphere(this.transform.position + _gizmoOffset, AreaSize.x / 2);
        }
        if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
        {
            MMDebug.DrawGizmoRectangle(this.transform.position + _gizmoOffset, AreaSize, Color.red);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (_damageAreaCollider == null) { return; }
        if (Owner == null) { return; }

        float flipped = Owner.IsFacingRight ? 1f : -1f;
        _gizmoOffset = AreaOffset;
        _gizmoOffset.x *= flipped;

        Gizmos.color = Color.white;
        if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
        {
            Gizmos.DrawWireSphere(this.transform.position + _gizmoOffset, AreaSize.x / 2);
        }
        if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
        {
            MMDebug.DrawGizmoRectangle(this.transform.position + _gizmoOffset, AreaSize, Color.red);
        }
    }
   
}
