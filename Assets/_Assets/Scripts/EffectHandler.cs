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
    [SerializeField] private Transform textEffect;
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

    public void SpawnTextEffect(string text, Vector3 position, TextEffect.TextColor color, float startSize = 1f, float speed = 3f) {
        Transform textEffectSpawned = Instantiate(textEffect, position, Quaternion.identity);
        TextEffect textEffectComponent = textEffectSpawned.GetComponent<TextEffect>();
        textEffectComponent.SetText(text);
        textEffectComponent.SetScale(startSize);
        textEffectComponent.SetSpeed(speed);
        textEffectComponent.SetColor(color);
    }
}
