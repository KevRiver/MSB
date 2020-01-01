using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaAnimation : MonoBehaviour
{
    private SpriteRenderer _outlineRenderer;
    private SpriteOutline _spriteOutline;
    private Color _initialColor;
    private Color _color;
    private float _start;
    private float _progress;
    public float _lerpDuration;
    private bool _fadeIn;
    private bool _initialized = false;

    

    private void Start()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        _spriteOutline = transform.GetComponentInParent<SpriteOutline>();
        if (!_spriteOutline)
        {
            Debug.LogWarning("AlphaAnimation.cs : Outline doesn't have <SpriteOutlineOverride>");
            return;
        }
        _outlineRenderer = GetComponent<SpriteRenderer>();
        if (!_outlineRenderer || !_spriteOutline)
        {
            Debug.LogWarning("AlphaAnimation.cs : Outline doesn't have <SpriteRenderer>");
            return;
        }

        _initialColor = _spriteOutline.color;
        _color = _initialColor;
        _fadeIn = true;
        _initialized = true;
    }
    
    private void OnEnable()
    {
        _start = Time.time;
        if (_initialized)
        {
            _color = _initialColor;
            _fadeIn = true;
        }
    }

    private void Update()
    {
        _progress = Time.time - _start;

        if (_fadeIn)
        {
            _color.a = Mathf.Lerp(1.0f, 0.0f, _progress / _lerpDuration);
            _outlineRenderer.material.SetColor("_Color",_color);
            if (_progress / _lerpDuration >= 1)
            {
                _start = Time.time;
                _fadeIn = false;
            }
        }
        else
        {
            _color.a = Mathf.Lerp(0.0f, 1.0f, _progress / _lerpDuration);
            _outlineRenderer.material.SetColor("_Color",_color);
            if (_progress / _lerpDuration >= 1)
            {
                _start = Time.time;
                _fadeIn = true;
            }
        }
    }
}
