using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    [SerializeField] private Transform[] obstaclePrefabs;
    [SerializeField] private float spawnInterval = 2f;
    private float spawnTimer = 0f;
    [SerializeField] private BoxCollider boxCollider;
    private Bounds bounds;
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
        Transform obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Vector3 spawnPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}
