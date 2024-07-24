using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI fpsText;


    // Update is called once per frame
    void Update() {
        fpsText.text = "FPS: " + (1.0f / Time.unscaledDeltaTime).ToString("F0");
    }
}
