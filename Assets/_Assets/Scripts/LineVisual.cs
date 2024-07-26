using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineVisual : MonoBehaviour {
    private bool isAiming = false;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform landingPointPrefab;
    private Transform landingPointObject;
    [SerializeField] private int resolution = 20;
    [SerializeField] private float timeStep = 0.1f;
    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private float rayTimeStep = 1f;
    [SerializeField] private int rayCount = 10;
    [SerializeField] private LayerMask landingPointCollisionLayers;
    Vector3[] points;
    // Start is called before the first frame update
    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        landingPointObject = Instantiate(landingPointPrefab);
        lineRenderer.enabled = false;
        landingPointObject.gameObject.SetActive(false);
        points = new Vector3[resolution];
        lineRenderer.positionCount = resolution;
        Player.Instance.OnStateChange += Player_OnStateChange;
    }

    private void Player_OnStateChange(object sender, Player.OnStateChangeEventArgs e) {
        if (e.playerState == Player.State.Aiming) {
            isAiming = true;
            RenderLineRenderer();
            RenderLandingPoint();
            lineRenderer.enabled = true;
        }
        else {
            isAiming = false;
            lineRenderer.enabled = false;
            landingPointObject.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (isAiming) {
            RenderLineRenderer();
            RenderLandingPoint();
        }
    }

    private void RenderLineRenderer() {
        Vector3 launchVector = Player.Instance.GetLaunchVector();
        Vector3 launchOrigin = Player.Instance.GetLaunchOrigin();

        float gravity = Physics.gravity.y;

        for (int i = 0; i < resolution; i++) {
            float t = i * timeStep;
            // Calculate the position at time t
            float x = launchOrigin.x + launchVector.x * t;
            float y = launchOrigin.y + launchVector.y * t + 0.5f * gravity * t * t;
            float z = launchOrigin.z + launchVector.z * t;
            points[i] = new Vector3(x, y, z);
        }

        lineRenderer.SetPositions(points);
    }

    private void RenderLandingPoint() {
        Vector3 launchVector = Player.Instance.GetLaunchVector();
        Vector3 launchOrigin = Player.Instance.GetLaunchOrigin();
        //Do a series of raycasts approximating projectile motion
        //If we hit anything then render the landing point at the hit point
        float gravity = Physics.gravity.y;
        Vector3 oldOrigin = launchOrigin;
        Vector3 origin = Vector3.zero;
        for (int i = 0; i < rayCount; i++) {

            float t = i * rayTimeStep;
            origin.x = launchOrigin.x + launchVector.x * t;
            origin.y = launchOrigin.y + launchVector.y * t + 0.5f * gravity * t * t;
            origin.z = launchOrigin.z + launchVector.z * t;
            Vector3 direction = origin - oldOrigin;
            direction.Normalize();
            RaycastHit hit;
            if (Physics.SphereCast(oldOrigin, 1f, direction, out hit, rayDistance, landingPointCollisionLayers)) {
                if (!landingPointObject.gameObject.activeSelf)
                    landingPointObject.gameObject.SetActive(true);
                landingPointObject.position = hit.point;
                break;
            }
            oldOrigin = origin;
        }
    }


}
