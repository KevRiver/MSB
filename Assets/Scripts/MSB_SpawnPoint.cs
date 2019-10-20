using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class MSB_SpawnPoint : MonoBehaviour
{
    public Character.FacingDirections FacingDirection;
    public int SpawnerIndex;

    public void SpawnPlayer(MSB_Character player)
    {
        Debug.Log("MSB_SpawnPoint::SpawnPlayer");
        player.RespawnAt(transform, FacingDirection);
    }

    protected void OnDrawGizmos()
    {
        //Debug.Log("CheckPoint::OnDrawGizmos");
#if UNITY_EDITOR

        if (MSB_LevelManager.Instance == null)
        {
            return;
        }

        if (MSB_LevelManager.Instance.Spawnpoints == null)
        {
            return;
        }

        if (MSB_LevelManager.Instance.Spawnpoints.Count == 0)
        {
            return;
        }

        for (int i = 0; i < MSB_LevelManager.Instance.Spawnpoints.Count; i++)
        {
            // we draw a line towards the next point in the path
            if ((i + 1) < MSB_LevelManager.Instance.Spawnpoints.Count)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(MSB_LevelManager.Instance.Spawnpoints[i].transform.position, MSB_LevelManager.Instance.Spawnpoints[i + 1].transform.position);
            }
        }
#endif
    }
}
