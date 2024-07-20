using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

     
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnGameEndEventArgs> OnGameEnd;


    public enum GameState{
        Playing,
        GameOver,
    }
    public enum GameEndState {
        Win,
        Lose,
    }
    public class OnGameEndEventArgs : EventArgs {
        public GameEndState endState;
    }
    [SerializeField] private Transform playerObject;
    [SerializeField] private Transform enemyObject;
    [SerializeField] private LevelSO levelDetails;
    private GameState gameState;


     private void Awake() {
        Instance = this;
        gameState=GameState.Playing;
    //    Vector3 playerPos = playerObject.position;
    //    //Set enemy to be correct distance from player
    //    enemyObject.position = new Vector3(playerPos.x + levelDetails.enemyDistance, playerPos.y, playerPos.z);
    }

    private void Start() {
        Player.Instance.OnStateChange += Player_OnStateChange;
        Enemy.Instance.OnStateChange += Enemy_OnStateChange;
    }

    private void Enemy_OnStateChange(object sender, Enemy.OnStateChangeEventArgs e) {
        if(e.enemyState == Enemy.State.Dead && gameState==GameState.Playing) {
            //Enemy is dead, end the game
            gameState=GameState.GameOver;
            OnGameEnd?.Invoke(this, new OnGameEndEventArgs { endState = GameEndState.Win });
        }
    }

    private void Player_OnStateChange(object sender, Player.OnStateChangeEventArgs e) {
        if(e.playerState == Player.State.Dead && gameState == GameState.Playing) {
            //Player is dead, end the game
            gameState=GameState.GameOver;
            OnGameEnd?.Invoke(this, new OnGameEndEventArgs { endState = GameEndState.Lose });
         }
    }
}
