using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachWeapon : MonoBehaviour
{
    public GameObject swordPrefab;
    Vector3 attachPosition;
    Quaternion rotation;

    void Start()
    {
        attachPosition = swordPrefab.GetComponent<Sword>().attachPosition;
        rotation = swordPrefab.GetComponent<Sword>().rotation;
        swordPrefab = Instantiate(swordPrefab, new Vector3(0,0,0), rotation);
        swordPrefab.transform.parent = gameObject.transform;
        //swordPrefab.transform.localRotation.Set(0, 0, 20, 0);
        swordPrefab.transform.localPosition = new Vector3(0f, 4.2f, 1f);
        //swordPrefab.transform.localPosition.Set(0, 0, 1);
        //Debug.Log(attachPosition);
        //Debug.Log(quaternion);
    }
}
