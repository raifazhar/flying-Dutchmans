using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance { get; private set; }
    [SerializeField]
    private AudioSource mainMusic;
    [SerializeField] private float audioFadeOutTime = 1f;
    private Coroutine audioStopCoroutine;
    [SerializeField] private float displacementMultiplier;
    [SerializeField] private float maxDisplacement;
    [SerializeField] private float displacementSpeed;
    [SerializeField] private Transform shakeEmpty;
    [SerializeField] private float maxYaw;
    [SerializeField] private float maxPitch;
    [SerializeField] private float maxRoll;
    [SerializeField] private float maxOffsetX;
    [SerializeField] private float maxOffsetY;
    [SerializeField] private float maxHardYaw;
    [SerializeField] private float maxHardPitch;
    [SerializeField] private float maxHardRoll;
    [SerializeField] private float maxHardOffsetX;
    [SerializeField] private float maxHardOffsetY;
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
    private bool isPaused = false;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one CameraController in the scene");
        }
        else {
            Instance = this;
        }
    }

    private void Start() {
        GameManager.Instance.OnTogglePause += GameManager_OnTogglePause;
    }

    private void GameManager_OnTogglePause(object sender, GameManager.OnTogglePauseEventArgs e) {
        isPaused = e.isPaused;
    }

    // Update is called once per frame
    private void LateUpdate() {
        if (isPaused) return;
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
        yaw = maxYaw * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed, Time.time * noiseFrequency));
        pitch = maxPitch * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 1, Time.time * noiseFrequency));
        roll = maxRoll * shake * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 2, Time.time * noiseFrequency));
        Vector3 shakeRotationVector = new Vector3(pitch, yaw, roll);
        shakeRotationVector *= shakeMagnitude;
        shakeRotationVector.x = Mathf.Clamp(shakeRotationVector.x, -maxHardPitch, maxHardPitch);
        shakeRotationVector.y = Mathf.Clamp(shakeRotationVector.y, -maxHardYaw, maxHardYaw);
        shakeRotationVector.z = Mathf.Clamp(shakeRotationVector.z, -maxHardRoll, maxHardRoll);
        Quaternion shakeRotation = Quaternion.Euler(shakeRotationVector);
        shakeEmpty.localRotation = Quaternion.Lerp(shakeEmpty.localRotation, shakeRotation, Time.fixedDeltaTime * shakeSnapSpeed);
    }
    private void CameraShakeLocation() {
        shakeOffsetX = Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 3, Time.time * noiseFrequency));
        shakeOffsetY = Mathf.Lerp(-1, 1, Mathf.PerlinNoise(noiseSeed + 4, Time.time * noiseFrequency));
        offsetVector = new Vector3(shakeOffsetX, shakeOffsetY, shakeEmpty.localPosition.z);
        offsetVector = offsetVector.normalized;
        offsetVector *= shake;
        offsetVector.x *= maxOffsetX;
        offsetVector.y *= maxOffsetY;
        offsetVector *= shakeMagnitude;
        offsetVector.x = Mathf.Clamp(offsetVector.x, -maxHardOffsetX, maxHardOffsetX);
        offsetVector.y = Mathf.Clamp(offsetVector.y, -maxHardOffsetY, maxHardOffsetY);
        shakeEmpty.localPosition = Vector3.Lerp(shakeEmpty.localPosition, offsetVector, Time.deltaTime * shakeSnapSpeed);
    }

    public void AddTrauma(float t) {
        trauma += t;
    }


    public void EnableAudio() {
        mainMusic.Play();
    }

    public void DisableAudio() {
        if (audioStopCoroutine == null) {
            audioStopCoroutine = StartCoroutine(AudioFadeOut(audioFadeOutTime));
        }
    }
    IEnumerator AudioFadeOut(float time) {
        float startVolume = mainMusic.volume;

        while (mainMusic.volume > 0) {
            mainMusic.volume -= startVolume * Time.deltaTime / time;

            yield return null;
        }

        mainMusic.Stop();
        mainMusic.volume = startVolume;
        audioStopCoroutine = null;
    }


}
