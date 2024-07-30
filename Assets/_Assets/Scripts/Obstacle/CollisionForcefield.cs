using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CollisionForcefield : MonoBehaviour {
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private AnimationCurve vfxCurve;
    [SerializeField] private float flashDuration;

    private float flashTimer = 0f;
    private void OnCollisionEnter(Collision collision) {
        EffectHandler.Instance.SpawnEffect(EffectHandler.EffectType.ForceFieldHit, collision.contacts[0].point);
        flashTimer = 0f;
    }
    private void Update() {
        if (flashTimer < flashDuration) {
            flashTimer += Time.deltaTime;
            float lerp = flashTimer / flashDuration;
            VFX.SetFloat("opacity", vfxCurve.Evaluate(lerp));
        }
    }

}
