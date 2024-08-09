using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject {
    public int LevelID;
    public ObstacleListSO ObstacleList;
    public float obstacleSpawnInterval;
    public float obstacleFallSpeed = 1f;
    public float[] spawnChance;
    public int playerHealth;
    public int enemyHealth;
    public int enemyCannons = 1;
    [Range(0, 1)] public float enemyMissChance;
    public float enemyShootInterval;
    public float enemyCannonOffset = 0f;
    [Range(0, 1)] public float invertedChance;
    public int levelSeed;
}
