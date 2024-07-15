using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseProjectile, IHittable {
    [SerializeField] private int damage;

    public override int GetDamage() {
        return damage;
    }

    public void Hit(BaseProjectile projectile) {
        if (projectile.GetOwner() == ProjectileOwner.Player)
            return;
    }

    public HittableType GetHittableType() {
        return HittableType.PlayerProjectile;
    }

    private void OnCollisionEnter(Collision collision) {
        IHittable hittable = collision.gameObject.GetComponent<IHittable>();
        if (hittable != null && hittable.GetHittableType() != HittableType.Player) {
            //If collision object is a hittable object
            if(collision.gameObject.TryGetComponent<BaseProjectile>(out BaseProjectile projectile))
            {
                //If collision object is another projectile
                if(projectile.GetOwner() == ProjectileOwner.Player)
                {
                    //If the projectile is from the player, do nothing
                    return;
                }

            }
            //If the collision object is not a projectile and is not player
            hittable.Hit(this);
        }
    }
}
