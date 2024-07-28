using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    private enum Stage {
        SetOrigin,
        SetEnd,
        Shoot,
        SetOrigin2,
        SetEnd2,
        Shoot2,
        SetOrigin3,
        SetEnd3,
        Shoot3,
        Done
    }

    private Player player;
    private Stage stage;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 origin1;
    [SerializeField] private Vector2 end1;
    [SerializeField] private Vector2 origin2;
    [SerializeField] private Vector2 end2;
    [SerializeField] private Vector2 origin3;
    [SerializeField] private Vector2 end3;
    [SerializeField] private float shoot1Duration;
    [SerializeField] private float shoot2Duration;
    [SerializeField] private float shoot3Duration;
    [SerializeField] private float touchDistance = 30f;
    [SerializeField] private Image originImage;
    [SerializeField] private Image endImage;
    private float shootTimer = 0f;
    void Start() {
        stage = Stage.SetOrigin;
        player = Player.Instance;
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

        origin1 = new Vector2(Screen.width / 2f, Screen.height - (Screen.height / 3f));
        end1 = new Vector2(Screen.width / 2f, Screen.height / 3f);
        origin2 = origin3 = origin1;
        end2 = end3 = end1;
        Vector2 originPosition = ScreenToCanvasPoint(origin1, canvasScaler);
        Vector2 endPosition = ScreenToCanvasPoint(end1, canvasScaler);



        // Set the localPosition of the RectTransforms
        originImage.rectTransform.localPosition = originPosition;
        endImage.rectTransform.localPosition = endPosition;

        originImage.rectTransform.sizeDelta = new Vector2(touchDistance, touchDistance);
        endImage.rectTransform.sizeDelta = new Vector2(touchDistance, touchDistance);

    }

    Vector2 ScreenToCanvasPoint(Vector2 screenPoint, CanvasScaler canvasScaler) {
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
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Vector2.Distance(Input.GetTouch(0).position, origin1) < touchDistance) {
                    stage = Stage.SetEnd;
                    player.StartAiming(Input.GetTouch(0).position);
                }
                break;
            case Stage.SetEnd:
                if (Input.touchCount > 0) {
                    if (Vector2.Distance(Input.GetTouch(0).position, end1) < touchDistance) {
                        stage = Stage.Shoot;
                        shootTimer = shoot1Duration;
                        player.StartLaunching();
                    }
                    else {
                        player.SetAimVector(Input.GetTouch(0).position);
                    }
                }
                break;
            case Stage.Shoot:
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0f) {
                    stage = Stage.SetOrigin2;
                    shootTimer = 0f;
                }
                break;
            case Stage.SetOrigin2:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Vector2.Distance(Input.GetTouch(0).position, origin2) < touchDistance) {
                    stage = Stage.SetEnd2;
                    player.StartAiming(Input.GetTouch(0).position);
                }
                break;
            case Stage.SetEnd2:
                if (Input.touchCount > 0) {
                    if (Vector2.Distance(Input.GetTouch(0).position, end2) < touchDistance) {
                        stage = Stage.Shoot2;
                        shootTimer = shoot2Duration;
                        player.StartLaunching();
                    }
                    else {
                        player.SetAimVector(Input.GetTouch(0).position);
                    }
                }
                break;
            case Stage.Shoot2:
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0f) {
                    stage = Stage.SetOrigin3;
                    shootTimer = 0f;
                }
                break;
            case Stage.SetOrigin3:
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Vector2.Distance(Input.GetTouch(0).position, origin3) < touchDistance) {
                    stage = Stage.SetEnd3;
                    player.StartAiming(Input.GetTouch(0).position);
                }
                break;
            case Stage.SetEnd3:
                if (Input.touchCount > 0) {
                    if (Vector2.Distance(Input.GetTouch(0).position, end3) < touchDistance) {
                        stage = Stage.Shoot3;
                        shootTimer = shoot3Duration;
                        player.StartLaunching();
                    }
                    else {
                        player.SetAimVector(Input.GetTouch(0).position);
                    }
                }
                break;
            case Stage.Shoot3:
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0f) {
                    stage = Stage.Done;
                    shootTimer = 0f;
                }
                break;
            default:
                break;
        }
        if (Input.touchCount > 0) {
            Debug.Log(Input.GetTouch(0).position);
        }
    }
}
