using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

//캐릭터가 빙글빙글 돈다
public class CharacterSpin : CharacterAbility
{
    private Vector3 _rotAngle = Vector3.zero;
    public GameObject characterModel;
    public float spinSpeed;
    public float speedMultiplier = 1;

    // Start is called before the first frame update
    protected override void Start()
    {
        Initialization();
    }

    protected override void Initialization()
    {
        base.Initialization();
    }

    public override void ProcessAbility()
    {
        if (characterModel == null)
            return;
        
        if (AbilityPermitted)
        {
            _rotAngle.z = Time.deltaTime * spinSpeed * speedMultiplier;
            characterModel.transform.Rotate(_rotAngle);
            //transform.Rotate(_rotAngle);
            //transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * spinSpeed * speedMultiplier);
        }
    }

    public void SetSpinSpeedMultiplier(float value)
    {
        speedMultiplier = value;
    }

    public void ResetSpinSpeedMultiplier()
    {
        speedMultiplier = 1.0f;
    }

    public override void LateProcessAbility()
    {
        base.LateProcessAbility();
    }

    public override void Flip()
    {
        spinSpeed *= -1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
