using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickButton : MonoBehaviour
{
    public AudioClip Sound;

    private Button _button
    {
        get { return GetComponent<Button>(); }
    }

    private AudioSource _source
    {
        get { return GetComponent<AudioSource>(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        _source.clip = Sound;
        _source.playOnAwake = false;
        
        _button.onClick.AddListener(()=>PlaySound());
    }

    void PlaySound()
    {
        _source.PlayOneShot(Sound);
    }
}
