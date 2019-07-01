using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private float moveSpeed;
    private float jumpForce;
    public float hp;

    public GameObject prefab;   //유저가 선택한 캐릭터의 프리팹
    public GameObject weapon;   // 유저가 선택한 무기

    // Start is called before the first frame update
    void Start()
    {
        hp = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
