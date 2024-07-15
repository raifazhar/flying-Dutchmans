using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Image enemyHealthBar;
    void Start() {
        Enemy.Instance.OnHealthChanged += Enemy_OnHealthChange;
    }

    private void Enemy_OnHealthChange(object sender, System.EventArgs e) {
        enemyHealthBar.fillAmount = Enemy.Instance.GetHealthNormalized();
    }
}
