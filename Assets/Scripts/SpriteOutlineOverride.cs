using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOutlineOverride : SpriteOutline
{
    private MSB_Character _character;
    private MSB_GameManager.Team _team;
    private MSB_GameManager.Team _localPlayerTeam;
    // Start is called before the first frame update
    protected override void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            if (generatesOnValidate) {
                Regenerate ();
            }

            return;
        }
#endif
        _character = transform.GetComponentInParent<MSB_Character>();
        if (!_character)
            return;
        _localPlayerTeam = MSB_LevelManager.Instance.TargetPlayer.team;
        _team = _character.team;
        if (!generatesOnStart)
            return;

        Regenerate ();
    }

    public override void Regenerate()
    {
        base.Regenerate();

        if (Application.isPlaying)
        {
            Color color = Color.white;
            if (!_character.IsRemote)
                color = Color.yellow;
            else if (_team == _localPlayerTeam)
                color = Color.green;
            else
            {
                color = Color.red;
            }

            material.SetColor("_Color", color);
        }
    }
}
