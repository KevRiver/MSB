using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Color OpponentOutlineColor;
    public Color AllyOutlineColor;
    private MSB_Character _character;
    private MSB_GameManager.Team _team;
    private MSB_GameManager.Team _localPlayerTeam;
    private bool _characterIsRemote;
    private Transform _model;
    private SpriteRenderer _outlineRenderer;
    private Transform _outline;
    public void Initialization(MSB_GameManager.Team localPlayerTeam)
    {
        _character = GetComponent<MSB_Character>();
        if (!_character)
        {
            Debug.LogWarning("OutlineController.cs : can't find msb character");
            return;
        }

        _characterIsRemote = _character.IsRemote;
        _team = _character.team;
        _model = transform.GetChild(0);
        _outline = _model.GetChild(1);
        _outlineRenderer = _outline.GetComponentInChildren<SpriteRenderer>();
        if (!_outlineRenderer)
        {
            Debug.LogWarning("OutlineController.cs : SpriteOutline not initialized");
            return;
        }

        if (!_characterIsRemote)
        {
            _outlineRenderer.material.SetColor("_Color",Color.yellow);
        }
        else if(_team == localPlayerTeam)
            _outlineRenderer.material.SetColor("_Color", AllyOutlineColor);
        else
            _outlineRenderer.material.SetColor("_Color",OpponentOutlineColor);
    }
}
