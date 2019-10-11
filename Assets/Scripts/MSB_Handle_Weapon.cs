using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class MSB_Handle_Weapon : CharacterAbility
{

    public Transform WeaponAttachment;

    public bool AutomaticallyBindAnimator = true;

    public Animator CharacterAnimator { get; set; }

    public float _secondaryHorizontalMovement { get; set; }
    public float _secondaryVerticalMovement { get; set; }

    public float _thirdHorizontalMovement { get; set; }
    public float _thirdVerticalMovement { get; set; }

    protected WeaponAim _aimableWeapon;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Initialization()
    {
        base.Initialization();
    }

    public void SetUp()
    {
        if (WeaponAttachment == null)
        {
            WeaponAttachment = transform;
        }

        /*if (_animator != null)
        {
            _weaponIK = _animator.GetComponent<WeaponIK>();
        }*/
        
        _character = gameObject.GetComponentNoAlloc<Character>();
        CharacterAnimator = _animator;
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
    }

    protected override void HandleInput()
    {
        
    }

    protected override void InitializeAnimatorParameters()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
