using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointingArrowUI : MonoBehaviour {
    [SerializeField] private Vector2 arrowOffset1;
    [SerializeField] private Vector2 arrowOffset2;
    [SerializeField] private Vector2 arrowSize;
    [SerializeField] private Vector2 pointingPosition;
    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private float arrowSpeed = 1f;
    private float lerp = 0;
    // Update is called once per frame
    void Update() {
        lerp += Time.deltaTime * arrowSpeed;
        if (lerp > 1) {
            lerp = 0;
        }
        float t = movementCurve.Evaluate(lerp);
        gameObject.GetComponent<RectTransform>().localPosition = Vector2.Lerp(pointingPosition, pointingPosition + arrowOffset2, t) + arrowOffset1;
    }

    public void SetPosition(Vector2 v) {
        pointingPosition = v;
    }
    public void ResetLerp() {
        lerp = 0;
    }

}
