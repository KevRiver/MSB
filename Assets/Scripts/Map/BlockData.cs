using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    public int blockID;
    public int objectID;

    // 오브젝트 타입
    public int objectType;
    /*
    Block Type  
    Gray Block - 0
    Brown Block - 1  
    Object Type
    JumpPad - 10
    Flag - 11  
    Dummy Player - 12  
    */

    // GameManager
    public GameObject gameManager;

    // Start is called before the first frame update
    void Start() {
        if (objectType < 10)
        {
            // 게임메니저에서 해시테이블 받아옴
            gameManager = GameObject.Find("GameManager");
            gameManager.GetComponent<GameManager>().mapHashtable.Add(blockID, gameObject);
            //Debug.Log(gameManager.GetComponent<GameManager>().mapHashtable[blockID]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
