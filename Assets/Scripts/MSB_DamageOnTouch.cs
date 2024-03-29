﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public enum CausedCCType
{
    Non,
    KnockBack,
    Stun,
}

public class MSB_DamageOnTouch : DamageOnTouch
{
    public LayerMask[] TargetLayerMasks;
    [Header("MSB Custom")]
    private MSB_Projectile _projectile;
    public MSB_Character _ownerCharacter;

    [Space(10)]
    
    [Header("MSB CC Options")] //caused cc types and duration
    public CausedCCType CCType;
    public float stunDuration;
    
    
    protected override void Awake()
    {
        base.Awake();
        _projectile = GetComponent<MSB_Projectile>();
        //_ownerCharacter = Owner.GetComponent<MSB_Character>();
        if(_projectile != null)
            IgnoreGameObject(_projectile._owner);
        
        
        //Debug.LogError("PlayerLayer : " + LayerMask.NameToLayer("Player"));
        //Debug.LogError("PlatformLayer : " + LayerMask.NameToLayer("Platform"));
        //Debug.LogError("Target LayerMask : " + TargetLayerMask);
    }

    public override void OnTriggerStay2D(Collider2D collider)
    {
    }

    public override void OnTriggerEnter2D(Collider2D collider)
    {
        Colliding(collider);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        string objectName = other.gameObject.name;
        //Debug.Log("Trigger Exit : " + objectName);
    }

    protected override void Colliding(Collider2D collider)
    {
        if (!this.isActiveAndEnabled)
        {
            return;
        }

        // if the object we're colliding with is part of our ignore list, we do nothing and exit
        if (_ignoredGameObjects.Contains(collider.gameObject))
        {
            return;
        }

        // if what we're colliding with isn't part of the target layers, we do nothing and exit
        if (!MMLayers.LayerInLayerMask(collider.gameObject.layer,TargetLayerMask))
        {
            return;
        }
        //Debug.LogWarning("DamageOnTouch : " + collider.gameObject);
        _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();
        if (_colliderHealth != null)
        {
            OnCollideWithDamageable(_colliderHealth);
        }
        else
        {
            OnCollideWithNonDamageable();
        }
    }

    private MSB_Character _colliderMsbCharacter;
    private MSB_Character _ownerMsbCharacter;
    private int _targetNum;
    private string _options;
    protected override void OnCollideWithDamageable(Health health)
    {
        _ownerMsbCharacter = Owner.gameObject.GetComponent<MSB_Character>();
        _colliderMsbCharacter = health.gameObject.MMGetComponentNoAlloc<MSB_Character>();
        _colliderCorgiController = health.gameObject.MMGetComponentNoAlloc<CorgiController>();
        if ((_colliderCorgiController != null) && (DamageCausedKnockbackForce != Vector2.zero) && (!_colliderHealth.TemporaryInvulnerable) && (!_colliderHealth.Invulnerable) && (!_colliderHealth.ImmuneToKnockback))
        {
            _knockbackForce.x = DamageCausedKnockbackForce.x;
            if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnSpeed)
            {
                Vector2 totalVelocity = _colliderCorgiController.Speed + _velocity;
                _knockbackForce.x *= -1 * Mathf.Sign(totalVelocity.x);
            }
            if (DamagedTakenKnockbackDirection == KnockbackDirections.BasedOnOwnerPosition)
            {
                if (Owner == null) { Owner = this.gameObject; }
                Vector2 relativePosition = _colliderCorgiController.transform.position - Owner.transform.position;
                _knockbackForce.x *= Mathf.Sign(relativePosition.x);
            }
				
            _knockbackForce.y = DamageCausedKnockbackForce.y;
        }

        if (!_ownerMsbCharacter.IsRemote)
        {
            _targetNum = _colliderMsbCharacter.UserNum;
            _options = ((int) CCType).ToString() + "," + _knockbackForce.x.ToString() + "," + _knockbackForce.y.ToString() + "," + stunDuration.ToString();
            RCSender.Instance.RequestDamage(_targetNum, DamageCaused, _options);
        }
        
        if(_health!=null)
            SelfDamage(DamageTakenEveryTime + DamageTakenNonDamageable);
    }
    protected override void OnCollideWithNonDamageable()
    {
        if(_health!=null)
            SelfDamage(DamageTakenEveryTime + DamageTakenNonDamageable);
    }
}
