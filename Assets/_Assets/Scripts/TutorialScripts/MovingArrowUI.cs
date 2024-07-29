using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingArrowUI : MonoBehaviour {
    [SerializeField] private Vector2 arrowOffset;
    [SerializeField] private Vector2 arrowSize;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;
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
        gameObject.GetComponent<RectTransform>().localPosition = Vector2.Lerp(startPos, endPos, t) + arrowOffset;
    }

    public void SetStartPos(Vector2 v) {
        startPos = v;
    }
    public void SetEndPos(Vector2 v) {
        endPos = v;
    }
    public void ResetLerp() {
        lerp = 0;
    }

}
