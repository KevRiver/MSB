//
//  QueueLoadingObject
//  Created by 문주한 on 01/01/2020.
//
//  Queue Loading Object
//

using UnityEngine;
using System.Collections;

public class QueueLoadingObject : MonoBehaviour
{
    // Canvas
    ManageLobbyObject canvas;

    int room;

    // Use this for initialization
    void Start()
    {
        canvas = FindObjectOfType<ManageLobbyObject>();
    }

    public void getRoom(int _room)
    {
        room = _room;
    }

    void FinishQueue() {
        canvas.loadScene(room);
    }
}
