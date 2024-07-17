using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulateTheUI : MonoBehaviour
{
    [SerializeField] private ListOfBlocks listOfBlocks;  // Reference to your ScriptableObject
    [SerializeField] private GameObject uiImagePrefab;  // Reference to your UI Image prefab
    [SerializeField] private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < listOfBlocks.materials.Count; i++)
        {
            // Instantiate the UI Image prefab
            GameObject uiImageObject = Instantiate(uiImagePrefab, transform);

            // Set the name of the instantiated object
            uiImageObject.name = listOfBlocks.materials[i].name;

            // Get the Image component and set the sprite
            Image image = uiImageObject.GetComponent<Image>();
            image.sprite = listOfBlocks.materials[i].image;

            // Get the TextMeshPro component in the child object and set the text
            TextMeshProUGUI textMeshPro = uiImageObject.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textMeshPro.text = listOfBlocks.materials[i].count.ToString();
            }
        }
    }
}
