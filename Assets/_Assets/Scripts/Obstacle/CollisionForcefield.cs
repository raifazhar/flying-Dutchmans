using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CollisionForcefield : MonoBehaviour {
    [SerializeField] private Transform hitVFX;
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private AnimationCurve vfxCurve;
    [SerializeField] private float flashDuration;

    private float flashTimer = 0f;
    private void OnCollisionEnter(Collision collision) {
        Instantiate(hitVFX, collision.GetContact(0).point, Quaternion.identity);
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
