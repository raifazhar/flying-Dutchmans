using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour {

    public enum EffectType {
        BombExplosion,
        CollisionRed,
        CollisionWhite,
        ForceFieldHit,
        WaterSplash,
        WoodCrack,
    }
    public static EffectHandler Instance { get; private set; }
    [SerializeField] private Transform bombExplosionEffect;
    [SerializeField] private Transform collisionRedEffect;
    [SerializeField] private Transform collisionWhiteEffect;
    [SerializeField] private Transform forceFieldHitEffect;
    [SerializeField] private Transform waterSplashParticles;
    [SerializeField] private Transform woodCrackEffect;
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void SpawnEffect(EffectType effect, Vector3 position) {
        switch (effect) {
            case EffectType.BombExplosion:
                Instantiate(bombExplosionEffect, position, Quaternion.identity);
                break;
            case EffectType.CollisionRed:
                Instantiate(collisionRedEffect, position, Quaternion.identity);
                break;
            case EffectType.CollisionWhite:
                Instantiate(collisionWhiteEffect, position, Quaternion.identity);
                break;
            case EffectType.ForceFieldHit:
                Instantiate(forceFieldHitEffect, position, Quaternion.identity);
                break;
            case EffectType.WaterSplash:
                Instantiate(waterSplashParticles, position, Quaternion.identity);
                break;
            case EffectType.WoodCrack:
                Instantiate(woodCrackEffect, position, Quaternion.identity);
                break;
            default:
                break;
        }

    }
}
