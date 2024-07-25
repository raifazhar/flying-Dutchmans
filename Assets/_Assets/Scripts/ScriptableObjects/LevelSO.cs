using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject {
    public int LevelID;
    public ObstacleListSO ObstacleList;
    public float[] spawnChance;
    public int playerHealth;
    public int enemyHealth;
}
