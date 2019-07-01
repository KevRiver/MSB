using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClasses : MonoBehaviour
{
    
}

public interface IWeapon
{

}

public class WeaponInfo
{
    public float basicAtkDmg;
    public float basicAtkSpeed;
    public float moveSpeedMultiplier;
    public float jumpForceMultiplier;
}

public class WeaponDescription
{
    public string description;
    public string basicAtkDescription;
    public string activeSkillDescription;
    public string passiveSkillDescription;
}


[System.Serializable]
public abstract class WeaponBase : IWeapon
{
    public Vector3 attachPosition;
    public WeaponInfo wepaonInfo;
    public WeaponDescription weaponDescription;
    public Animator animator;

    public void AttachWeapon2Player(GameObject _player,Vector3 _position)
    {
        //무기를 Instantiate player의 자식으로 Instantiate 한 후 position을 바꿔준다
    }
}

[System.Serializable]
public class LaserSword : WeaponBase
{

}