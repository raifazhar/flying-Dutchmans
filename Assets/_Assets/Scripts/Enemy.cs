using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IHittable {
    public enum State {
        None,
        Shooting,
        Dead,
        GameOver,
    }
    public static Enemy Instance { get; private set; }
    public event EventHandler OnHealthChanged;
    public Action<State> onstatechangehad;
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public class OnStateChangeEventArgs : EventArgs {
        public State enemyState;
    }
    private int maxHealth = 100;
    private int health;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private GameObject enemyProjectile;
    [Header("AI")]
    [SerializeField] private float shootInterval = 2f;
    [SerializeField] private float heightThreshold = 2f;
    [SerializeField] private float launchVelocity = 2f;
    private float missChance = 0f;
    private Vector3 launchVector;
    private Vector3 targetPosition;
    private float shootTimer;

    private State enemyState;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        health = maxHealth;
        enemyState = State.None;
    }

    private void Start() {
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver(object sender, EventArgs e) {
        enemyState = State.GameOver;
    }

    public void Initialize() {
        health = maxHealth;
        enemyState = State.Shooting;
    }

    public void Update() {
        switch (enemyState) {
            case State.Shooting:
                Shoot();
                break;
        }
    }

    private void Shoot() {
        if (shootTimer > 0) {
            shootTimer -= Time.deltaTime;
            return;
        }
        shootTimer = shootInterval;
        if (MakeShootingDecision()) {
            GameObject projectile = Instantiate(enemyProjectile, launchPoint.position, launchPoint.rotation);
            projectile.GetComponent<Rigidbody>().velocity = launchVector;
            projectile.GetComponent<Rigidbody>().angularVelocity = launchVector;
        }
    }

    private bool MakeShootingDecision() {
        launchVector = Vector3.zero;
        List<Transform> activeObstacles = ObstacleSpawner.Instance.GetActiveObstacles();
        //Remove all obstacles below height threshold
        for (int i = 0; i < activeObstacles.Count; i++) {
            if (activeObstacles[i].position.y < heightThreshold) {
                activeObstacles.RemoveAt(i);
            }
        }
        //Find highest damage obstacle
        Transform highestDamageObstacle = null;
        int highestDamage = 0;
        for (int i = 0; i < activeObstacles.Count; i++) {
            if (activeObstacles[i].GetComponent<Obstacle>().GetDamage() >= highestDamage) {
                highestDamage = activeObstacles[i].GetComponent<Obstacle>().GetDamage();
                highestDamageObstacle = activeObstacles[i];
            }
        }

        if (highestDamageObstacle != null) {
            //Get the position of the target
            Vector3 currentTargetPosition = highestDamageObstacle.position;
            //Calculate the time it will take for the projectile to reach the current target Position
            float fallSpeed = highestDamageObstacle.GetComponent<Obstacle>().GetFallSpeed();
            Vector3 preliminaryLaunchVector = CalculateLaunchVector(launchPoint.position, currentTargetPosition, launchVelocity);
            float timeToTarget = Vector3.Distance(currentTargetPosition, launchPoint.position) / preliminaryLaunchVector.magnitude;
            //Compensate for the fall of the obstacle
            targetPosition = currentTargetPosition;
            targetPosition.y -= fallSpeed * timeToTarget;
            //Randomly make enemy miss target by offseting the target position on the x/z by a random amount (minimum 3)
            if (UnityEngine.Random.Range(0f, 1f) <= missChance) {
                float x = UnityEngine.Random.Range(3f, 5f);
                float z = UnityEngine.Random.Range(3f, 5f);
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f) {
                    targetPosition.x += x;
                }
                else {
                    targetPosition.x -= x;
                }
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f) {
                    targetPosition.z += z;
                }
                else {
                    targetPosition.z -= z;
                }
            }
            //Calculate the launch vector
            launchVector = CalculateLaunchVector(launchPoint.position, targetPosition, launchVelocity);
            return true;
        }
        return false;
    }


    private Vector3 CalculateLaunchVector(Vector3 startPos, Vector3 targetPos, float launchSpeed) {
        Vector3 toTarget = targetPos - startPos;
        // Set up the terms we need to solve the quadratic equations.
        float gSquared = Physics.gravity.sqrMagnitude;


        float b = launchSpeed * launchSpeed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;
        if (discriminant < 0) {
            b = (float)Math.Sqrt(gSquared * toTarget.sqrMagnitude);
            discriminant = 0;
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest shot with the given max speed:
        //float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Most direct shot with the given max speed:
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        // Lowest-speed arc available:
        //float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        float T = T_min;

        // Convert from time-to-hit to a launch velocity:
        return (toTarget / T - Physics.gravity * T / 2f);
    }
    public HittableType GetHittableType() {
        return HittableType.Enemy;
    }

    public void Hit(BaseProjectile projectile) {
        health -= projectile.GetDamage();
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        GameManager.Instance.AddScore(projectile.GetDamage(), projectile.gameObject.transform.position);
        if (health <= 0) {
            enemyState = State.Dead;
            OnStateChange?.Invoke(this, new OnStateChangeEventArgs { enemyState = enemyState });
        }
    }

    public float GetHealthNormalized() {
        return (float)health / maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth) {
        maxHealth = newMaxHealth;
    }
    public void SetMissChance(float newMissChance) {
        missChance = newMissChance;
    }
    public void SetShootInterval(float newShootInterval) {
        shootInterval = newShootInterval;
    }

}
