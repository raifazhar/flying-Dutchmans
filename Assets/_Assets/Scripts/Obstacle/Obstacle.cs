using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IHittable {
    [SerializeField] private float fallingSpeed = 0.1f;
    [SerializeField] private float launchSpeed = 10f;
    [SerializeField] private float upwardsAngle = 10f;
    [SerializeField] private Transform obstacleProjectile;
    private Vector3 launchVector;

    public HittableType GetHittableType() {
        return HittableType.Obstacle;
    }

    public void Hit(BaseProjectile projectile) {
        Vector3 targetPosition = Vector3.zero;
        if (projectile.GetHittableType() == HittableType.PlayerProjectile) {
            launchVector = Enemy.Instance.transform.position - transform.position;
        }
        else if (projectile.GetHittableType() == HittableType.EnemyProjectile) {
            launchVector = Player.Instance.transform.position - transform.position;
        }
        launchVector.y += upwardsAngle;
        LaunchProjectile();
        DestroyObstacle();
    }

    private void LaunchProjectile() {
        Transform projectile = Instantiate(obstacleProjectile, transform.position, Quaternion.identity);
        projectile.GetComponent<ObstacleProjectile>().SetProjectileType(HittableType.Obstacle);
        projectile.GetComponent<Rigidbody>().velocity = launchVector * launchSpeed;
        projectile.GetComponent<Rigidbody>().angularVelocity = launchVector * launchSpeed;
    }
    private void Start() {
        launchVector = Vector3.zero;
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
    private void DestroyObstacle() {
        ObstacleSpawner.Instance.RemoveObstacleFromList(this.transform);
        Destroy(gameObject);
    }

}
