using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    public GameObject breakSound;

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        breakSound = GameObject.Find("BreakSound");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject particleEffect;

    /*private void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            destroyBlock();
            // 서버에 어떤 블록이 부셔졌는지 보내야댐 블록마다 정보가 있어야됨
            JSONObject jsonData = new JSONObject();
            jsonData.AddField("blockIndex", gameObject.GetComponent<BlockData>().blockID);
            gameManager.sendBlockDestroy(jsonData);
        }
    }*/

    public void destroyBlock()
    { 
        Vector3 objectPosition = gameObject.transform.position;
        objectPosition.z--;
        Instantiate(particleEffect, objectPosition, Quaternion.identity);

        sendDestroidBlock();

        breakSound.GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }

    void sendDestroidBlock()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("blockIndex", gameObject.GetComponent<BlockData>().blockID);
        gameManager.sendBlockDestroy(jsonData);
    }
}
