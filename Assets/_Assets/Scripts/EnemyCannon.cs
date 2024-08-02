using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour {

    public event EventHandler<LaunchEventArgs> OnLaunch;
    public class LaunchEventArgs : EventArgs {
        public Vector3 launchVector;
    }
    [SerializeField] private Transform launchOrigin;

    public void LaunchProjectile(Transform enemyProjectile, Vector3 launchVector) {
        Transform projectile = Instantiate(enemyProjectile, launchOrigin.position, launchOrigin.rotation);
        projectile.GetComponent<Rigidbody>().velocity = launchVector;
        projectile.GetComponent<Rigidbody>().angularVelocity = launchVector;
    }



    public Transform GetLaunchOrigin() {
        return launchOrigin;
    }

}
