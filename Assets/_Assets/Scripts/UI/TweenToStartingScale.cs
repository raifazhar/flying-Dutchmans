using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenToStartingScale : MonoBehaviour {
    [SerializeField] private float delay = 0f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Ease ease;
    [SerializeField] private bool setUpdate = false;

    private Vector3 startingScale;
    private Vector3 offsetScale;
    private bool initialized = false;
    private void OnEnable() {
        InitializeAndTween();
    }

    private void InitializeAndTween() {
        if (!initialized) {
            initialized = true;
            startingScale = gameObject.GetComponent<RectTransform>().localScale;
            offsetScale.x = startingScale.x * offset.x;
            offsetScale.y = startingScale.y * offset.y;
            offsetScale.z = startingScale.z * offset.z;
        }
        gameObject.GetComponent<RectTransform>().localScale = offsetScale;
        gameObject.GetComponent<RectTransform>().DOScale(startingScale, duration).SetDelay(delay).SetEase(ease).SetUpdate(setUpdate);
    }
}
