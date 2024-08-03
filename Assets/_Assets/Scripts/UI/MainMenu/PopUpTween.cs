using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTween : MonoBehaviour {
    [SerializeField] private float startY = -20;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 0f;
    [SerializeField]
    private Ease ease;
    private Vector2 finalPos;
    private Vector2 startPos;
    void Start() {
        finalPos = transform.GetComponent<RectTransform>().position;
        transform.GetComponent<RectTransform>().position = new Vector2(finalPos.x, startY);
        transform.GetComponent<RectTransform>().DOMoveY(finalPos.y, duration).SetDelay(delay).SetEase(ease);
    }


}
