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

    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public class OnStateChangeEventArgs : EventArgs {
        public State enemyState;
    }

    public event EventHandler<OnHitArgs> OnHit;
    public class OnHitArgs : EventArgs {
        public Collision collision;
    }
    public event EventHandler OnCannonHealthChanged;
    private int cannonHealth = 10;
    [SerializeField] private Transform enemyCannonSpawnPoint;
    private int numCannons = 1;
    [SerializeField] private float enemyCannonSpawnExtent = 5f;
    [SerializeField] private Transform enemyCannonPrefab;
    private EnemyCannon[] cannons;
    [SerializeField] private GameObject enemyProjectile;
    [Header("AI")]
    [SerializeField] private float shootInterval = 2f;
    [SerializeField] private float heightThreshold = 2f;
    [SerializeField] private float launchVelocity = 2f;
    [SerializeField] private BoxCollider targetCollider;
    private Bounds targetColliderBounds;
    private float missChance = 0f;
    private Vector3 launchVector;
    private Vector3 targetPosition;
    private float shootTimer;
    private bool cannonsSpawned = false;
    private State enemyState;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        enemyState = State.None;
        targetColliderBounds = targetCollider.bounds;
        targetCollider.enabled = false;
    }

    private void Start() {
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver(object sender, EventArgs e) {
        enemyState = State.GameOver;
        OnStateChange?.Invoke(this, new OnStateChangeEventArgs { enemyState = enemyState });
    }

    public void Initialize() {
        for (int i = 0; i < numCannons; i++) {
            cannons[i].SetHealth(cannonHealth);
        }
        OnCannonHealthChanged?.Invoke(this, EventArgs.Empty);
        enemyState = State.Shooting;
        OnStateChange?.Invoke(this, new OnStateChangeEventArgs { enemyState = enemyState });
    }

    public void Update() {
        switch (enemyState) {
            case State.Shooting:
                Shoot();
                break;
        }
    }

    public void SpawnCannons() {
        if (cannonsSpawned)
            return;
        cannonsSpawned = true;
        float maxZCoordinate = enemyCannonSpawnExtent;
        float minZCoordinate = -enemyCannonSpawnExtent;
        float xCoordinate = enemyCannonSpawnPoint.position.x;
        float yCoordinate = enemyCannonSpawnPoint.position.y;
        cannons = new EnemyCannon[numCannons];
        //Instantiate the amount of cannons specified by numCannons evenly distributed between the max and min z coordinates
        float dt = Mathf.Abs((maxZCoordinate - minZCoordinate)) / (numCannons);

        for (int i = 0; i < numCannons; i++) {
            float t = (0.5f * dt) + (i * dt);
            t /= Mathf.Abs(maxZCoordinate - minZCoordinate);
            float zPos = Mathf.Lerp(minZCoordinate, maxZCoordinate, t);
            Vector3 spawnPosition = new Vector3(xCoordinate, yCoordinate, zPos);
            Transform cannon = Instantiate(enemyCannonPrefab, spawnPosition, Quaternion.identity);
            cannons[i] = cannon.GetComponent<EnemyCannon>();
            cannon.SetParent(transform);
            cannons[i].SetDomainExtent(enemyCannonSpawnExtent / numCannons);
        }

    }

    private void Shoot() {
        if (shootTimer > 0) {
            shootTimer -= Time.deltaTime;
            return;
        }
        shootTimer = shootInterval;
        LaunchProjectileFromRandomCannon();

    }

    private void LaunchProjectileFromRandomCannon() {
        launchVector = Vector3.zero;
        //List<Transform> activeObstacles = ObstacleSpawner.Instance.GetActiveObstacles();
        ////Remove all obstacles that will be below height threshold by the time we hit them
        //for (int i = 0; i < activeObstacles.Count; i++) {
        //    Vector3 currentObstaclePosition = activeObstacles[i].position;
        //    float fallSpeed = activeObstacles[i].GetComponent<IFallingObstacle>().GetFallSpeed();
        //    Vector3 launchVector = CalculateLaunchVector(cannons[0].GetLaunchOrigin().position, currentObstaclePosition, launchVelocity);
        //    float timeToTarget = Vector3.Distance(currentObstaclePosition, cannons[0].GetLaunchOrigin().position) / launchVector.magnitude;
        //    if (currentObstaclePosition.y - fallSpeed * timeToTarget < heightThreshold) {
        //        activeObstacles.RemoveAt(i);
        //        i--;
        //    }

        //}
        ////Find highest damage obstacle
        //Transform highestDamageObstacle = null;
        //int highestDamage = 0;
        //for (int i = 0; i < activeObstacles.Count; i++) {
        //    if (activeObstacles[i].GetComponent<IFallingObstacle>().GetDamage() >= highestDamage
        //        && !activeObstacles[i].GetComponent<IFallingObstacle>().IsInverted()) {
        //        highestDamage = activeObstacles[i].GetComponent<IFallingObstacle>().GetDamage();
        //        highestDamageObstacle = activeObstacles[i];
        //    }
        //}
        //Choose a random cannon to shoot out of, make sure the cannon is alive
        int randomCannonIndex = UnityEngine.Random.Range(0, numCannons);
        while (!cannons[randomCannonIndex].IsAlive()) {
            randomCannonIndex = UnityEngine.Random.Range(0, numCannons);
        }
        float cannonLaunchDelay = cannons[randomCannonIndex].GetLaunchDelay();

        //if (highestDamageObstacle != null) {
        //    //Get the position of the target
        //    Vector3 currentTargetPosition = highestDamageObstacle.position;
        //    float fallSpeed = highestDamageObstacle.GetComponent<IFallingObstacle>().GetFallSpeed();
        //    Vector3 preliminaryLaunchVector = CalculateLaunchVector(cannons[randomCannonIndex].GetLaunchOrigin().position, currentTargetPosition, launchVelocity);
        //    float timeToTarget = Vector3.Distance(currentTargetPosition, cannons[randomCannonIndex].GetLaunchOrigin().position) / preliminaryLaunchVector.magnitude;
        //    timeToTarget += cannonLaunchDelay;
        //    //Compensate for the fall of the obstacle
        //    targetPosition = currentTargetPosition;
        //    targetPosition.y -= fallSpeed * timeToTarget;
        //}
        //else {
        //    targetPosition = Player.Instance.transform.position;
        //}
        targetPosition = Player.Instance.transform.position;
        //Calculate the time it will take for the projectile to reach the current target Position
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
        launchVector = CalculateLaunchVector(cannons[randomCannonIndex].GetLaunchOrigin().position, targetPosition, launchVelocity);
        cannons[randomCannonIndex].LaunchProjectile(enemyProjectile.transform, launchVector);
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
    public void Hit(BaseProjectile projectile, Collision collision) {
        if (enemyState == State.GameOver)
            return;
        //Check which cannons domain was hit (cannon domain extends in z axis so each cannon has equivalent domain)
        float collisionZ = collision.GetContact(0).point.z;
        for (int i = 0; i < numCannons; i++) {
            if (cannons[i].IsZCoordinateInDomain(collisionZ) && cannons[i].IsAlive()) {
                cannons[i].DoDamage(projectile.GetDamage());
                GameManager.Instance.AddScore(projectile.GetDamage(), projectile.gameObject.transform.position);
                OnHit?.Invoke(this, new OnHitArgs { collision = collision });
                SoundManager.Playsound(SoundManager.Sound.EnemyHit);
                OnCannonHealthChanged?.Invoke(this, EventArgs.Empty);
                break;
            }
        }

        if (GetCannonHealthCumulativeNormalized() <= 0) {
            enemyState = State.Dead;
            OnStateChange?.Invoke(this, new OnStateChangeEventArgs { enemyState = enemyState });
        }

    }

    public float GetCannonHealthCumulativeNormalized() {
        float health = 0;
        for (int i = 0; i < numCannons; i++) {
            health += cannons[i].GetHealthNormalized();
        }
        return health / numCannons;
    }
    public void SetMaxCannonHealth(int newMaxHealth) {
        cannonHealth = newMaxHealth;
    }
    public void SetMissChance(float newMissChance) {
        missChance = newMissChance;
    }
    public void SetShootInterval(float newShootInterval) {
        shootInterval = newShootInterval;
    }

    public void SetNumCannons(int i) {
        numCannons = i;
    }

    public Bounds GetBounds() {
        return targetColliderBounds;
    }
}
