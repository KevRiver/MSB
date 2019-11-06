using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;
using MoreMountains.Tools;

public class ScoreItem : Item
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Target = other.gameObject.MMGetComponentNoAlloc<MSB_Character>();
        if (Target == null)
        {
            Debug.LogWarning("Trigger is not Msb Character");
            return;
        }
        NetworkModule.GetInstance().RequestGameUserActionItem(Room, 0, ItemIndex);
        gameObject.SetActive(false);
    }
}
