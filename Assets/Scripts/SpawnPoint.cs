using UnityEngine;
using MoreMountains.CorgiEngine;

public class SpawnPoint : MonoBehaviour
{
    // each SpawnPoint has number var which stores target MSB_Character's user number
    public int num;
    public Character.FacingDirections FacingDirection = Character.FacingDirections.Right;
    
    public virtual void SpawnPlayer(MSB_Character player)
    {
        player.RespawnAt(transform, FacingDirection);
    }
}
