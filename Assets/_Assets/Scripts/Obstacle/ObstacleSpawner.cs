using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    public static ObstacleSpawner Instance { get; private set; }
    [SerializeField] private float spawnInterval = 2f;
    private Transform[] obstaclePrefabs;
    private float[] spawnChance;
    private float spawnTimer = 0f;
    [SerializeField] private BoxCollider boxCollider;
    private Bounds bounds;

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
    }

    // Update is called once per frame
    void Update() {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval) {
            spawnTimer = 0f;
            SpawnObstacle();
        }
    }

    private void SpawnObstacle() {
        Vector3 spawnPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
        Transform chosenObstaclePrefab;
        float randomValue = Random.value;
        float sum = 0;
        for (int i = 0; i < spawnChance.Length; i++) {
            sum += spawnChance[i];
            if (randomValue <= sum) {
                chosenObstaclePrefab = obstaclePrefabs[i];
                Instantiate(chosenObstaclePrefab, spawnPosition, Quaternion.identity);
                break;
            }
        }

    }

    public void SetObstacles(Transform[] obstaclePrefabs, float[] spawnChance) {
        this.obstaclePrefabs = obstaclePrefabs;
        this.spawnChance = spawnChance;
    }
}
