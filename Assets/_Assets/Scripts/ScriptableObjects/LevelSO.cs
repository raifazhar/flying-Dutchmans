using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject {
    public int LevelID;
    public ObstacleListSO ObstacleList;
    public float obstacleSpawnInterval;
    public float[] spawnChance;
    public int playerHealth;
    public int enemyHealth;
    [Range(0, 1)] public float enemyMissChance;
    public float enemyShootInterval;
    [Range(0, 1)] public float invertedChance;
    public int levelSeed;
    public int startingAmmo;
}
