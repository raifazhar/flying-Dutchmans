using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform[] positions;
    [SerializeField] private RectTransform content;

    [SerializeField] private LevelListSO levelList;
    [SerializeField] private GameObject prefab;

   

    private void Start()
    {
        // Calculate the number of screens needed
        int numberOfScreens = Mathf.CeilToInt(levelList.levels.Count / positions.Length)-1;
        float height = content.rect.height;
        // Calculate the new height for the content based on the number of screens
        float newHeight = numberOfScreens* height;
        newHeight += (levelList.levels.Count % positions.Length)*(height/positions.Length);

        // Set the new height to the content's RectTransform
        content.offsetMax = new Vector2(content.offsetMax.x, newHeight);

        float offset=0;

        // Loop through levels and instantiate prefabs (example usage)
        for (int i = 0; i < levelList.levels.Count;)
        {
            // Example: Instantiate prefab at each position (adjust logic as needed)
            InstantiatePrefabAtPosition(i++,offset);
            if (i % positions.Length == 0)
            {
                offset += height;
            }

        }
    }

    private void InstantiatePrefabAtPosition(int index,float offset)
    {

        RectTransform targetPosition = positions[index%positions.Length];
        Vector2 anchoredPosition = targetPosition.anchoredPosition;

        GameObject instantiatedPrefab = Instantiate(prefab, anchoredPosition , Quaternion.identity, content);
        RectTransform instantiatedRect = instantiatedPrefab.GetComponent<RectTransform>();
        instantiatedRect.anchoredPosition = anchoredPosition;
        instantiatedRect.anchoredPosition += new Vector2(0,offset);
        TMP_Text[] textComponents = instantiatedPrefab.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text textComponent in textComponents)
        {
            if (textComponent.name == "LevelText")
            {
                textComponent.text = "Level " + (index+1);
            }
            else if (textComponent.name == "LevelDescription")
            {
                textComponent.text = "Difficulty \n";
                for(int i=0; i<levelList.levels[index].difficulty;i++)
                {
                    textComponent.text += "<sprite index=0>";
                }
            }
            
        }
        Button button=instantiatedRect.GetComponent<Button>();
        button.onClick.AddListener(() => LoadLevel(index+1));
        

    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 1 || levelIndex > levelList.levels.Count + 1)
        {
            Debug.LogWarning("Level does not exist");
            return;
        }
        SelectedLevel.SetSelectedLevel(levelIndex - 1);
        SceneManager.LoadScene(Scenes.GameScene);
    }
}


