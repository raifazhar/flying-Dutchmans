using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {
    public static LevelSelect Instance { get; private set; }
    [SerializeField] private Transform levelIconTemplate;
    [SerializeField] private Transform levelSelectContainer;
    [SerializeField] private LevelListSO levelList;
    void Awake() {
        Instance = this;
        InitializeLevels();
    }
    private void InitializeLevels() {
        for (int i = 0; i < levelList.levels.Count; i++) {
            Transform levelIcon = Instantiate(levelIconTemplate, levelSelectContainer);
            levelIcon.gameObject.SetActive(true);
            levelIcon.GetComponent<LevelIcon>().SetLevel(i + 1);
        }
    }

    public void LoadLevel(int levelIndex) {
        if (levelIndex < 1 || levelIndex > levelList.levels.Count + 1) {
            Debug.LogWarning("Level does not exist");
            return;
        }
        SelectedLevel.SetSelectedLevel(levelIndex - 1);
        SceneManager.LoadScene(Scenes.GameScene);
    }

}
