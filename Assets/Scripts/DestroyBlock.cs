using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    public GameObject breakSound;

    public GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        breakSound = GameObject.Find("WoodBreak001");

        gameManager = GameObject.Find("GameManager");
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
            destroyBlock();
            // 서버에 어떤 블록이 부셔졌는지 보내야댐 블록마다 정보가 있어야됨
            Debug.Log("SEND");
            Debug.Log(gameObject.GetComponent<BlockData>().blockID);
            Debug.Log(gameObject);
            gameManager.GetComponent<GameManager>().sendBlockDestroy(gameObject.GetComponent<BlockData>().blockID);
        }
    }

    public void destroyBlock()
    {
        Debug.Log("DESTROY!!");         
        Vector3 objectPosition = gameObject.transform.position;
        objectPosition.z--;
        Instantiate(particleEffect, objectPosition, Quaternion.identity);

        breakSound.GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }
}
