using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillClasses : MonoBehaviour
{
  
}

public interface Skill
{

}



[System.Serializable]
public abstract class SkillBase : Skill
{
    public string skillName;
    public Sprite skillIcon;
    public string skillDescription;
    public float coolTime;
}

[System.Serializable]
public abstract class ActiveSkill : SkillBase
{
    
}

[System.Serializable]
public abstract class PassiveSkill : SkillBase
{

}

public interface IMelee
{
    void MeleeAttack(GameObject _weaponObj, float _dmg, float _range);
}

public interface IShootable
{
    void Shoot(GameObject _weaponObj, float _force);
}

public interface IFire
{
    void Fire(GameObject _weaponObj, float _range);
}

public interface IComboable
{
    void ComboAttack(int _atkTimes, float _endTime);
}

public interface IChargable
{
    void Charge(float _chargeTime);
}

public interface IDashable
{
    void Dash(GameObject _player, float _direction, float _force);
}