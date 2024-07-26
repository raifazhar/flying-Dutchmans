using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float adjustmentValue = 0.1f;
    [SerializeField] private float launchVelocity = 2f;
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
        GameManager.Instance.OnGameEnd += GameManager_OnGameEnd;
    }

    private void GameManager_OnGameEnd(object sender, GameManager.OnGameEndEventArgs e) {
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

        Vector3 toTarget = Vector3.zero;
        if (highestDamageObstacle != null) {
            //Reduce the target position by the fall speed of the obstacle and the distance between the launch point and the target
            targetPosition = highestDamageObstacle.position;
            float distance = Vector3.Distance(launchPoint.position, targetPosition);
            distance *= adjustmentValue;
            targetPosition.y -= highestDamageObstacle.GetComponent<Obstacle>().GetFallSpeed() * distance * 2f;
            toTarget = targetPosition - transform.position;
            // Set up the terms we need to solve the quadratic equations.
            float gSquared = Physics.gravity.sqrMagnitude;


            float b = launchVelocity * launchVelocity + Vector3.Dot(toTarget, Physics.gravity);
            float discriminant = b * b - gSquared * toTarget.sqrMagnitude;
            if (discriminant < 0) {
                b = (float)Math.Sqrt(gSquared * toTarget.sqrMagnitude);
                discriminant = 0;
            }

            float discRoot = Mathf.Sqrt(discriminant);

            // Highest shot with the given max speed:
            float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

            // Most direct shot with the given max speed:
            float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

            // Lowest-speed arc available:
            float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

            float T = T_min;

            // Convert from time-to-hit to a launch velocity:
            launchVector = toTarget / T - Physics.gravity * T / 2f;
            return true;
        }
        return false;
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.5f);
    }
}
