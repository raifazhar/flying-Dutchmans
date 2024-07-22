using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject {
    public int LevelID;
    public Transform Environment;
    public ObstacleListSO ObstacleList;
    public float[] spawnChance;
}
