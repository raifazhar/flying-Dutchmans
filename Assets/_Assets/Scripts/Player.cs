using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IHittable {


    public static Player Instance { get; private set; }
    public enum State {
        Idle,
        Aiming,
        Launching,
        Dead,
    }
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public class OnStateChangeEventArgs : EventArgs {
        public State playerState;
    }
    public event EventHandler OnHealthChange;
    
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
     private float health;


    [Header("Player Settings")]
    [SerializeField] private Transform launchOrigin;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float touchSensibility = 1f;
    [SerializeField] private float launchVectorMax;
    [SerializeField] private float launchVectorMin;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float maxLaunchAngle;
    [SerializeField] private float minLaunchAngle;
    private Vector2 touchOrigin;
    private Vector2 touchOriginWorldSpace;
    private Vector2 touchPoint;
    private Vector2 touchPointWorldSpace;
    private Vector3 launchVector;
    private State playerState;
    // Start is called before the first frame update

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        playerState = State.Idle;
        health= maxHealth;
    }

    // Update is called once per frame
    void Update() {
        switch (playerState) {
            case State.Idle:
                if (Input.touchCount > 0) {
                    touchOrigin = Input.GetTouch(0).position;
                    touchOriginWorldSpace = Camera.main.ScreenToWorldPoint(touchOrigin);
                    SetAimVector();
                    playerState = State.Aiming;
                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
                }
                break;
            case State.Aiming:
                if (Input.touchCount == 0) {
                    LaunchProjectile();
                    playerState = State.Launching;
                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
                }
                else {
                    SetAimVector();
                }
                break;
            case State.Launching:
                playerState = State.Idle;
                OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }


    private void SetAimVector() {
        touchPoint = Input.GetTouch(0).position;
        touchPointWorldSpace = Camera.main.ScreenToWorldPoint(touchPoint);
        launchVector = touchPointWorldSpace - touchOriginWorldSpace;
        launchVector *= touchSensibility;
        launchVector *= -1;
        if (launchVector.sqrMagnitude > launchVectorMax * launchVectorMax) {
            launchVector = launchVector.normalized * launchVectorMax;
        }
        else if (launchVector.sqrMagnitude < launchVectorMin * launchVectorMin) {
            launchVector = launchVector.normalized * launchVectorMin;
        }
        float angle = Vector2.SignedAngle(Vector2.right, launchVector);

        if (angle > maxLaunchAngle) {
            launchVector = Quaternion.Euler(0, 0, maxLaunchAngle) * Vector3.right * launchVector.magnitude;
        }
        else if (angle < minLaunchAngle) {
            launchVector = Quaternion.Euler(0, 0, minLaunchAngle) * Vector3.right * launchVector.magnitude;
        }
        launchVector *= launchSpeed;
    }
    private void LaunchProjectile() {
        Instantiate(projectilePrefab, launchOrigin.position, Quaternion.identity).GetComponent<Rigidbody>().velocity = launchVector;
    }



    public Vector3 GetLaunchVector() {
        return launchVector;
    }
    public Vector3 GetLaunchOrigin() {
        return launchOrigin.transform.position;
    }

    public void Hit(BaseProjectile projectile) {        
        health -= projectile.GetDamage();
        OnHealthChange?.Invoke(this, EventArgs.Empty);
        if (health <= 0) {
            playerState = State.Dead;
            OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
        }
    }

    public HittableType GetHittableType() {
        return HittableType.Player;
    }

    public float GetHealthNormalized() {
        return health / maxHealth;
    }
}
