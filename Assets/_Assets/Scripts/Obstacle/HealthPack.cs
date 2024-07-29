using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour, IHittable, IFallingObstacle {
    [SerializeField] private float fallingSpeed = 0.1f;
    [SerializeField] private int healthAmount = 20;

    private Vector3 launchVector;


    public HittableType GetHittableType() {
        return HittableType.Obstacle;
    }

    public void Hit(BaseProjectile projectile) {
        if (projectile.GetHittableType() == HittableType.PlayerProjectile) {
            Player.Instance.AddHealth(healthAmount);
        }
        else if (projectile.GetHittableType() == HittableType.EnemyProjectile) {
            Enemy.Instance.AddHealth(healthAmount);
        }
        DestroyObstacle();
    }



    private void FixedUpdate() {
        transform.position -= new Vector3(0, fallingSpeed * Time.deltaTime, 0);
        if (transform.position.y < -10) {
            DestroyObstacle();
        }
    }

    public int GetDamage() {
        return 0;
    }
    public float GetFallSpeed() {
        return fallingSpeed;
    }

    public void SetFallSpeed(float speed) {
        fallingSpeed = speed;
    }
    public void SetInverted(bool i) {
    }
    public bool IsInverted() {
        return false;
    }
    private void DestroyObstacle() {
        ObstacleSpawner.Instance.RemoveObstacleFromList(this.transform);
        Destroy(gameObject);
    }
}
