using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;
using MoreMountains.Tools;
using Random = System.Random;

public class ScoreItem : Item
{
    public FloatingMessage ScoreMessagePrefab;
    public int score;
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Target = other.gameObject.MMGetComponentNoAlloc<MSB_Character>();
        if (Target == null)
        {
            Debug.LogWarning("Trigger is not Msb Character");
            return;
        }
        ItemTakenFeedback?.PlayFeedbacks();
        if(!Target.IsRemote)
            NetworkModule.GetInstance().RequestGameUserActionItem(Room, 0, ItemIndex);
        if (ScoreMessagePrefab != null)
        {
            float xOffset = UnityEngine.Random.Range(-1, 1);
            float yOffset = UnityEngine.Random.Range(-1, 1);
            Vector3 randomOffset = new Vector3(xOffset,yOffset,0);
            Instantiate(ScoreMessagePrefab, transform.position + randomOffset, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }
}
