using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineVisual : MonoBehaviour
{
    private bool isAiming=false;
    private LineRenderer lineRenderer;
    [SerializeField] private int resolution=20;
    [SerializeField] private float timeStep=0.1f;
    Vector3[] points;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        points=new Vector3[resolution];
        lineRenderer.positionCount = resolution;
        Player.Instance.OnStateChange += Player_OnStateChange;
    }

    private void Player_OnStateChange(object sender, Player.OnStateChangeEventArgs e) {
        if (e.playerState == Player.State.Aiming) {
            isAiming = true;
            CalculatePoints();
            lineRenderer.enabled = true;
        }
        else {
            isAiming = false;
            lineRenderer.enabled = false;
        }
    }

    private void Update() {
        if (isAiming) {
            CalculatePoints();
        }
    }

    private void CalculatePoints() {
        Vector3 launchVector = Player.Instance.GetLaunchVector();
        Vector3 launchOrigin= Player.Instance.GetLaunchOrigin();

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


}
