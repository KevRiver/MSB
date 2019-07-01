using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjects : MonoBehaviour
{
    //플레이어에 붙는 여러 자식 게임 오브젝트들을 등록함
    public GameObject aimAxis;
    public GameObject weaponAxis;
    public GameObject headUpPosition;
    
    void Awake()
    {
        //aimAxis = gameObject.transform.Find("AimAxis").gameObject;
        //weaponAxis = gameObject.transform.Find("WeaponAxis").gameObject;
    }
}
