using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour, IHittable {
    public HittableType GetHittableType() {
        return HittableType.Ocean;
    }

    public void Hit(BaseProjectile projectile, Collision collision) {
    }
}
