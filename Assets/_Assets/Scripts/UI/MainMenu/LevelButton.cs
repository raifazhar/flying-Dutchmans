using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{

    [SerializeField] private GameObject levelText;
    [SerializeField] private GameObject locker;
    [SerializeField] private Button button;



    public void CreatButton(int index,int difficulty)
    {
        Debug.Log(levelText);
        levelText.GetComponent<TMP_Text>().text=(index+1).ToString();

        if (index <= PlayerPrefs.GetInt("UnlockedLevels", 0))
        {
            EnabableButton();
        }

        
        button.onClick.AddListener(() => LoadLevel(index + 1));
    }

    private void EnabableButton()
    {
        button.interactable = true;
        locker.SetActive(false);
    }

    public void LoadLevel(int levelIndex)
    {
        
        SelectedLevel.SetSelectedLevel(levelIndex - 1);
        SceneManager.LoadScene(Scenes.GameScene);
    }


}
