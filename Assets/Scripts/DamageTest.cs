using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class DamageTest : MonoBehaviour
{
    public MSB_Character[] players;
    public int playerIndex;

    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectsOfType<MSB_Character>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DoDamage(playerIndex);
        }
    }

    public void DoDamage(int playerIndex)
    {
        MSB_Character player = players[playerIndex];
        Health health = player.GetComponent<Health>();
        if (health == null)
        {
            Debug.LogWarning("No Health component in this character");
            return;
        }

        health.Damage(10, null, 0.5f, 0.1f);
    }
}
