using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform position1;
    [SerializeField] private RectTransform position2;
    [SerializeField] private RectTransform content;

    [SerializeField] private LevelListSO levelList;
    [SerializeField] private GameObject prefab;

    private void Start()
    {
        // Calculate the number of screens needed
        int numberOfScreens = Mathf.CeilToInt(levelList.levels.Count / 2f)-1;
        float height = content.rect.height;
        // Calculate the new height for the content based on the number of screens
        float newHeight = numberOfScreens* height;

        // Set the new height to the content's RectTransform
        content.offsetMax = new Vector2(content.offsetMax.x, newHeight);

        float offset=0;

        // Loop through levels and instantiate prefabs (example usage)
        for (int i = 0; i < levelList.levels.Count;)
        {
            // Example: Instantiate prefab at each position (adjust logic as needed)
            InstantiatePrefabAtPosition(i++,offset);
            InstantiatePrefabAtPosition(i++, offset);
            offset += height;

        }
    }

    private void InstantiatePrefabAtPosition(int index,float offset)
    {
        RectTransform targetPosition = (index % 2 == 0) ? position1 : position2;
        Vector2 anchoredPosition = targetPosition.anchoredPosition;

        GameObject instantiatedPrefab = Instantiate(prefab, anchoredPosition , Quaternion.identity, content);
        RectTransform instantiatedRect = instantiatedPrefab.GetComponent<RectTransform>();
        instantiatedRect.anchoredPosition = anchoredPosition;
        instantiatedRect.anchoredPosition += new Vector2(0,offset);
    }

 
}
