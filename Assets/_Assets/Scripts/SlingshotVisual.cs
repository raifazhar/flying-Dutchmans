using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotVisual : MonoBehaviour {

    private enum State {
        Idle,
        Aiming,
        Recoil,
    }

    [SerializeField] private Transform slingShotHolder;
    [SerializeField] private Transform slingBone;
    [SerializeField] private float holderMultiplier = 1f;
    [SerializeField] private float rotationMultiplier = 1f;
    [SerializeField] private float tiltMultiplier = 1f;
    private Vector3 screenVector;
    [SerializeField] private float recoilMultiplier = 1f;
    [SerializeField] private float recoilSpeed = 1f;
    [SerializeField] private float rotateToOriginSpeed = 1f;
    [SerializeField] private AnimationCurve recoilCurve;
    private Vector3 slingshotOrigPosition;
    private Vector3 recoilPosition;
    private Vector3 shootPosition;
    private Quaternion recoilRotation;
    private Quaternion shootRotation;

    private float lerpPoint = 0f;
    private float recoilLerp = 0f;
    private State state;
    private void Start() {
        slingshotOrigPosition = slingShotHolder.localPosition;
        Player.Instance.OnStateChange += Player_OnStateChange;
        state = State.Idle;
        screenVector = Vector3.zero;
    }

    private void Player_OnStateChange(object sender, Player.OnStateChangeEventArgs e) {
        if (e.playerState == Player.State.Aiming) {
            state = State.Aiming;
        }
        else if (e.playerState == Player.State.Launching) {
            state = State.Recoil;
            CalculateRecoil();
        }
    }

    private void Update() {
        switch (state) {
            case State.Idle:
                slingShotHolder.localPosition = Vector3.Lerp(slingShotHolder.localPosition, slingshotOrigPosition, Time.deltaTime * rotateToOriginSpeed);
                slingBone.localRotation = Quaternion.Lerp(slingBone.localRotation, Quaternion.identity, Time.deltaTime * rotateToOriginSpeed);
                break;
            case State.Aiming:
                HandleAiming();
                break;
            case State.Recoil:
                HandleRecoil();
                break;
        }
    }

    private void HandleAiming() {
        screenVector = Player.Instance.GetScreenVector();
        float magnitude = screenVector.magnitude;
        float tilt = screenVector.z;
        tilt *= tiltMultiplier;
        magnitude *= holderMultiplier;
        slingShotHolder.localPosition = new Vector3(slingshotOrigPosition.x - magnitude, slingshotOrigPosition.y, slingshotOrigPosition.z);
        slingBone.localRotation = Quaternion.Euler(0, tilt, magnitude * rotationMultiplier);

    }

    private void CalculateRecoil() {
        float magnitude = screenVector.magnitude;
        float tilt = screenVector.z;
        tilt *= tiltMultiplier;
        magnitude *= holderMultiplier;
        recoilPosition = new Vector3(slingshotOrigPosition.x + magnitude * recoilMultiplier, slingshotOrigPosition.y, slingshotOrigPosition.z);
        shootPosition = slingShotHolder.localPosition;
        recoilRotation = Quaternion.Euler(0, tilt, -magnitude * rotationMultiplier);
        shootRotation = slingBone.localRotation;
        recoilLerp = 0f;
    }
    private void HandleRecoil() {
        recoilLerp += Time.deltaTime * recoilSpeed;
        if (recoilLerp > 1f) {
            state = State.Idle;
            recoilLerp = 1f;
        }
        //lerpPoint is found through an oscillating sine wave with decreasing amplitude over time
        lerpPoint = recoilCurve.Evaluate(recoilLerp);
        slingShotHolder.localPosition = Vector3.Lerp(shootPosition, recoilPosition, lerpPoint);
        slingBone.localRotation = Quaternion.Lerp(shootRotation, recoilRotation, lerpPoint);
    }
}
