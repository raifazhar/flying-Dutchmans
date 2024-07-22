using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance { get; private set; }
    [SerializeField] private float displacementMultiplier;
    [SerializeField] private float maxDisplacement;
    [SerializeField] private float displacementSpeed;
    [SerializeField] private Transform shakeEmpty;
    [SerializeField] private float maxYaw;
    [SerializeField] private float maxPitch;
    [SerializeField] private float maxRoll;
    [SerializeField] private float maxOffsetX;
    [SerializeField] private float maxOffsetY;
    [SerializeField] private int noiseSeed;
    [SerializeField] private float traumaDecreaseFactor;
    [SerializeField] private float noiseFrequency;
    [SerializeField] private float shakeSnapSpeed;
    [SerializeField] private float shakeMagnitude;
    private float trauma = 0f;
    private float shake;
    private float yaw;
    private float pitch;
    private float roll;
    private float shakeOffsetX;
    private float shakeOffsetY;
    private Vector3 playerOffset;
    private Vector3 offsetVector;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one CameraController in the scene");
        }
        else {
            Instance = this;
        }
    }


    // Update is called once per frame
    private void LateUpdate() {
        HandleCameraShake();
        HandleCameraMove();
    }

    private void HandleCameraMove() {
        float backwardsDisplacement = Player.Instance.GetScreenVector().magnitude;
        backwardsDisplacement = Mathf.Clamp(backwardsDisplacement, 0, maxDisplacement);
        Vector3 targetPosition = new Vector3(0, 0, -backwardsDisplacement * displacementMultiplier);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, displacementSpeed * Time.deltaTime);
    }
    private void HandleCameraShake() {
        shake = trauma * trauma;
        CameraShakeRotation();
        CameraShakeLocation();
        trauma -= traumaDecreaseFactor * Time.deltaTime;
        trauma = Mathf.Clamp(trauma, 0, 1);
    }
    private void CameraShakeRotation() {
        yaw = maxYaw * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed, Time.realtimeSinceStartup * noiseFrequency));
        pitch = maxPitch * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 1, Time.realtimeSinceStartup * noiseFrequency));
        roll = maxRoll * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 2, Time.realtimeSinceStartup * noiseFrequency));
        Vector3 shakeRotationVector = new Vector3(pitch, yaw, roll);
        shakeRotationVector *= shakeMagnitude;
        Quaternion shakeRotation = Quaternion.Euler(shakeRotationVector);

        shakeEmpty.localRotation = Quaternion.Lerp(shakeEmpty.localRotation, shakeRotation, Time.fixedDeltaTime * shakeSnapSpeed);
    }
    private void CameraShakeLocation() {
        shakeOffsetX = Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 3, Time.realtimeSinceStartup * noiseFrequency));
        shakeOffsetY = Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 4, Time.realtimeSinceStartup * noiseFrequency));
        offsetVector = new Vector3(shakeOffsetX, shakeOffsetY, shakeEmpty.localPosition.z);
        offsetVector = offsetVector.normalized;
        offsetVector *= shake;
        offsetVector.x *= maxOffsetX;
        offsetVector.y *= maxOffsetY;
        offsetVector *= shakeMagnitude;
        shakeEmpty.localPosition = Vector3.Lerp(shakeEmpty.localPosition, offsetVector, Time.deltaTime * shakeSnapSpeed);
    }

    public void AddTrauma(float t) {

        trauma += t;
    }

}
