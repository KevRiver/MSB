using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAlphaAnimation : MonoBehaviour
{
    public Image _image;
    private Color _initColor;
    private Color _color;
    private float _progress;
    private float _start;
    private bool _fadeIn;
    public float a;
    public float b;
    public float _lerpDuration;

    private void OnEnable()
    {
        Debug.LogWarning("ImageAlphaAnim OnEnable");
        Initialization();
    }

    void Initialization()
    {
        if(!_image)
            Debug.LogWarning("ImageAlphaAnim:: Image is null");
        _initColor = _image.color;
        _color = _initColor;
        _start = Time.time;
        _fadeIn = false;
    }
    
    private void Update()
    {
        _progress = Time.time - _start;

        if (_fadeIn)
        {
            _color.a = Mathf.Lerp(b, a, _progress / _lerpDuration);
            _image.color = _color;
            if (_progress / _lerpDuration >= 1)
            {
                _start = Time.time;
                _fadeIn = false;
            }
        }
        else
        {
            _color.a = Mathf.Lerp(a, b, _progress / _lerpDuration);
            _image.color = _color;
            if (_progress / _lerpDuration >= 1)
            {
                _start = Time.time;
                _fadeIn = true;
            }
        }
    }

}
