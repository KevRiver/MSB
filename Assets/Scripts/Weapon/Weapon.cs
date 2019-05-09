using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected GameObject weaponPrefab;
    protected Animation attackAnim;
    protected Animation skillAnim;
    protected float attackDmg;
    protected float skillDmg;
    protected float attackCoolTime;
    protected float skillCoolTime;

    protected virtual void ShowAttackAnim(Vector2 dir) { }
    protected virtual void ShowSkillAnim(Vector2 dir) { }
}
