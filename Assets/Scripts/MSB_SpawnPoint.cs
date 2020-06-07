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
        player.RespawnAt(transform, FacingDirection);
    }
}