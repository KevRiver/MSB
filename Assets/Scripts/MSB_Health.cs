using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class MSB_Health : Health
{
    protected override void UpdateHealthBar(bool show)
    {
        if (_healthBar != null)
        {
            _healthBar.UpdateBar(CurrentHealth, 0f, MaximumHealth, show);
        }

        /*if (_character != null)
        {
            if (_character.CharacterType == Character.CharacterTypes.Player)
            {
                // We update the health bar
                if (GUIManager.Instance != null)
                {
                    GUIManager.Instance.UpdateHealthBar(CurrentHealth, 0f, MaximumHealth, _character.PlayerID);
                }
            }
        }*/
    }
}
