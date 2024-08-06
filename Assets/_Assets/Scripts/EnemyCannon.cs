using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour {

    public event EventHandler<LaunchEventArgs> OnLaunchStart;
    public event EventHandler OnLaunchEnd;

    public event EventHandler<HealthEventArgs> OnHealthChange;
    public class HealthEventArgs : EventArgs {
        public float healthNormalized;
    }
    public class LaunchEventArgs : EventArgs {
        public Vector3 launchVector;
    }
    [SerializeField] private Transform launchOrigin;
    [SerializeField] private float launchDelay = 0;
    private int maxHealth = 0;
    private int health = 0;
    private float cannonDomainExtent = 0;
    Coroutine launchCoroutine;
    public void LaunchProjectile(Transform enemyProjectile, Vector3 launchVector) {
        if (launchCoroutine == null && health > 0) {
            OnLaunchStart?.Invoke(this, new LaunchEventArgs { launchVector = launchVector });
            launchCoroutine = StartCoroutine(LaunchCoroutine(enemyProjectile, launchVector));
        }
    }

    private IEnumerator LaunchCoroutine(Transform enemyProjectile, Vector3 launchVector) {
        yield return new WaitForSeconds(launchDelay);
        OnLaunchEnd?.Invoke(this, EventArgs.Empty);
        Transform projectile = Instantiate(enemyProjectile, launchOrigin.position, launchOrigin.rotation);
        projectile.GetComponent<Rigidbody>().velocity = launchVector;
        projectile.GetComponent<Rigidbody>().angularVelocity = launchVector;
        launchCoroutine = null;
    }


    public Transform GetLaunchOrigin() {
        return launchOrigin;
    }

    public float GetLaunchDelay() {
        return launchDelay;
    }
    public void SetHealth(int h) {
        maxHealth = h;
        health = maxHealth;
        OnHealthChange?.Invoke(this, new HealthEventArgs { healthNormalized = (float)health / maxHealth });
    }

    public void SetDomainExtent(float extent) {
        cannonDomainExtent = extent;
    }

    public bool IsZCoordinateInDomain(float z) {
        return z <= (transform.position.z + cannonDomainExtent) && z >= (transform.position.z - cannonDomainExtent);
    }
    public float GetHealthNormalized() {
        return (float)health / maxHealth;
    }
    public void DoDamage(int damage) {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChange?.Invoke(this, new HealthEventArgs { healthNormalized = (float)health / maxHealth });
    }

    public bool IsAlive() {
        return health > 0;
    }
    public int GetHealth() {
        return health;
    }

}
