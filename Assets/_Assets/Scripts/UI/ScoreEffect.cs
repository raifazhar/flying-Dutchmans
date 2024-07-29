using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreEffect : MonoBehaviour {

    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve yTransformCurve;
    [SerializeField] private AnimationCurve alphaCurve;
    [SerializeField] private float scaleMagnitude = 1f;
    [SerializeField] private float transformMagnitude = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private TextMeshProUGUI text;

    private Vector3 positionOriginal;
    private float time = 0f;
    private float scaleOriginal;

    private void Start() {
        scaleOriginal = transform.localScale.x;
        positionOriginal = transform.localPosition;
    }

    private void Update() {
        time += Time.deltaTime * speed;
        if (time > 1f)
            time = 1f;
        float scale = scaleCurve.Evaluate(time);
        transform.localScale =
            new Vector3(scaleOriginal + (scale * scaleMagnitude),
            scaleOriginal + scale,
            scaleOriginal + scale);
        transform.localPosition = new Vector3(
            positionOriginal.x,
            positionOriginal.y + (yTransformCurve.Evaluate(time) * transformMagnitude),
            positionOriginal.z);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alphaCurve.Evaluate(time));
    }


    public void SetScore(int score) {
        text.text = score.ToString();
    }
}
