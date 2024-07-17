using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour, IHittable {

    [SerializeField] protected HittableType[] targets;
    [SerializeField] protected int damage;
    [SerializeField] protected int decayTime;
    private float decayTimer;
    [SerializeField] protected HittableType projectileType;

    public virtual int GetDamage() {
        return damage;
    }

    public virtual void Hit(BaseProjectile projectile) {
        Destroy(gameObject);
    }

    public virtual HittableType GetHittableType() {
        return projectileType;
    }

    protected bool CanHit(HittableType type) {
        bool canHit = false;
        foreach (var target in targets) {
            if (target == type) {
                canHit = true;
                break;
            }
        }
        return canHit;
    }
    protected virtual void OnCollisionEnter(Collision collision) {
        IHittable hittable = collision.gameObject.GetComponent<IHittable>();
        if (hittable != null) {
            if (CanHit(hittable.GetHittableType())) {
                hittable.Hit(this);
                //Projectile should always get destroyed after collision with anything it can hit
                Destroy(gameObject);
            }
        }
    }

    protected virtual void Start() {
        decayTimer = decayTime;
    }
    protected virtual void Update() {
        decayTimer -= Time.deltaTime;
        if (decayTimer <= 0) {
            Destroy(gameObject);
        }
    }

}
