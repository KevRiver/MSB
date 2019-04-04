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
        }
    }
}
