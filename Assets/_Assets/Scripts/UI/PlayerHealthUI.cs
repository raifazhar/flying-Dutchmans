using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    void Start() {
        Player.Instance.OnHealthChange += Player_OnHealthChange;
    }

    private void Player_OnHealthChange(object sender, System.EventArgs e) {
        healthBar.fillAmount = Player.Instance.GetHealthNormalized();
    }
}
