using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject particleEffect;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("space");
            Vector3 objectPosition = gameObject.transform.position;
            Instantiate(particleEffect, objectPosition, Quaternion.identity);
            Destroy(gameObject);
            //서버에 어떤 블록이 부셔졌는지 보내야댐 블록마다 정보가 있어야됨
        }
    }
}
