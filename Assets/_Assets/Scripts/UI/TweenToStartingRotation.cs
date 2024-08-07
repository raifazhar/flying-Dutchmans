using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenToStartingRotation : MonoBehaviour {
    [SerializeField] private float delay = 0f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Ease ease;
    [SerializeField] private bool setUpdate = false;
    private Quaternion startingRotation;
    private Quaternion offsetRotation;
    private bool initialized = false;
    private void OnEnable() {
        InitializeAndTween();
    }

    private void InitializeAndTween() {
        if (!initialized) {
            initialized = true;
            startingRotation = gameObject.GetComponent<RectTransform>().rotation;
            offsetRotation = Quaternion.Euler(offset);
        }
        gameObject.GetComponent<RectTransform>().rotation = offsetRotation;
        gameObject.GetComponent<RectTransform>().DORotate(startingRotation.eulerAngles, duration).SetDelay(delay).SetEase(ease).SetUpdate(setUpdate);

    }
}
