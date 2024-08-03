using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {
    public static LevelSelector Instance {
        get; private set;
    }
    [SerializeField]
    private float verticalOffset = 0f;
    [SerializeField] private float verticalMultiplier = 1f;
    [SerializeField] private LevelListSO levelsSO;
    [SerializeField] private Transform levelIconPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float verticalPadding = 2f;
    private Vector2 verticalBounds;
    private float[] activeIconCoords;


    //Set bounds
    private float upperBound;
    private float lowerBound;
    private Transform[] activeIcons;
    [SerializeField] private Mesh[] visualMeshes;
    [SerializeField] private Material meshMaterial;
    private Vector2 prevPos;
    private Vector2 currPos;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        activeIcons = new Transform[poolSize];
        for (int i = 0; i < poolSize; i++) {
            Transform icon = Instantiate(levelIconPrefab, transform);
            icon.gameObject.SetActive(false);
            activeIcons[i] = icon;
        }
        prevPos = Vector2.zero;
        currPos = Vector2.zero;
        verticalBounds.y = 0f;
        verticalBounds.x = -(levelsSO.levels.Count * verticalPadding) + (verticalPadding * 2f);
        SetCameraFrustumBounds();
    }


    private void Update() {
        HandleOffset();
        HandleIcons();
    }

    private void HandleOffset() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                prevPos = touch.position;
            }
            if (touch.phase == TouchPhase.Moved) {
                currPos = touch.position;
                verticalOffset += ((currPos.y - prevPos.y) * verticalMultiplier * 0.01f);
                prevPos = currPos;
            }
        }
        verticalOffset = Mathf.Clamp(verticalOffset, verticalBounds.x, verticalBounds.y);

    }


    private void SetCameraFrustumBounds() {
        //Do 2 raycasts, one at the top and one at the bottom of the screen, and get the z value of the hit point   
        Plane worldPlane = new Plane(Vector3.up, Vector3.zero);
        Ray topRay = Camera.main.ScreenPointToRay(new Vector3(0, Screen.height, 0));
        Ray bottomRay = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0));
        float topHit = 0f;
        float bottomHit = 0f;
        if (worldPlane.Raycast(topRay, out topHit)) {
            upperBound = topRay.GetPoint(topHit).z;
        }
        if (worldPlane.Raycast(bottomRay, out bottomHit)) {
            lowerBound = bottomRay.GetPoint(bottomHit).z;
        }

    }

    //This method is used for setting the state, things like the levelindex, x position, model etc of the activeIcon
    private void SetIconState(int arrIndex, int levelIndex) {
        activeIcons[arrIndex].gameObject.SetActive(true);
        Random.InitState(levelIndex);
        float x = Random.Range(-1.7f, 1.7f);
        float rotation = Random.Range(0f, 360f);
        activeIcons[arrIndex].position = new Vector3(x, 0, 0);
        activeIcons[arrIndex].GetComponent<LevelIcon>().SetVisualMesh(visualMeshes[Random.Range(0, visualMeshes.Length)]);
        activeIcons[arrIndex].GetComponent<LevelIcon>().SetVisualRotation(Quaternion.Euler(0, rotation, 0));
        activeIcons[arrIndex].GetComponent<LevelIcon>().SetMaterial(meshMaterial);
        activeIcons[arrIndex].GetComponent<LevelIcon>().SetLevelIndex(levelIndex);
    }
    private void HandleIcons() {
        //Step 1, check which icons should be inside the view frustum based on vertical offset
        float upperBoundWorldSpace = upperBound - verticalOffset;
        float lowerBoundWorldSpace = lowerBound - verticalOffset;
        //we can divide them by the vertical padding to get the index of the first and last icon that should be visible
        int firstVisibleIndex = Mathf.FloorToInt(lowerBoundWorldSpace / verticalPadding);
        int lastVisibleIndex = Mathf.FloorToInt(upperBoundWorldSpace / verticalPadding);

        if (firstVisibleIndex < 0) {
            firstVisibleIndex = 0;
        }
        if (lastVisibleIndex > levelsSO.levels.Count - 1) {
            lastVisibleIndex = levelsSO.levels.Count - 1;
        }
        //Check if any active icons fall out of this range and disable them
        for (int i = 0; i < poolSize; i++) {
            if (!activeIcons[i].gameObject.activeSelf) {
                continue;
            }
            int index = activeIcons[i].GetComponent<LevelIcon>().GetLevelIndex();
            if (index < firstVisibleIndex || index > lastVisibleIndex) {
                activeIcons[i].gameObject.SetActive(false);
            }
        }
        //Make a list of all indexes that should fall within range and if they are not active, 
        //make an inactive icon become active and set its index
        for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++) {
            bool found = false;
            for (int j = 0; j < poolSize; j++) {
                int index = activeIcons[j].GetComponent<LevelIcon>().GetLevelIndex();
                if (index == i) {
                    activeIcons[j].gameObject.SetActive(true);
                    found = true;
                    break;
                }
            }
            if (!found) {
                for (int j = 0; j < poolSize; j++) {
                    if (!activeIcons[j].gameObject.activeSelf) {
                        SetIconState(j, i);
                        break;
                    }
                }
            }
        }

        //Step 2, update the position of all active icons as per their index
        for (int i = 0; i < poolSize; i++) {
            if (activeIcons[i].gameObject.activeSelf) {
                int index = activeIcons[i].GetComponent<LevelIcon>().GetLevelIndex();
                activeIcons[i].position = new Vector3(activeIcons[i].position.x, activeIcons[i].position.y, index * verticalPadding + verticalOffset);
            }
        }


    }

    public float GetVerticalOffset() {
        return verticalOffset;
    }

    public void LoadLevel(int levelIndex) {
        if (levelIndex < 0 || levelIndex > levelsSO.levels.Count) {
            Debug.LogWarning("Level does not exist");
            return;
        }
        MainMenu.Instance.LoadLevel(levelIndex);
    }
}
