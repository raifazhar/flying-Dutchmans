using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScrollView : MonoBehaviour {
    [SerializeField] private RectTransform[] positions;
    [SerializeField] private RectTransform content;

    [SerializeField] private LevelListSO levelList;
    [SerializeField] private GameObject prefab;

    private GameObject previousePrefab;


    private void Start() {

        // Calculate the number of screens needed
        int numberOfScreens = Mathf.CeilToInt(levelList.levels.Count / positions.Length) - 1;
        float height = content.rect.height;
        // Calculate the new height for the content based on the number of screens
        float newHeight = numberOfScreens * height;
        newHeight += (levelList.levels.Count % positions.Length) * (height / positions.Length)+20;

        // Set the new height to the content's RectTransform
        content.offsetMax = new Vector2(content.offsetMax.x, newHeight);

        float offset = 0;

        // Loop through levels and instantiate prefabs (example usage)
        for (int i = 0; i < levelList.levels.Count;) {
            // Example: Instantiate prefab at each position (adjust logic as needed)
            InstantiatePrefabAtPosition(i++, offset);
            if (i % positions.Length == 0) {
                offset += height;
            }

        }
    }

    private void InstantiatePrefabAtPosition(int index, float offset) {

        RectTransform targetPosition = positions[index % positions.Length];
        Vector2 anchoredPosition = targetPosition.anchoredPosition;

        GameObject instantiatedPrefab = Instantiate(prefab, anchoredPosition, Quaternion.identity, content);
        RectTransform instantiatedRect = instantiatedPrefab.GetComponent<RectTransform>();
        instantiatedRect.anchoredPosition = anchoredPosition;
        instantiatedRect.anchoredPosition += new Vector2(0, offset);



        instantiatedPrefab.GetComponent<LevelButton>().CreatButton(index, levelList.levels[index].difficulty);

        if(previousePrefab!=null)
        {
            
        }

        previousePrefab = instantiatedPrefab;
    }

   

    
}


