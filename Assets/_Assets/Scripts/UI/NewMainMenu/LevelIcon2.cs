using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelIcon2 : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI levelText;
    private int levelIndex = -1;

    public void SetLevelIndex(int i) {
        levelIndex = i;
        levelText.text = (i + 1).ToString();
    }

    public int GetLevelIndex() {
        return levelIndex;
    }
}
