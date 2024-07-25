using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CollisionForcefield : MonoBehaviour
{
    [SerializeField]private Transform hitvfx;
    [SerializeField]private VisualEffect vfx;
    [SerializeField]private AnimationCurve vfxCurve;
    [SerializeField] private float flashduration;

    private float flashtimer=0f;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision);
        Instantiate(hitvfx, collision.GetContact(0).point, Quaternion.identity);
        flashtimer = 0f;
    }
    private void Update()
    {
        if(flashtimer <flashduration) 
        {
            flashtimer += Time.deltaTime;
            float lerp = flashtimer / flashduration;
            vfx.SetFloat("opacity",vfxCurve.Evaluate(lerp));
        }
    }

}
