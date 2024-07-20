using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour,IHittable
{
    [SerializeField] private float fallingSpeed = 0.1f;
    [SerializeField] private float launchSpeed = 10f;
    [SerializeField] private Transform obstacleProjectile;
    private Vector3 launchVector;

    public HittableType GetHittableType() {
        return HittableType.Obstacle;
    }

    public void Hit(BaseProjectile projectile) {
        if (projectile.GetHittableType() == HittableType.PlayerProjectile) {
            launchVector = transform.position - Player.Instance.transform.position;

        }
        else if (projectile.GetHittableType() == HittableType.EnemyProjectile) {
            launchVector = transform.position - Enemy.Instance.transform.position;
        }
        LaunchProjectile();
        Destroy(gameObject);
    }

    private void LaunchProjectile() {
        Transform projectile = Instantiate(obstacleProjectile, transform.position, Quaternion.identity);
        projectile.GetComponent<ObstacleProjectile>().SetProjectileType(HittableType.Obstacle);
        projectile.GetComponent<Rigidbody>().velocity = launchVector.normalized * launchSpeed;
    }
    private void Start() {
        launchVector = Vector3.zero;
    }
    private void FixedUpdate() {
        transform.position -= new Vector3(0, fallingSpeed*Time.deltaTime, 0);
        if (transform.position.y < -10) { 
            Destroy(gameObject);
        }
    }


}
