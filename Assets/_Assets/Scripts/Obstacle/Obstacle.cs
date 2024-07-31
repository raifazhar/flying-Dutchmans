using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IHittable, IFallingObstacle {
    [SerializeField] private float fallingSpeed = 0.1f;
    [SerializeField] private float launchSpeed = 10f;
    [SerializeField] private Transform obstacleProjectile;
    [SerializeField] private bool isInverted = false;
    [SerializeField] private Color invertedColor;
    [SerializeField] private MeshRenderer meshRenderer;
    private bool isTargetingEnemy = false;

    private Vector3 launchVector;


    public HittableType GetHittableType() {
        return HittableType.Obstacle;
    }

    public void Hit(BaseProjectile projectile, Collision collision) {
        Vector3 targetPosition = Vector3.zero;
        if (!isInverted) {
            if (projectile.GetHittableType() == HittableType.PlayerProjectile) {
                targetPosition = GetClosestEnemyPosition(transform.position);
                isTargetingEnemy = true;
            }
            else if (projectile.GetHittableType() == HittableType.EnemyProjectile) {
                targetPosition = Player.Instance.transform.position;
            }
        }
        else {
            if (projectile.GetHittableType() == HittableType.PlayerProjectile) {
                targetPosition = Player.Instance.transform.position;

            }
            else if (projectile.GetHittableType() == HittableType.EnemyProjectile) {
                targetPosition = GetClosestEnemyPosition(transform.position);
                isTargetingEnemy = true;
            }

        }
        launchVector = CalculateLaunchVector(transform.position, targetPosition, launchSpeed);
        LaunchProjectile();
        DestroyObstacle();
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
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Most direct shot with the given max speed:
        //float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        // Lowest-speed arc available:
        //float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        float T = T_max;

        // Convert from time-to-hit to a launch velocity:
        return (toTarget / T - Physics.gravity * T / 2f);
    }

    private Vector3 GetClosestEnemyPosition(Vector3 origin) {
        //Use the origin to get the closest position within the bounds of the enemy
        Bounds enemyBounds = Enemy.Instance.GetBounds();
        Vector3 closestPosition = origin;
        closestPosition.x = Mathf.Clamp(closestPosition.x, enemyBounds.min.x, enemyBounds.max.x);
        closestPosition.y = Mathf.Clamp(closestPosition.y, enemyBounds.min.y, enemyBounds.max.y);
        closestPosition.z = Mathf.Clamp(closestPosition.z, enemyBounds.min.z, enemyBounds.max.z);
        return closestPosition;
    }

    private void LaunchProjectile() {
        Transform projectile = Instantiate(obstacleProjectile, transform.position, Quaternion.identity);
        if (isTargetingEnemy)
            projectile.GetComponent<ObstacleProjectile>().SetProjectileType(HittableType.PlayerProjectile);
        else
            projectile.GetComponent<ObstacleProjectile>().SetProjectileType(HittableType.EnemyProjectile);
        projectile.GetComponent<Rigidbody>().velocity = launchVector;
        projectile.GetComponent<Rigidbody>().angularVelocity = launchVector;
    }
    private void Start() {
        launchVector = Vector3.zero;
        if (isInverted) {
            meshRenderer.material.color = invertedColor;
        }
    }
    private void FixedUpdate() {
        transform.position -= new Vector3(0, fallingSpeed * Time.deltaTime, 0);
        if (transform.position.y < -10) {
            DestroyObstacle();
        }
    }

    public int GetDamage() {
        return obstacleProjectile.GetComponent<BaseProjectile>().GetDamage();
    }
    public float GetFallSpeed() {
        return fallingSpeed;
    }

    public void SetFallSpeed(float speed) {
        fallingSpeed = speed;
    }
    public void SetInverted(bool i) {
        isInverted = i;
    }
    public bool IsInverted() {
        return isInverted;
    }
    private void DestroyObstacle() {
        ObstacleSpawner.Instance.RemoveObstacleFromList(this.transform);
        Destroy(gameObject);
    }

}
