using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using MoreMountains.Tools;
using MSBNetwork;

public class Item : MonoBehaviour
{
    protected int _room;
    protected MSB_Character Target;
    public int ItemIndex;
    public float ItemSpawnDelay;
    private CircleCollider2D _circleCollider;
    

    public MMFeedbacks ItemSpawnFeedback;
    public MMFeedbacks ItemTakenFeedback;
    
    // Start is called before the first frame update
    protected virtual  void Start()
    {
        _room = GameInfo.Instance.room;
        ItemSpawnFeedback?.Initialization();
        ItemSpawnFeedback?.Initialization();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
       
    }

    protected virtual void OnEnable()
    {
        _circleCollider = GetComponentInParent<CircleCollider2D>();
        if (_circleCollider == null)
        {
            return;
        }

        _circleCollider.enabled = false;
        ItemSpawnFeedback?.PlayFeedbacks();
        Invoke("ColliderActivate", ItemSpawnDelay);
    }

    private void ColliderActivate()
    {
        _circleCollider.enabled = true;
    }
}
