using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleProjectile : BaseProjectile {

    public void SetProjectileType(HittableType type) {
        base.projectileType = type;
    }
}
