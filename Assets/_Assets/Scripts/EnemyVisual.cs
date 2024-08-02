using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisual : MonoBehaviour {

    [SerializeField] private Transform fullHealth;
    [SerializeField] private Transform destroyed;
    [SerializeField] private MeshRenderer[] renderers;
    [SerializeField] private Transform hitVisual;
    private readonly string DAMAGE_ATTRIBUTE = "_Damage";
    private float damageMax = 0.4f;
    void Start() {
        fullHealth.gameObject.SetActive(true);
        destroyed.gameObject.SetActive(false);
        Enemy.Instance.OnStateChange += Enemy_OnStateChange;
        Enemy.Instance.OnHealthChanged += Enemy_OnHealthChanged;
        Enemy.Instance.OnHit += Enemy_OnHit;
    }

    private void Enemy_OnHit(object sender, Enemy.OnHitArgs e) {
        Transform effect = Instantiate(hitVisual, e.collision.GetContact(0).point, Quaternion.FromToRotation(Vector3.up, e.collision.GetContact(0).normal));
        effect.SetParent(this.transform);
    }

    private void Enemy_OnHealthChanged(object sender, System.EventArgs e) {
        float damage = (1f - Enemy.Instance.GetHealthNormalized()) * damageMax;
        foreach (MeshRenderer renderer in renderers) {
            renderer.material.SetFloat(DAMAGE_ATTRIBUTE, damage);
        }
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
