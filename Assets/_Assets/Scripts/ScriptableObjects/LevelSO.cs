using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject {
    public int LevelID;
    public float enemyDistance;
    public Transform Environment;
}
