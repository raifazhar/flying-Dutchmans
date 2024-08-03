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
    [SerializeField] private float gameStartTime = 1f;
    [SerializeField] private float gameEndTime = 1f;
    [SerializeField] private Transform levelOutTransition;
    [SerializeField] private float levelOutTranisitonTime = 1f;
    private int levelIndex = 0;
    private readonly int targetFrameRate = 60;
    private bool isPaused = false;
    private int score = 0;
    private GameState gameState;
    private Coroutine gameEndCoroutine;
    private Coroutine gameStartCoroutine;
    private Coroutine levelTransitionCoroutine;

    private void Awake() {
        Instance = this;
        gameState = GameState.Starting;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        Screen.orientation = ScreenOrientation.Portrait;
    }
    private void Start() {
        levelIndex = SelectedLevel.selectedLevel;
        Player.Instance.OnStateChange += Player_OnStateChange;
        Enemy.Instance.OnStateChange += Enemy_OnStateChange;
        StartCoroutine(StartGameCoroutine(gameStartTime));
    }

    private IEnumerator StartGameCoroutine(float duration) {
        yield return new WaitForSeconds(duration);
        InitializeLevel(levelsSO.levels[levelIndex]);
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
            UnityEngine.Random.InitState(level.levelSeed);
            gameState = GameState.Playing;
            Player.Instance.SetMaxHealth(level.playerHealth);
            Player.Instance.Initialize();
            Enemy.Instance.SetMaxHealth(level.enemyHealth);
            Enemy.Instance.SetMissChance(level.enemyMissChance);
            Enemy.Instance.SetShootInterval(level.enemyShootInterval);
            Enemy.Instance.Initialize();
            ObstacleSpawner.Instance.SetInvertedChance(level.invertedChance);
            ObstacleSpawner.Instance.SetSpawnInterval(level.obstacleSpawnInterval);
            ObstacleSpawner.Instance.SetObstacles(level.ObstacleList.obstaclePrefabs, level.spawnChance);
            ObstacleSpawner.Instance.Initialize();
        }
    }

    #region LevelNavigation
    public void RetryLevel() {
        Time.timeScale = 1f;

        SelectedLevel.SetSelectedLevel(SelectedLevel.selectedLevel);
        if (levelTransitionCoroutine == null) {
            levelTransitionCoroutine = StartCoroutine(LevelTransitionCoroutine(levelOutTranisitonTime));
        }
    }

    public void NextLevel() {
        Time.timeScale = 1f;
        if (SelectedLevel.selectedLevel + 1 >= levelsSO.levels.Count)
            return;
        SelectedLevel.SetSelectedLevel(SelectedLevel.selectedLevel + 1);
        if (levelTransitionCoroutine == null) {
            levelTransitionCoroutine = StartCoroutine(LevelTransitionCoroutine(levelOutTranisitonTime));
        }
    }
    public void BackToMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Scenes.MainMenu);
    }
    #endregion


    IEnumerator LevelTransitionCoroutine(float duration) {
        levelOutTransition.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(Scenes.GameScene);
        levelTransitionCoroutine = null;
    }
    #region Score
    public int GetScore() {
        return score;
    }

    public void AddScore(int amount) {
        score += amount;
    }
    public void AddScore(int amount, Vector3 position) {
        AddScore(amount);
        EffectHandler.Instance.SpawnTextEffect(amount.ToString(), position, TextEffect.TextColor.Blue);
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
