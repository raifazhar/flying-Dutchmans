using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    private enum Stage {
        SetOrigin,
        SetEnd,
        ShootAtForceField,
        SetOrigin2,
        SetEnd2,
        ShootAtCrate,
        SetOrigin3,
        SetEnd3,
        ShootAtProjectile,
        SetOrigin4,
        SetEnd4,
        ShootAtCrate2,
        Done
    }

    private Player player;
    private Stage stage;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform tutorialPanel;
    [SerializeField] private Transform tutorialWin;
    [SerializeField] private PointingArrowUI arrow1;
    [SerializeField] private MovingArrowUI arrow2;
    private Vector2 origin;
    private Vector2 end1;
    private Vector2 end2;
    private Vector2 end3;
    private Vector2 end4;
    [SerializeField] private float shoot1Duration;
    [SerializeField] private float shoot2Duration;
    [SerializeField] private float shoot3Duration;
    [SerializeField] private float shoot4Duration;
    [SerializeField] private float touchDistance = 30f;
    [SerializeField] private Image originImage;
    [SerializeField] private Image endImage;
    [SerializeField] private Transform obstacleSpawnPosition;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private ProjectileLauncher projectileLauncher;
    private bool enemyProjectileInPosition = false;
    private CanvasScaler canvasScaler;
    private float shootTimer = 0f;
    void Start() {
        stage = Stage.SetOrigin;
        tutorialPanel.gameObject.SetActive(true);
        tutorialWin.gameObject.SetActive(false);
        player = Player.Instance;
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        origin = new Vector2(Screen.width / 2f, Screen.height - (Screen.height / 3f));
        end1 = new Vector2(Screen.width / 2f, Screen.height / 3f);
        end3 = end4 = end2 = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 originPosition = ScreenToCanvasPoint(origin);
        Vector2 endPosition = ScreenToCanvasPoint(end1);
        arrow1.SetPosition(originPosition);
        arrow2.SetStartPos(originPosition);
        arrow2.SetEndPos(endPosition);
        arrow2.ResetLerp();
        arrow2.gameObject.SetActive(false);
        SetImages(origin, end1);
        projectileLauncher.ProjectileInPosition += ProjectileLauncher_ProjectileInPosition; ;
    }

    private void ProjectileLauncher_ProjectileInPosition(object sender, System.EventArgs e) {
        enemyProjectileInPosition = true;
    }

    Vector2 ScreenToCanvasPoint(Vector2 screenPoint) {
        // Get the canvas rect
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Convert the screen point to a point relative to the canvas
        Vector2 viewportPoint = new Vector2(screenPoint.x / Screen.width, screenPoint.y / Screen.height);
        Vector2 worldPoint = new Vector2(viewportPoint.x * canvasRect.sizeDelta.x, viewportPoint.y * canvasRect.sizeDelta.y);

        return worldPoint - (canvasRect.sizeDelta / 2f);
    }
    // Update is called once per frame
    void Update() {

        switch (stage) {
            case Stage.SetOrigin:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Vector2.Distance(Input.GetTouch(0).position, origin) < touchDistance) {
                    stage = Stage.SetEnd;
                    arrow1.gameObject.SetActive(false);
                    arrow2.gameObject.SetActive(true);
                    player.StartAiming(Input.GetTouch(0).position);
                }
                break;
            case Stage.SetEnd:
                if (Input.touchCount > 0) {
                    if (Vector2.Distance(Input.GetTouch(0).position, end1) < touchDistance) {
                        stage = Stage.ShootAtForceField;
                        shootTimer = shoot1Duration;
                        player.StartLaunching();
                        DisableImages();
                    }
                    else {
                        player.SetAimVector(Input.GetTouch(0).position);
                    }
                }
                break;
            case Stage.ShootAtForceField:
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0f) {
                    stage = Stage.SetOrigin2;
                    SetImages(origin, end2);
                    arrow1.SetPosition(ScreenToCanvasPoint(origin));
                    arrow1.ResetLerp();
                    arrow1.gameObject.SetActive(true);
                    shootTimer = 0f;
                    Transform obstacle = Instantiate(obstaclePrefab, obstacleSpawnPosition.position, Quaternion.identity).transform;
                    obstacle.GetComponent<Obstacle>().SetFallSpeed(0);
                }
                break;
            case Stage.SetOrigin2:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Vector2.Distance(Input.GetTouch(0).position, origin) < touchDistance) {
                    stage = Stage.SetEnd2;
                    arrow1.gameObject.SetActive(false);
                    arrow2.gameObject.SetActive(true);
                    arrow2.SetStartPos(ScreenToCanvasPoint(origin));
                    arrow2.SetEndPos(ScreenToCanvasPoint(end2));
                    arrow2.ResetLerp();
                    player.StartAiming(Input.GetTouch(0).position);
                }
                break;
            case Stage.SetEnd2:
                if (Input.touchCount > 0) {
                    if (Vector2.Distance(Input.GetTouch(0).position, end2) < touchDistance) {
                        stage = Stage.ShootAtCrate;
                        shootTimer = shoot2Duration;
                        player.StartLaunching();
                        DisableImages();
                    }
                    else {
                        player.SetAimVector(Input.GetTouch(0).position);
                    }
                }
                break;
            case Stage.ShootAtCrate:
                if (shootTimer > 0f)
                    shootTimer -= Time.deltaTime;
                if (shootTimer < 0f) {
                    projectileLauncher.LaunchProjectile();
                    shootTimer = 0f;
                }
                if (enemyProjectileInPosition) {
                    stage = Stage.SetOrigin3;
                    SetImages(origin, end3);
                    arrow1.SetPosition(ScreenToCanvasPoint(origin));
                    arrow1.gameObject.SetActive(true);
                    shootTimer = 0f;
                }
                break;
            case Stage.SetOrigin3:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Vector2.Distance(Input.GetTouch(0).position, origin) < touchDistance) {
                    stage = Stage.SetEnd3;
                    arrow1.gameObject.SetActive(false);
                    arrow2.gameObject.SetActive(true);
                    arrow2.SetStartPos(ScreenToCanvasPoint(origin));
                    arrow2.SetEndPos(ScreenToCanvasPoint(end3));
                    arrow2.ResetLerp();

                    player.StartAiming(Input.GetTouch(0).position);
                }
                break;
            case Stage.SetEnd3:
                if (Input.touchCount > 0) {
                    if (Vector2.Distance(Input.GetTouch(0).position, end3) < touchDistance) {
                        stage = Stage.ShootAtProjectile;
                        shootTimer = shoot3Duration;
                        player.StartLaunching();
                        projectileLauncher.ResumeProjectile();
                        DisableImages();
                    }
                    else {
                        player.SetAimVector(Input.GetTouch(0).position);
                    }
                }
                break;
            case Stage.ShootAtProjectile:
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0f) {
                    stage = Stage.SetOrigin4;
                    SetImages(origin, end4);
                    arrow1.SetPosition(ScreenToCanvasPoint(origin));
                    arrow1.gameObject.SetActive(true);
                    shootTimer = 0f;
                    Transform obstacle = Instantiate(obstaclePrefab, obstacleSpawnPosition.position, Quaternion.identity).transform;
                    obstacle.GetComponent<Obstacle>().SetFallSpeed(0);

                }
                break;
            case Stage.SetOrigin4:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Vector2.Distance(Input.GetTouch(0).position, origin) < touchDistance) {
                    stage = Stage.SetEnd4;
                    arrow1.gameObject.SetActive(false);
                    arrow2.gameObject.SetActive(true);
                    arrow2.SetStartPos(ScreenToCanvasPoint(origin));
                    arrow2.SetEndPos(ScreenToCanvasPoint(end4));
                    arrow2.ResetLerp();

                    player.StartAiming(Input.GetTouch(0).position);
                }
                break;
            case Stage.SetEnd4:
                if (Input.touchCount > 0) {
                    if (Vector2.Distance(Input.GetTouch(0).position, end4) < touchDistance) {
                        stage = Stage.ShootAtCrate2;
                        shootTimer = shoot4Duration;
                        player.StartLaunching();
                        DisableImages();
                    }
                    else {
                        player.SetAimVector(Input.GetTouch(0).position);
                    }
                }
                break;
            case Stage.ShootAtCrate2:
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0f) {
                    stage = Stage.Done;
                    ShowTutorialEnd();
                    shootTimer = 0f;
                }
                break;
            default:
                break;
        }
    }

    private void SetImages(Vector2 start, Vector2 end) {
        Vector2 originPosition = ScreenToCanvasPoint(start);
        Vector2 endPosition = ScreenToCanvasPoint(end);
        originImage.rectTransform.localPosition = originPosition;
        endImage.rectTransform.localPosition = endPosition;
        originImage.rectTransform.sizeDelta = new Vector2(touchDistance, touchDistance);
        endImage.rectTransform.sizeDelta = new Vector2(touchDistance, touchDistance);
        EnableImages();
    }


    private void EnableImages() {
        originImage.gameObject.SetActive(true);
        endImage.gameObject.SetActive(true);
    }
    private void DisableImages() {
        originImage.gameObject.SetActive(false);
        endImage.gameObject.SetActive(false);
        arrow1.gameObject.SetActive(false);
        arrow2.gameObject.SetActive(false);
    }

    public void GotoFirstLevel() {
        SelectedLevel.SetSelectedLevel(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(Scenes.GameScene);
    }

    public void ShowTutorialEnd() {
        PlayerPrefs.SetInt(PlayerPrefVariables.TutorialDone, 1);
        tutorialPanel.gameObject.SetActive(false);
        tutorialWin.gameObject.SetActive(true);
    }
}
