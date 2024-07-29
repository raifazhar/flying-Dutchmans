using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour {

    public event EventHandler ProjectileInPosition;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform target;
    [SerializeField] private float launchSpeed = 10f;
    [SerializeField] private float threshold = 0.1f;
    private Vector3 velocity;
    private Vector3 angularVelocity;
    private bool isLaunched = false;
    private Transform projectile;

    public void LaunchProjectile() {
        if (!isLaunched) {
            Vector3 launchVector = CalculateLaunchVector(transform.position, target.position, launchSpeed);
            projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            projectile.GetComponent<Rigidbody>().velocity = launchVector;
            projectile.GetComponent<Rigidbody>().angularVelocity = launchVector;
            projectile.GetComponent<DestroyAfterTime>().enabled = false;

            isLaunched = true;
        }
    }

    private void Update() {
        if (isLaunched && Vector3.SqrMagnitude(projectile.transform.position - target.transform.position) < threshold) {
            ProjectileInPosition?.Invoke(this, EventArgs.Empty);
            velocity = projectile.GetComponent<Rigidbody>().velocity;
            angularVelocity = projectile.GetComponent<Rigidbody>().angularVelocity;
            projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projectile.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            projectile.GetComponent<Rigidbody>().useGravity = false;

            isLaunched = false;
        }
    }

    public void ResumeProjectile() {
        projectile.GetComponent<Rigidbody>().useGravity = true;
        projectile.GetComponent<DestroyAfterTime>().enabled = true;
        projectile.GetComponent<Rigidbody>().velocity = velocity;
        projectile.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
    }
    private Vector3 CalculateLaunchVector(Vector3 startPos, Vector3 targetPos, float launchSpeed) {
        Vector3 toTarget = targetPos - startPos;
        // Set up the terms we need to solve the quadratic equations.
        float gSquared = Physics.gravity.sqrMagnitude;

        float b = launchSpeed * launchSpeed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;
        if (discriminant < 0) {
            b = (float)Math.Sqrt(gSquared * toTarget.sqrMagnitude);
            discriminant = 0;
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest shot with the given max speed:
        //float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Most direct shot with the given max speed:
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        // Lowest-speed arc available:
        //float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        float T = T_min;

        // Convert from time-to-hit to a launch velocity:
        return (toTarget / T - Physics.gravity * T / 2f);
    }

}
