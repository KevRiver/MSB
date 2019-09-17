using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class Bush : MonoBehaviour
{

    public int bushID = 0;
    static MSB_Character localPlayer;

    public static void LazyLoad()
    {        
        List<MSB_Character> users = MSB_LevelManager.Instance.MSB_Players;
        foreach (MSB_Character user in users)
        {
            //Debug.LogWarning(user);
            if (user.isLocalUser)
            {
                Debug.LogWarning(localPlayer);
                localPlayer = user;
                break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MSB_Character>() != null)
        {
            collision.gameObject.GetComponent<MSB_Character>().bushID = bushID;
            localPlayer.OnBushEvent();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MSB_Character>() != null)
        {
            collision.gameObject.GetComponent<MSB_Character>().bushID = 0;
            localPlayer.OnBushEvent();
        }
    }

}