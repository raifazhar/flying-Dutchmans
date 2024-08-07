using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenToStartingLocation : MonoBehaviour {
    [SerializeField] private float delay = 0f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Ease ease;
    [SerializeField] private bool setUpdate = false;
    private Vector3 startingLocation;
    private Vector3 offsetLocation;
    private bool initialized = false;
    private void OnEnable() {
        InitializeAndTween();
    }

    private void InitializeAndTween() {
        if (!initialized) {
            initialized = true;
            startingLocation = gameObject.GetComponent<RectTransform>().anchoredPosition;
            offsetLocation = startingLocation + offset;
        }
        gameObject.GetComponent<RectTransform>().anchoredPosition = offsetLocation;
        gameObject.GetComponent<RectTransform>().DOAnchorPos(startingLocation, duration).SetDelay(delay).SetEase(ease).SetUpdate(setUpdate);
    }
}
