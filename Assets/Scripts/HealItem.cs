using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;
using MoreMountains.Tools;

public class HealItem : Item
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Target = other.gameObject.MMGetComponentNoAlloc<MSB_Character>();
        if (Target == null)
        {
            return;
        }
        ItemTakenFeedback?.PlayFeedbacks();
        if(!Target.IsRemote)
            NetworkModule.GetInstance().RequestGameUserActionItem(_room, 1, ItemIndex);
        gameObject.SetActive(false);
    }
}
