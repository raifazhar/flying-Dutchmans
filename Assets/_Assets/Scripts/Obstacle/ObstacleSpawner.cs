using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    public static ObstacleSpawner Instance { get; private set; }
    [SerializeField] private float spawnInterval = 2f;
    private Transform[] obstaclePrefabs;
    private float[] spawnChance;
    private float spawnTimer = 0f;
    private float invertedChance = 0f;
    [SerializeField] private BoxCollider boxCollider;
    private Bounds bounds;

    private List<Transform> activeObstacles;

    private bool active = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    void Start() {
        bounds = boxCollider.bounds;
        spawnTimer = spawnInterval;
        activeObstacles = new List<Transform>();
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver(object sender, EventArgs e) {
        active = false;
    }

    // Update is called once per frame
    void Update() {
        if (!active) {
            return;
        }
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval) {
            spawnTimer = 0f;
            SpawnObstacle();
        }
    }



    private void SpawnObstacle() {
        Vector3 spawnPosition = new(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
        Transform chosenObstaclePrefab;
        float randomValue = UnityEngine.Random.value;
        float sum = 0;
        for (int i = 0; i < spawnChance.Length; i++) {
            sum += spawnChance[i];
            if (randomValue <= sum) {
                chosenObstaclePrefab = obstaclePrefabs[i];
                Transform generatedObstacle = Instantiate(chosenObstaclePrefab, spawnPosition, Quaternion.identity);
                if (UnityEngine.Random.Range(0f, 1f) <= invertedChance) {
                    generatedObstacle.GetComponent<IFallingObstacle>().SetInverted(true);
                }
                activeObstacles.Add(generatedObstacle);
                break;
            }
        }

    }

    public void SetObstacles(Transform[] obstaclePrefabs, float[] spawnChance) {
        this.obstaclePrefabs = obstaclePrefabs;
        this.spawnChance = spawnChance;
    }
    public List<Transform> GetActiveObstacles() {
        return activeObstacles;
    }
    public void SetSpawnInterval(float newSpawnInterval) {
        spawnInterval = newSpawnInterval;
    }
    public void RemoveObstacleFromList(Transform obstacle) {
        activeObstacles.Remove(obstacle);
    }

    public void SetInvertedChance(float newInvertedChance) {
        invertedChance = newInvertedChance;
    }
    public void Initialize() {
        active = true;

    }
}
