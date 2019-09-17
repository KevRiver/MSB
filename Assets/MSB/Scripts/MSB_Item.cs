using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSB_Item : MonoBehaviour
{
    public int itemID;
    //public Collider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.name += " " + itemID.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D _collider)
    {
        if (_collider.GetComponent<MSB_Character>() == null)
        {
            return;
        }

        MSB_Character _colliderPlayer = _collider.GetComponent<MSB_Character>();
        int userNumber = _colliderPlayer.c_userData.userNumber;
        Debug.Log("MSB_Item OnTriggerEnter");
        MSBNetwork.NetworkModule.GetInstance().RequestGameUserActionItem(MSB_GameManager.Instance.roomIndex, itemID, userNumber);
    }
}
