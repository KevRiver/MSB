using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;
using MoreMountains.Tools;

public class HealItem : Item
{
    // Start is called before the first frame update
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Target = other.gameObject.MMGetComponentNoAlloc<MSB_Character>();
        if (Target == null)
        {
            Debug.LogWarning("Trigger is not Msb Character");
            return;
        }
        ItemTakenFeedback?.PlayFeedbacks();
        NetworkModule.GetInstance().RequestGameUserActionItem(Room, 1, ItemIndex);
        gameObject.SetActive(false);
    }
}
