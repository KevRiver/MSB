﻿using System.Collections;
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

    [Header("Damage Area Timing")]
    /// the initial delay to apply before triggering the damage area
    public float InitialDelay = 0f;
    /// the duration during which the damage area is active
    public float ActiveDuration = 1f;

    [Header("Damage Caused")]
    // the layers that will be damaged by this object
    public LayerMask TargetLayerMask;
    /// The amount of health to remove from the player's health
    public int DamageCaused = 10;
    /// the kind of knockback to apply
    public DamageOnTouch.KnockbackStyles Knockback;
    /// The force to apply to the object that gets damaged
    public Vector2 KnockbackForce = new Vector2(10, 2);
    /// The duration of the invincibility frames after the hit (in seconds)
    public float InvincibilityDuration = 0.5f;

    public float StunDuration;

    protected Collider2D _damageAreaCollider;
    protected bool _attackInProgress = false;

    protected Color _gizmosColor;
    protected Vector3 _gizmoSize;

    protected CircleCollider2D _circleCollider2D;
    protected BoxCollider2D _boxCollider2D;
    protected Vector3 _gizmoOffset;
    protected DamageOnTouch _damageOnTouch;
    protected MSB_DamageOnTouch _msbDamageOnTouch;
    public CausedCCType ccType;
    protected GameObject _damageArea;

    private bool _isOwnerRemote = false;
    private Transform _aimIndicator;

    /// <summary>
    /// Initialization
    /// </summary>
    public override void Initialization()
    {
        base.Initialization();
        _aimIndicator = transform.parent.GetChild(0);
        if (!_aimIndicator.gameObject.activeInHierarchy)
            _aimIndicator = null;
        if (_damageArea == null)
        {
            CreateDamageArea();
            DisableDamageArea();
        }
    }

    /// <summary>
    /// Creates the damage area.
    /// </summary>
    protected virtual void CreateDamageArea()
    {
        _damageArea = new GameObject();
        _damageArea.name = this.name + "DamageArea";
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

        _msbDamageOnTouch = _damageArea.AddComponent<MSB_DamageOnTouch>();
        _msbDamageOnTouch.TargetLayerMask = TargetLayerMask;
        _msbDamageOnTouch.CCType = ccType;
        _msbDamageOnTouch.Owner = Owner.gameObject;
        _msbDamageOnTouch._ownerCharacter = Owner.GetComponent<MSB_Character>();
        if (_msbDamageOnTouch._ownerCharacter != null)
        {
            foreach (var player in MSB_LevelManager.Instance.Players)
            {
                if (player.team == _msbDamageOnTouch._ownerCharacter.team)
                    _msbDamageOnTouch.IgnoreGameObject(player.gameObject);
            }
        }

        _msbDamageOnTouch.DamageCaused = DamageCaused;
        _msbDamageOnTouch.DamageCausedKnockbackType = Knockback;
        _msbDamageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
        _msbDamageOnTouch.InvincibilityDuration = InvincibilityDuration;
        _msbDamageOnTouch.stunDuration = StunDuration;
    }

    public override void SetOwner(Character newOwner, CharacterHandleWeapon handleWeapon)
    {
        if (((MSB_Character) newOwner).IsRemote)
            _isOwnerRemote = true;
        base.SetOwner(newOwner,handleWeapon);
    }

    /// <summary>
    /// When the weapon is used, we trigger our attack routine
    /// </summary>
    protected override void WeaponUse()
    {
        Owner.GetComponent<CharacterSpin>().SetSpinSpeedMultiplier(0.1f);
        base.WeaponUse();
        if(!_isOwnerRemote)
            RCSender.Instance.RequestUserSync();
        StartCoroutine(MeleeWeaponAttack());
    }

    protected override void CaseWeaponIdle()
    {
        base.CaseWeaponIdle();
        if (!_aimIndicator)
            return;
        _aimIndicator.gameObject.SetActive(true);
    }

    protected override void CaseWeaponUse()
    {
        base.CaseWeaponUse();
        if (!_aimIndicator)
            return;
        _aimIndicator.gameObject.SetActive(false);
    }

    public override void TurnWeaponOff()
    {
        Owner.GetComponent<CharacterSpin>().ResetSpinSpeedMultiplier();
        base.TurnWeaponOff();
    }

    /// <summary>
    /// Triggers an attack, turning the damage area on and then off
    /// </summary>
    /// <returns>The weapon attack.</returns>
    protected virtual IEnumerator MeleeWeaponAttack()
    {
        if (_attackInProgress) { yield break; }
        _attackInProgress = true;
        yield return new WaitForSeconds(InitialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(ActiveDuration);
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

    public override void FlipWeapon()
    {
        //Debug.LogWarning("LaserSword Flip");
    }

    public override void FlipWeaponModel()
    {
        //Debug.LogWarning("LaserSword Flip Model");
        transform.localScale = Vector3.Scale (transform.localScale, FlipValue);		
    }

    protected virtual void DrawGizmos()
    {
        _gizmoOffset = AreaOffset;

        Gizmos.color = Color.red;
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
        if (!Application.isPlaying)
        {
            DrawGizmos();
        }
    }
}
