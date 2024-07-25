using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelIcon : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI levelText;
    private int levelIndex;



    public void SetLevel(int levelIndex) {
        this.levelIndex = levelIndex;
        levelText.text = levelIndex.ToString();
    }

    public void OnClick() {
        LevelSelect.Instance.LoadLevel(levelIndex);
    }

}
