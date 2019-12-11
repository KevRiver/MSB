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
    Stun
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
    //public Color DamageColor;
    //public Color HealColor;
    //public Color TextColor0;
    //public Color TextColor1;
    public GameObject[] FloatingMessagePrefabs;
    //private MMObjectPooler _objectPooler;

    public Vector3 SpawnOffset;
    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        /*if (GetComponent<MMSimpleObjectPooler>() != null)
        {
            _objectPooler = GetComponent<MMSimpleObjectPooler>();
        }*/
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

    /*protected virtual GameObject SpawnFloatingMessage(FloatingMessageType msgType, string msg, float duration)
    {
        Debug.LogWarning("SpawnFloatingMessage Called");
        GameObject nextGameObject = _objectPooler.GetPooledGameObject();

        // mandatory checks
        if (nextGameObject==null)	{ return null; }
        if (nextGameObject.GetComponent<MMPoolableObject>()==null)
        {
            throw new Exception(gameObject.name+" is trying to spawn objects that don't have a PoolableObject component.");		
        }

        FloatingMessage message = nextGameObject.GetComponent<FloatingMessage>();
        if (message != null)
        {
            message.Duration = duration;
        }

        TextMesh mesh = message.GetComponent<TextMesh>();
        if (mesh != null)
        {
            mesh.text = msg;
            switch (msgType)
            {
                case FloatingMessageType.Damage:
                    mesh.color = DamageColor;
                    message.SetAnimation(FloatingMessageStartAnimation.PopSlideUp,FloatingMessageDestroyAnimation.FadeOut);
                    SpawnOffset = RandomOffset();
                    break;
                
                case FloatingMessageType.Heal:
                    mesh.color = HealColor;
                    message.SetAnimation(FloatingMessageStartAnimation.PopSlideUp,FloatingMessageDestroyAnimation.FadeOut);
                    SpawnOffset = RandomOffset();
                    break;
                
                case FloatingMessageType.KnockBack:
                    mesh.color = TextColor0;
                    message.SetAnimation(FloatingMessageStartAnimation.Pop,FloatingMessageDestroyAnimation.PopOut);
                    SpawnOffset = Vector3.zero;
                    break;
                
                case FloatingMessageType.Stun:
                    mesh.color = TextColor1;
                    message.SetAnimation(FloatingMessageStartAnimation.Pop,FloatingMessageDestroyAnimation.PopOut);
                    SpawnOffset = Vector3.zero;
                    break;
            }
        }

        nextGameObject.transform.position = this.transform.position + SpawnOffset;
        nextGameObject.gameObject.SetActive(true);
        Debug.LogWarning("FloatingMessage Activate");
        
        return nextGameObject;
    }*/
    
    private Vector3 RandomOffset()
    {
        Vector3 offset;
        float x = UnityEngine.Random.Range(-0.1f, 0.1f);
        float y = UnityEngine.Random.Range(0.1f, 0.2f);
        float z = 0f;
        
        offset = new Vector3(x, y, z);
        return offset;
    }
    
    public void OnMMEvent(FloatingMessageEvent e)
    {
        Debug.LogWarning("FloatingMessageEvent called");
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
