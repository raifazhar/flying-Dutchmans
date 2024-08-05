using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour {

    public event EventHandler<LaunchEventArgs> OnLaunchStart;
    public event EventHandler OnLaunchEnd;
    public class LaunchEventArgs : EventArgs {
        public Vector3 launchVector;
    }
    [SerializeField] private Transform launchOrigin;
    [SerializeField] private float launchDelay = 0;
    Coroutine launchCoroutine;
    public void LaunchProjectile(Transform enemyProjectile, Vector3 launchVector) {
        if (launchCoroutine == null) {
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

}
