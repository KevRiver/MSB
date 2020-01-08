using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTest : MonoBehaviour
{
    public MSB_Character Character;
    // Start is called before the first frame update
    void Start()
    {
        Character = FindObjectOfType<MSB_Character>();
        Character.AbilityControl(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
