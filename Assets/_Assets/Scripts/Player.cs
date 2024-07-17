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
    [SerializeField] private float touchSensitivity = 1f;
    [SerializeField] private float launchVectorMax;
    [SerializeField] private float launchVectorMin;
    [SerializeField] private float launchSpeed;
    private float cameraZDistance=1;
    private Vector3 touchOrigin;
    private Vector3 touchOriginWorldSpace;
    private Vector3 touchPoint;
    private Vector3 touchPointWorldSpace;
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

    void FixedUpdate() {
        switch (playerState) {
            case State.Idle:
                if (Input.touchCount > 0) {
                    //IF player has started touch in idle state then store the initial touchOrigin and get its world space coordinates as well
                    touchOrigin = Input.GetTouch(0).position;
                    touchOrigin.z = cameraZDistance;
                    touchOriginWorldSpace = Camera.main.ScreenToWorldPoint(touchOrigin);
                    SetAimVector();
                    playerState = State.Aiming;
                    //OnStateChange is invoked everytime state changes, this is for any visual stuff that needs to be done
                    //We seperate the logic from the visuals this way
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
        //The launchVector decides the direction the projectile is launched in when player releases the touch
        //Get the current touchPoint of the finger
        touchPoint = Input.GetTouch(0).position;
        touchPoint.z = cameraZDistance;
        touchPointWorldSpace = Camera.main.ScreenToWorldPoint(touchPoint);
        Vector3 screenVector = touchPointWorldSpace - touchOriginWorldSpace;
        //These decide the actual height and z axis tilt of the launchVector
        launchVector.y = screenVector.magnitude;
        launchVector.z = -screenVector.z; //We do -screenVector.z so the x axis is inverted, basically player has to pull in opposite direction of where they wanna shoot
        launchVector *= touchSensitivity;
        //This decides the x distance, basically the length of the launchVector
        launchVector.x = 1;
        //Clamp the launchVector so it doesn't go too far or too short
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
