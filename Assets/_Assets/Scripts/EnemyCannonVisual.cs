using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCannonVisual : MonoBehaviour {

    [SerializeField] private EnemyCannon cannon;
    [SerializeField] private Transform pivotBone;
    [SerializeField] private Transform angleBone;
    [SerializeField] private Transform vfxSpawnPoint;
    [SerializeField] private Transform healthyVisual;
    [SerializeField] private Transform destroyedVisual;
    [SerializeField] private Image healthBar;
    private Vector3 launchVector;
    private Vector3 angleOrigin;
    private Vector3 pivotOrigin;
    private Quaternion pivotTarget;
    private Quaternion angleTarget;
    private float launchLerp = 1f;
    // Start is called before the first frame update
    void Start() {
        launchVector = Vector3.zero;
        cannon.OnLaunchStart += Cannon_OnLaunch;
        cannon.OnLaunchEnd += Cannon_OnLaunchEnd;
        cannon.OnHealthChange += Cannon_OnHealthChange;
        angleTarget = Quaternion.identity;
        pivotTarget = Quaternion.identity;
        angleOrigin = angleBone.eulerAngles;
        pivotOrigin = pivotBone.eulerAngles;
        healthyVisual.gameObject.SetActive(true);
        destroyedVisual.gameObject.SetActive(false);
    }

    private void Cannon_OnHealthChange(object sender, EnemyCannon.HealthEventArgs e) {
        healthBar.fillAmount = e.healthNormalized;
        if (e.healthNormalized <= 0) {
            healthyVisual.gameObject.SetActive(false);
            destroyedVisual.gameObject.SetActive(true);
        }

    }

    private void Cannon_OnLaunchEnd(object sender, System.EventArgs e) {
        EffectHandler.Instance.SpawnEffect(EffectHandler.EffectType.CollisionWhite, vfxSpawnPoint.position);
    }

    private void Cannon_OnLaunch(object sender, EnemyCannon.LaunchEventArgs e) {
        launchVector = e.launchVector;
        launchLerp = 0f;
        Vector3 flatLaunchVector = new Vector3(launchVector.x, 0, launchVector.z);
        pivotTarget = Quaternion.LookRotation(flatLaunchVector, Vector3.up);
        pivotTarget *= Quaternion.Euler(0, 90, 0);
        float elevationAngle = Vector3.Angle(flatLaunchVector, launchVector);
        if (launchVector.y < 0) {
            elevationAngle = -elevationAngle;
        }
        angleTarget = Quaternion.Euler(-elevationAngle, 0, 0);

    }

    private void Update() {
        if (launchLerp < 1) {
            launchLerp += Time.deltaTime / cannon.GetLaunchDelay();
            launchLerp = Mathf.Clamp(launchLerp, 0, 1f);
            angleBone.localRotation = Quaternion.Lerp(angleBone.localRotation, angleTarget, launchLerp);
            pivotBone.localRotation = Quaternion.Lerp(pivotBone.localRotation, pivotTarget, launchLerp);
        }
    }
}
