using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject hitmarkerSound;
    public GameObject hitmarker;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hit !");
            hitmarkerSound.GetComponent<AudioSource>().Play();
            /* 히트마커 임시 삭제
            Vector3 playerPos = this.gameObject.transform.position;
            Instantiate(hitmarker, playerPos, Quaternion.identity);
            */          


        }
    }
}
