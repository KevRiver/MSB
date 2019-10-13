using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class MSB_CheckPoint : CheckPoint
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnDrawGizmos()
    {
        /*
        Debug.Log("MSB_CheckPoint::OnDrawGizmos");
        
        #if UNITY_EDITOR

        if (MSB_LevelManager.Instance == null)
        {
            return;
        }

        if (MSB_LevelManager.Instance.Checkpoints == null)
        {
            return;
        }

        if (MSB_LevelManager.Instance.Checkpoints.Count == 0)
        {
            return;
        }

        for (int i = 0; i < MSB_LevelManager.Instance.Checkpoints.Count; i++)
        {
            // we draw a line towards the next point in the path
            if ((i + 1) < MSB_LevelManager.Instance.Checkpoints.Count)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(MSB_LevelManager.Instance.Checkpoints[i].transform.position, MSB_LevelManager.Instance.Checkpoints[i + 1].transform.position);
            }
        }
    #endif*/
    }
}
