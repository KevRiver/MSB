using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using Random = UnityEngine.Random;

public enum FloatingMessageType
{
    Damage,
    Heal,
    KnockBack,
    Stun,
}

public struct FloatingMessageEvent
{
    public int id;
    public FloatingMessageType type;
    //public string message;
    public int amount;
    //public float duration;

    public FloatingMessageEvent(int id,FloatingMessageType type, int amount)
    {
        this.id = id;
        this.type = type;
        //this.message = message;
        this.amount = amount;
        //this.duration = duration;
    }

    static FloatingMessageEvent e;
    public static void Trigger(int id,FloatingMessageType type,int amount)
    {
        e.id = id;
        e.type = type;
        //e.message = message;
        e.amount = amount;
        //e.duration = duration;
        MMEventManager.TriggerEvent(e);
    }
}

public class FloatingMessageController : MonoBehaviour, MMEventListener<FloatingMessageEvent>
{
    private int _id;
    public GameObject[] FloatingMessagePrefabs;

    public Vector3 SpawnOffset;
    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _id = transform.parent.GetComponent<MSB_Character>().UserNum;
    }

    private void ShowFloatingMessage(FloatingMessageType type, int amount)
    {
        GameObject floatingMessage;
        Vector3 randomOffset = RandomOffset();
        if (amount != 0)
        {
            floatingMessage = Instantiate(FloatingMessagePrefabs[(int) type], transform.position + randomOffset,
                Quaternion.identity);
            
            floatingMessage.GetComponent<TextMesh>().text = amount.ToString();
        }
        else
        {
            floatingMessage = Instantiate(FloatingMessagePrefabs[(int) type], transform.position + randomOffset,
                Quaternion.identity);
        }
    }
    private Vector3 RandomOffset()
    {
        Vector3 offset;
        float x = UnityEngine.Random.Range(-0.2f, 0.2f);
        float y = UnityEngine.Random.Range(0.5f, 1.5f);
        float z = 0f;
        
        offset = new Vector3(x, y, z);
        return offset;
    }
    
    public void OnMMEvent(FloatingMessageEvent e)
    {
        if (_id == e.id)
            ShowFloatingMessage(e.type, e.amount);
    }
    
    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }
}
