using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour {
    public static LevelSelector Instance { get; private set; }

    [SerializeField] private float offsetLerpSpeed = 1f;
    [SerializeField] private float verticalMultiplier = 1f;
    [SerializeField] private LevelListSO levelsSO;
    [SerializeField] private Transform levelIconPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float verticalPadding = 2f;
    [SerializeField] private Mesh[] visualMeshes;
    [SerializeField] private Mesh[] rippleMeshes;

    private float effectiveVerticalOffset = 0f;
    private float inputVerticalOffset = 0f;
    private int targetFrameRate = 60;
    private Vector2 verticalBounds;
    private Transform[] activeIcons;
    private Vector2 prevPos;
    private Vector2 currPos;

    private float upperBound;
    private float lowerBound;
    private int maxCompletedLevel = 0;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        Screen.orientation = ScreenOrientation.Portrait;
        maxCompletedLevel = PlayerPrefs.GetInt(PlayerPrefVariables.MaxCompletedLevel, -1);
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
        HandleOffset();
        HandleIcons();
        gameObject.SetActive(false);
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
                inputVerticalOffset += ((currPos.y - prevPos.y) * verticalMultiplier * 0.01f);
                prevPos = currPos;
            }
        }
        inputVerticalOffset = Mathf.Clamp(inputVerticalOffset, verticalBounds.x, verticalBounds.y);
        effectiveVerticalOffset = Mathf.Lerp(effectiveVerticalOffset, inputVerticalOffset, Time.deltaTime * offsetLerpSpeed);
    }

    private void SetCameraFrustumBounds() {
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

    private void HandleIcons() {
        float upperBoundWorldSpace = upperBound - effectiveVerticalOffset;
        float lowerBoundWorldSpace = lowerBound - effectiveVerticalOffset;

        int firstVisibleIndex = Mathf.FloorToInt(lowerBoundWorldSpace / verticalPadding);
        int lastVisibleIndex = Mathf.FloorToInt(upperBoundWorldSpace / verticalPadding);

        firstVisibleIndex = Mathf.Clamp(firstVisibleIndex, 0, levelsSO.levels.Count - 1);
        lastVisibleIndex = Mathf.Clamp(lastVisibleIndex, 0, levelsSO.levels.Count - 1);

        // Deactivate out-of-bound icons
        for (int i = 0; i < poolSize; i++) {
            if (activeIcons[i].gameObject.activeSelf) {
                int index = activeIcons[i].GetComponent<LevelIcon>().GetLevelIndex();
                if (index < firstVisibleIndex || index > lastVisibleIndex) {
                    activeIcons[i].gameObject.SetActive(false);
                }
            }
        }

        // Activate or update in-bound icons
        for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++) {
            bool found = false;
            for (int j = 0; j < poolSize; j++) {
                if (activeIcons[j].gameObject.activeSelf && activeIcons[j].GetComponent<LevelIcon>().GetLevelIndex() == i) {
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

        // Update positions of active icons
        for (int i = 0; i < poolSize; i++) {
            if (activeIcons[i].gameObject.activeSelf) {
                int index = activeIcons[i].GetComponent<LevelIcon>().GetLevelIndex();
                activeIcons[i].position = new Vector3(activeIcons[i].position.x, activeIcons[i].position.y, index * verticalPadding + effectiveVerticalOffset);
            }
        }
    }

    private void SetIconState(int arrIndex, int levelIndex) {
        activeIcons[arrIndex].gameObject.SetActive(true);
        Random.InitState(levelIndex);

        float x = Random.Range(-1.7f, 1.7f);
        float rotation = Random.Range(0f, 360f);
        LevelIcon icon = activeIcons[arrIndex].GetComponent<LevelIcon>();
        if (levelIndex > maxCompletedLevel + 1) {
            icon.SetLocked(true);
        }
        else {
            icon.SetLocked(false);
        }
        int meshIndex = Random.Range(0, visualMeshes.Length);
        icon.SetVisualMesh(visualMeshes[meshIndex], rippleMeshes[meshIndex]);
        icon.SetVisualRotation(Quaternion.Euler(0, rotation, 0));
        icon.SetLevelIndex(levelIndex);
        activeIcons[arrIndex].position = new Vector3(x, 0, 0);
    }

    public float GetVerticalOffset() {
        return effectiveVerticalOffset;
    }

    public void LoadLevel(int levelIndex) {
        if (levelIndex < 0 || levelIndex > levelsSO.levels.Count) {
            Debug.LogWarning("Level does not exist");
            return;
        }
        MainMenu.Instance.LoadLevel(levelIndex);
    }
}
