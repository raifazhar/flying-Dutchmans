using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public static GameManager Instance { get; private set; }

    public event EventHandler<OnGameEndEventArgs> OnGameEnd;


    public enum GameState {
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
    [SerializeField] private LevelSO levelDetails;
    private GameState gameState;


    private void Awake() {
        Instance = this;
        gameState = GameState.Playing;

    }
    private void Start() {
        Vector3 playerPos = Player.Instance.transform.position;
        //Set enemy to be correct distance from player
        Enemy.Instance.transform.position = new Vector3(playerPos.x + levelDetails.enemyDistance, Enemy.Instance.transform.position.y, Enemy.Instance.transform.position.z);
        Player.Instance.OnStateChange += Player_OnStateChange;
        Enemy.Instance.OnStateChange += Enemy_OnStateChange;
    }

    private void Enemy_OnStateChange(object sender, Enemy.OnStateChangeEventArgs e) {
        if (e.enemyState == Enemy.State.Dead && gameState == GameState.Playing) {
            //Enemy is dead, end the game
            gameState = GameState.GameOver;
            OnGameEnd?.Invoke(this, new OnGameEndEventArgs { endState = GameEndState.Win });
        }
    }

    private void Player_OnStateChange(object sender, Player.OnStateChangeEventArgs e) {
        if (e.playerState == Player.State.Dead && gameState == GameState.Playing) {
            //Player is dead, end the game
            gameState = GameState.GameOver;
            OnGameEnd?.Invoke(this, new OnGameEndEventArgs { endState = GameEndState.Lose });
        }
    }
}
