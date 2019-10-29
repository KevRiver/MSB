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
        base.OnTriggerEnter2D(other);
        NetworkModule.GetInstance().RequestGameUserActionItem(Room, 0, ItemIndex);
        gameObject.SetActive(false);
    }
}
