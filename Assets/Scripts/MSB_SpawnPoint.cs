using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class MSB_SpawnPoint : CheckPoint
{
    public void SpawnPlayer(MSB_Character player)
    {
        Debug.Log("CheckPoint::SpawnPlayer");
        player.RespawnAt(transform, FacingDirection);

        foreach (Respawnable listener in _listeners)
        {
            listener.OnPlayerRespawn(this, player);
        }
    }

    /// <summary>
    /// MSB Custom : we don't need check point. Only respawn point needed
    /// </summary>
    /// <param name="collider">Something colliding with the water.</param>
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        //Do nothing
    }

    /// <summary>
    /// On DrawGizmos, we draw lines to show the path the object will follow
    /// </summary>
    protected override void OnDrawGizmos()
    {
        //Debug.Log("CheckPoint::OnDrawGizmos");
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
#endif
    }
}
