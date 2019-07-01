using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public float maxHp;
    public float currentHp;
    public Slider healthSlider;

    private void Start()
    {
        maxHp = gameObject.GetComponent<PlayerData>().hp;
        currentHp = maxHp;
    }

    public void TakeDamage(float _dmg)
    {
        currentHp -= _dmg;
        healthSlider.value = currentHp;
    }
}
