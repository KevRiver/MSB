using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

//캐릭터가 빙글빙글 돈다
public class CharacterSpin : CharacterAbility
{
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
        if(AbilityPermitted)
            transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * spinSpeed * speedMultiplier);
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
