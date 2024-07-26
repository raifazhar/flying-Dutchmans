using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {


    public static GameManager Instance { get; private set; }

    public event EventHandler<OnGameEndEventArgs> OnGameEnd;


    public enum GameState {
        Starting,
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
    private int score = 0;
    [SerializeField] private LevelListSO levelsSO;
    private int levelIndex = 0;
    private readonly int targetFrameRate = 60;


    private GameState gameState;


    private void Awake() {
        Instance = this;
        gameState = GameState.Starting;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        Screen.orientation = ScreenOrientation.Portrait;
    }
    private void Start() {
        InitializeLevel();
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


    private void InitializeLevel() {
        if (gameState == GameState.Starting) {
            gameState = GameState.Playing;
            levelIndex = SelectedLevel.selectedLevel;
            Player.Instance.SetMaxHealth(levelsSO.levels[levelIndex].playerHealth);
            Player.Instance.Initialize();
            Enemy.Instance.SetMaxHealth(levelsSO.levels[levelIndex].enemyHealth);
            Enemy.Instance.SetMissChance(levelsSO.levels[levelIndex].enemyMissChance);
            Enemy.Instance.SetShootInterval(levelsSO.levels[levelIndex].enemyShootInterval);
            Enemy.Instance.Initialize();
            ObstacleSpawner.Instance.SetInvertedChance(levelsSO.levels[levelIndex].invertedChance);
            ObstacleSpawner.Instance.SetObstacles(levelsSO.levels[levelIndex].ObstacleList.obstaclePrefabs, levelsSO.levels[levelIndex].spawnChance);
            ObstacleSpawner.Instance.Initialize();
        }
    }

    public void RetryLevel() {
        SelectedLevel.SetSelectedLevel(SelectedLevel.selectedLevel);
        SceneManager.LoadScene(Scenes.GameScene);
    }

    public void NextLevel() {
        if (SelectedLevel.selectedLevel + 1 >= levelsSO.levels.Count)
            return;
        SelectedLevel.SetSelectedLevel(SelectedLevel.selectedLevel + 1);
        SceneManager.LoadScene(Scenes.GameScene);
    }
    public void BackToMenu() {
        SceneManager.LoadScene(Scenes.MainMenu);
    }
    public int GetScore() {
        return score;
    }

    public void AddScore(int amount) {
        score += amount;
    }
    public void AddScore(int amount, Vector3 position) {
        AddScore(amount);
    }

    public void RemoveScore(int amount) {
        score -= amount;
        if (score < 0) {
            score = 0;
        }

    }
    public void RemoveScore(int amount, Vector3 position) {
        RemoveScore(amount);
    }
}
