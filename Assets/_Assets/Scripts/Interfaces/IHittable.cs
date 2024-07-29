using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HittableType {
    Player,
    Enemy,
    PlayerProjectile,
    EnemyProjectile,
    Obstacle,
    Ocean
}
public interface IHittable {
    void Hit(BaseProjectile projectile, Collision collision);

    HittableType GetHittableType();
}
