using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndUI : MonoBehaviour {

    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    private void Start() {
        GameManager.Instance.OnGameEnd += GameManager_OnGameEnd;
        endUI.SetActive(false);
        winUI.SetActive(false);
        loseUI.SetActive(false);
    }

    private void GameManager_OnGameEnd(object sender, GameManager.OnGameEndEventArgs e) {
        endUI.SetActive(true);
        if (e.endState == GameManager.GameEndState.Win) {
            winUI.SetActive(true);
        }
        else {
            winUI.SetActive(false);
        }
    }
}
