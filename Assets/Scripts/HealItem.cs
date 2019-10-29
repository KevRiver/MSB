using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;

public class HealItem : Item
{
    // Start is called before the first frame update
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        NetworkModule.GetInstance().RequestGameUserActionItem(Room, 1, ItemIndex);
        gameObject.SetActive(false);
    }
}
