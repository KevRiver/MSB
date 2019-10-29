using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Color OpponentOutlineColor;
    public Color AllyOutlineColor;
    private bool _characterIsRemote;
    private Transform _model;
    private SpriteOutline _spriteOutline;
    void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _characterIsRemote = GetComponent<MSB_Character>().IsRemote;
        _model = transform.GetChild(0);
        _spriteOutline = _model.GetComponent<SpriteOutline>();
        if(_spriteOutline == null)
            Debug.LogWarning("SpriteOutline not initialized");

        /*if (_characterIsRemote)
            _spriteOutline.color = OpponentOutlineColor;
        else
        {
            _spriteOutline.color = AllyOutlineColor;
        }*/
    }
}
