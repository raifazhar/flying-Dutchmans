using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleProjectile : BaseProjectile {

    public void SetProjectileType(HittableType type) {
        base.projectileType = type;
    }

    override protected void OnCollisionEnter(Collision collision) {
        IHittable hittable = collision.gameObject.GetComponent<IHittable>();
        if (hittable != null) {
            base.HandleHitEffect(hittable, collision);
            //Don't destroy if it hit an obstacle, instead reset velocity to make it not bounce off
            if (hittable.GetHittableType() == HittableType.Obstacle) {
                transform.gameObject.GetComponent<Rigidbody>().velocity = -collision.relativeVelocity;
            }
            else {
                Destroy(gameObject);
            }
        }
        else {
            Destroy(gameObject);
        }
    }
}
