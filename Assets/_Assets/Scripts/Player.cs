using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IHittable {


    public static Player Instance { get; private set; }
    public enum State {
        None,
        Idle,
        Aiming,
        Launching,
        Dead,
        GameOver,
    }
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public class OnStateChangeEventArgs : EventArgs {
        public State playerState;
    }
    public event EventHandler OnHealthChange;

    [Header("Health Settings")]
    private int maxHealth = 100;
    private int health;
    [SerializeField] private float cameraShakeOnHit = 0.5f;

    [Header("Player Settings")]
    [SerializeField] private Transform launchOrigin;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float touchSensitivityHorizontal = 1f;
    [SerializeField] private float touchSensitivityVertical = 1f;
    [SerializeField] private float launchVectorMax;
    [SerializeField] private float launchVectorMin;
    [SerializeField] private float launchSpeed;
    [SerializeField, Range(0, 1)] private float slowDownFactor = 0.5f;
    private readonly float cameraZDistance = 1;
    [SerializeField] private Vector3 startingVector;
    private Vector3 touchOrigin;
    private Vector3 touchOriginWorldSpace;
    private Vector3 touchPoint;
    private Vector3 touchPointWorldSpace;
    private Vector3 launchVector;
    private Vector3 screenVector;

    [SerializeField] private float launchCooldown = 1f;
    private float launchTimer = 0f;


    private State playerState;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        playerState = State.None;
        health = maxHealth;
    }

    private void Start() {
        GameManager.Instance.OnGameEnd += GameManager_OnGameEnd;
    }

    private void GameManager_OnGameEnd(object sender, GameManager.OnGameEndEventArgs e) {
        playerState = State.GameOver;
    }

    public void Initialize() {
        health = maxHealth;
        playerState = State.Idle;
    }
    void FixedUpdate() {
        switch (playerState) {
            case State.None:
                break;
            case State.Idle:
                if (Input.touchCount > 0 && !IsTouchOverUI()) {
                    //IF player has started touch in idle state then store the initial touchOrigin and get its world space coordinates as well
                    touchOrigin = Input.GetTouch(0).position;
                    touchOrigin.z = cameraZDistance;
                    touchOriginWorldSpace = Camera.main.ScreenToWorldPoint(touchOrigin);
                    SetAimVector();
                    playerState = State.Aiming;
                    //OnStateChange is invoked everytime state changes, this is for any visual stuff that needs to be done
                    //We seperate the logic from the visuals this way
                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
                    Time.timeScale = slowDownFactor;
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                }
                break;
            case State.Aiming:
                if (Input.touchCount == 0) {
                    LaunchProjectile();
                    playerState = State.Launching;
                    launchTimer = launchCooldown;
                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                }
                else {
                    SetAimVector();
                }
                break;
            case State.Launching:
                launchTimer -= Time.fixedDeltaTime;
                if (launchTimer < 0) {
                    playerState = State.Idle;
                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
                }
                break;
            case State.Dead:
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }


    private void SetAimVector() {
        //The launchVector decides the direction the projectile is launched in when player releases the touch
        //Get the current touchPoint of the finger
        touchPoint = Input.GetTouch(0).position;
        touchPoint.z = cameraZDistance;
        touchPointWorldSpace = Camera.main.ScreenToWorldPoint(touchPoint);
        screenVector = touchPointWorldSpace - touchOriginWorldSpace;
        //Touch Sensitivity field is used to change the sensitivity of the launchVector as per user touch
        screenVector.y *= touchSensitivityVertical;
        screenVector.z *= touchSensitivityHorizontal;
        //Set the launchVector to the startingVector then add the relevant screenVector components to it
        launchVector = startingVector;
        //These decide the actual height and z axis tilt of the launchVector
        launchVector.y -= screenVector.y;
        launchVector.z -= screenVector.z; //We do -screenVector.z so the x axis is inverted, basically player has to pull in opposite direction of where they wanna shoot
        //This decides the x distance, basically the length of the launchVector
        launchVector.x = 1;
        //Clamp the launchVector so it doesn't go too far or too short
        //We use launchVector.sqrMagnitude and check against the square of the max and min values to avoid using square root
        //This is because square root is a costly operation
        if (launchVector.sqrMagnitude > launchVectorMax * launchVectorMax) {
            launchVector = launchVector.normalized * launchVectorMax;
        }
        else if (launchVector.sqrMagnitude < launchVectorMin * launchVectorMin) {
            launchVector = launchVector.normalized * launchVectorMin;
        }
        //Multiply the launchVector by launchSpeed to get the actual speed of the projectile
        launchVector *= launchSpeed;
    }
    private void LaunchProjectile() {
        GameObject launchedProjectile = Instantiate(projectilePrefab, launchOrigin.position, Quaternion.identity);
        launchedProjectile.GetComponent<Rigidbody>().velocity = launchVector;
        launchedProjectile.GetComponent<Rigidbody>().angularVelocity = launchVector;
        launchVector = Vector3.zero;
        screenVector = Vector3.zero;
    }

    private bool IsTouchOverUI() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    #region Interface
    public void Hit(BaseProjectile projectile) {
        health -= projectile.GetDamage();
        OnHealthChange?.Invoke(this, EventArgs.Empty);
        CameraController.Instance.AddTrauma(cameraShakeOnHit);
        GameManager.Instance.RemoveScore(projectile.GetDamage() / 2);
        if (health <= 0) {
            playerState = State.Dead;
            OnStateChange?.Invoke(this, new OnStateChangeEventArgs { playerState = playerState });
        }
    }

    public HittableType GetHittableType() {
        return HittableType.Player;
    }
    #endregion

    #region Getters
    public float GetHealthNormalized() {
        return (float)health / (float)maxHealth;
    }

    public Vector3 GetLaunchVector() {
        return launchVector;
    }
    public Vector3 GetLaunchOrigin() {
        return launchOrigin.transform.position;
    }
    public Vector3 GetScreenVector() {
        return screenVector;
    }

    public float GetLaunchTimerNormalized() {
        return launchTimer / launchCooldown;
    }

    public void SetMaxHealth(int newMaxHealth) {
        maxHealth = newMaxHealth;
    }

    #endregion

}
