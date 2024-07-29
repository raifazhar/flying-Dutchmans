using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisual : MonoBehaviour {

    [SerializeField] private Transform fullHealth;
    [SerializeField] private Transform destroyed;
    void Start() {
        fullHealth.gameObject.SetActive(true);
        destroyed.gameObject.SetActive(false);
        Enemy.Instance.OnStateChange += Enemy_OnStateChange;
    }

    private void Enemy_OnStateChange(object sender, Enemy.OnStateChangeEventArgs e) {
        if (e.enemyState == Enemy.State.Dead) {
            EnableDestroyedHarbour();
        }
    }

    private void EnableDestroyedHarbour() {
        fullHealth.gameObject.SetActive(false);
        destroyed.gameObject.SetActive(true);
    }
}
