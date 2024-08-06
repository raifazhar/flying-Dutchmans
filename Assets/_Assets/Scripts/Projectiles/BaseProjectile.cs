using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;


public abstract class BaseProjectile : MonoBehaviour, IHittable {

    [SerializeField] protected EffectHandler.EffectType hitEffectType;
    [SerializeField] protected HittableType[] targets;
    [SerializeField] protected int damage;
    [SerializeField] protected HittableType projectileType;

    public virtual int GetDamage() {
        return damage;
    }

    public virtual void Hit(BaseProjectile projectile, Collision collision) {
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
                hittable.Hit(this, collision);
                EffectHandler.Instance.SpawnEffect(hitEffectType, collision.contacts[0].point);

            }
            else if (hittable.GetHittableType() == HittableType.Ocean) {
                EffectHandler.Instance.SpawnEffect(EffectHandler.EffectType.WaterSplash, collision.contacts[0].point);
            }
            if (hittable.GetHittableType() == HittableType.Obstacle && collision.gameObject.GetComponent<Obstacle>() != null) {
                EffectHandler.Instance.SpawnEffect(EffectHandler.EffectType.WoodCrack, collision.contacts[0].point);
            }
            if (hitEffectType == EffectHandler.EffectType.BombExplosion) {
                SoundManager.Playsound(SoundManager.Sound.Explosion, transform.position);
            }
            else {
                SoundManager.Playsound(SoundManager.Sound.Collision, transform.position);
            }
        }
        Destroy(gameObject);
    }




}
