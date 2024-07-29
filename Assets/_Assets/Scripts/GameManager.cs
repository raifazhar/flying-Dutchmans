using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum GameState {
        Starting,
        Playing,
        GameOver,
    }
    public enum GameEndState {
        Win,
        Lose,
    }
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnGameEndEventArgs> OnGameEnd;
    public class OnGameEndEventArgs : EventArgs {
        public GameEndState endState;
    }
    public event EventHandler OnGameOver;
    public event EventHandler<OnTogglePauseEventArgs> OnTogglePause;

    public class OnTogglePauseEventArgs : EventArgs {
        public bool isPaused;
    }

    [SerializeField] private LevelListSO levelsSO;
    [SerializeField] private float gameEndTime;
    private int levelIndex = 0;
    private readonly int targetFrameRate = 60;
    private bool isPaused = false;
    private int score = 0;
    private GameState gameState;
    private Coroutine gameEndCoroutine;

    [SerializeField] private bool isTutorial;
    [SerializeField] private LevelSO tutorialLevel;
    [SerializeField] private Transform scoreEffect;
    private void Awake() {
        Instance = this;
        gameState = GameState.Starting;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        Screen.orientation = ScreenOrientation.Portrait;
    }
    private void Start() {
        if (!isTutorial) {
            levelIndex = SelectedLevel.selectedLevel;
            InitializeLevel(levelsSO.levels[levelIndex]);
        }
        else {
            InitializeLevel(tutorialLevel);
        }
        Player.Instance.OnStateChange += Player_OnStateChange;
        Enemy.Instance.OnStateChange += Enemy_OnStateChange;
    }

    private void Enemy_OnStateChange(object sender, Enemy.OnStateChangeEventArgs e) {
        if (e.enemyState == Enemy.State.Dead && gameState == GameState.Playing) {
            //Enemy is dead, end the game
            gameState = GameState.GameOver;
            OnGameOver?.Invoke(this, EventArgs.Empty);
            if (gameEndCoroutine == null) {
                gameEndCoroutine = StartCoroutine(GameEndCoroutine(gameEndTime, GameEndState.Win));
            }
        }
    }

    private void Player_OnStateChange(object sender, Player.OnStateChangeEventArgs e) {
        if (e.playerState == Player.State.Dead && gameState == GameState.Playing) {
            //Player is dead, end the game
            gameState = GameState.GameOver;
            OnGameOver?.Invoke(this, EventArgs.Empty);
            if (gameEndCoroutine == null) {
                gameEndCoroutine = StartCoroutine(GameEndCoroutine(gameEndTime, GameEndState.Lose));
            }
        }
    }


    IEnumerator GameEndCoroutine(float duration, GameEndState e) {
        yield return new WaitForSeconds(duration);
        OnGameEnd?.Invoke(this, new OnGameEndEventArgs { endState = e });

    }
    private void InitializeLevel(LevelSO level) {
        if (gameState == GameState.Starting) {
            gameState = GameState.Playing;
            Player.Instance.SetMaxHealth(level.playerHealth);
            Player.Instance.Initialize();
            Enemy.Instance.SetMaxHealth(level.enemyHealth);
            Enemy.Instance.SetMissChance(level.enemyMissChance);
            Enemy.Instance.SetShootInterval(level.enemyShootInterval);
            Enemy.Instance.Initialize();
            if (!isTutorial) {
                ObstacleSpawner.Instance.SetInvertedChance(level.invertedChance);
                ObstacleSpawner.Instance.SetSpawnInterval(level.obstacleSpawnInterval);
                ObstacleSpawner.Instance.SetObstacles(level.ObstacleList.obstaclePrefabs, level.spawnChance);
                ObstacleSpawner.Instance.Initialize();
            }
        }
    }

    #region LevelNavigation
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
    #endregion

    #region Score
    public int GetScore() {
        return score;
    }

    public void AddScore(int amount) {
        score += amount;
    }
    public void AddScore(int amount, Vector3 position) {
        AddScore(amount);
        Transform effect = Instantiate(scoreEffect, position, Quaternion.identity);
        effect.GetComponent<ScoreEffect>().SetScore(amount);
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
    #endregion

    public void TogglePauseGame() {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        OnTogglePause?.Invoke(this, new OnTogglePauseEventArgs { isPaused = isPaused });
    }

    public void QuitGame() {
        Application.Quit();
    }


}
