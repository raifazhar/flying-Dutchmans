using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    [SerializeField] private Transform playerObject;
    [SerializeField] private Transform enemyObject;
    [SerializeField] private LevelSO levelDetails;
    private void Awake() {
        Instance = this;
    //    Vector3 playerPos = playerObject.position;
    //    //Set enemy to be correct distance from player
    //    enemyObject.position = new Vector3(playerPos.x + levelDetails.enemyDistance, playerPos.y, playerPos.z);
    }





}
