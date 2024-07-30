using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEffect : MonoBehaviour {

    public enum TextColor {
        Green,
        Blue
    }
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve yTransformCurve;
    [SerializeField] private AnimationCurve alphaCurve;
    [SerializeField] private float transformMagnitude = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color[] textColors;
    private Vector3 positionOriginal;
    private float time = 0f;
    private float scaleOriginal = 1f;

    private void Start() {
        positionOriginal = transform.localPosition;
    }

    private void Update() {
        time += Time.deltaTime * speed;
        if (time > 1f) {
            Destroy(gameObject);
        }
        float scale = scaleCurve.Evaluate(time);
        scale *= scaleOriginal;
        transform.localScale =
            new Vector3(scaleOriginal + scale,
            scaleOriginal + scale,
            scaleOriginal + scale);
        transform.localPosition = new Vector3(
            positionOriginal.x,
            positionOriginal.y + (yTransformCurve.Evaluate(time) * transformMagnitude),
            positionOriginal.z);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alphaCurve.Evaluate(time));
    }

    public void SetScale(float s) {
        scaleOriginal = s;
    }

    public void SetSpeed(float s) {
        speed = s;
    }

    public void SetColor(TextColor textColor) {
        switch (textColor) {
            case TextColor.Green:
                text.color = textColors[0];
                break;
            case TextColor.Blue:
                text.color = textColors[1];
                break;
            default:
                break;
        }
    }

    public void SetText(string t) {
        text.text = t;
    }
}
