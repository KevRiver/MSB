using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAxis : MonoBehaviour
{
    public bool animPlayToggle;
    private GameObject weapon;
    // Start is called before the first frame update
    void Start()
    {
        animPlayToggle = false;
        weapon = gameObject.transform.Find("Sword(Clone)").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (animPlayToggle == true)
        {
            weapon.GetComponent<Animation>().Play();
            animPlayToggle = false;
        }
    }
}
