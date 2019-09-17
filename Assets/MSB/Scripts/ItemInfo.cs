using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public int itemID;

    private void Start()
    {
        gameObject.name += " " + itemID.ToString();
    }
}
