using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwordSkills : MonoBehaviour
{
    
}

[System.Serializable]
public class LaserSwordBasicAtk : ActiveSkill, IMelee, IComboable
{
    public void MeleeAttack(GameObject _weaponObj, float _dmg, float _range)
    {

    }

    public void ComboAttack(int _atkTimes, float _endTime)
    {

    }
}

[System.Serializable]
public class LaserSwordActiveSkill : ActiveSkill
{

}

[System.Serializable]
public class LaserSwordPassiveSkill : PassiveSkill
{

}