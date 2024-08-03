using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDownTween : MonoBehaviour {
    [SerializeField] private float finalY = -20;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private Ease ease;
    void Start() {
        transform.GetComponent<RectTransform>().DOMoveY(finalY, duration).SetDelay(delay).SetEase(ease);
    }

}
