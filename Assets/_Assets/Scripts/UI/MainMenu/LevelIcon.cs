using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelIcon : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private MeshFilter islandMeshFilter;
    [SerializeField] private MeshFilter rippleMeshFilter;
    [SerializeField] private Transform visualMesh;
    [SerializeField] private Canvas canvas;
    private int levelIndex = -1;

    private void Start() {
        canvas.worldCamera = Camera.main;
    }
    public void SetLevelIndex(int i) {
        levelIndex = i;
        levelText.text = (i + 1).ToString();
    }

    public int GetLevelIndex() {
        return levelIndex;
    }

    public void SetVisualMesh(Mesh mesh1, Mesh mesh2) {
        islandMeshFilter.mesh = mesh1;
        rippleMeshFilter.mesh = mesh2;
    }

    public void SetVisualRotation(Quaternion rotation) {
        visualMesh.transform.rotation = rotation;
    }


    public void LoadLevel() {
        LevelSelector.Instance.LoadLevel(levelIndex);
    }
}
