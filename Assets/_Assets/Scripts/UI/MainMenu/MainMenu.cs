using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour {

    public static MainMenu Instance {
        get; private set;
    }
    [SerializeField] private Transform menu;
    [SerializeField] private Transform background;
    [SerializeField] private Transform levelSelect;
    [SerializeField] private Transform levelTransition;
    [SerializeField] private float levelTransitionTime = 1f;
    private Coroutine transitionCoroutine;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    public void OnPlayClick() {
        background.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(true);
    }

    public void OnOptionsClick() {
        menu.gameObject.SetActive(false);
    }

    public void OnBackClick() {
        levelSelect.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
    }

    public void OnExitClick() {
        Application.Quit();
    }

    public void LoadLevel(int levelIndex) {
        if (transitionCoroutine != null) {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionToLevel(levelIndex));
    }

    private IEnumerator TransitionToLevel(int levelIndex) {
        levelTransition.gameObject.SetActive(true);
        yield return new WaitForSeconds(levelTransitionTime);
        SelectedLevel.SetSelectedLevel(levelIndex);
        SceneManager.LoadScene(Scenes.GameScene);
        transitionCoroutine = null;
    }




}

