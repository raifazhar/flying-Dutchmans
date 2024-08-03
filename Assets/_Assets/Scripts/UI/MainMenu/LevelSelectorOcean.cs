using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorOcean : MonoBehaviour {
    [SerializeField] private LevelSelector _levelSelector;
    [SerializeField] private Material _2DOceanMaterial;
    [SerializeField] private float verticalMultiplier = 1f;
    private readonly string VERTICAL_OFFSET_ATTRIBUTE = "_VerticalOffset";

    // Update is called once per frame
    void Update() {
        _2DOceanMaterial.SetFloat(VERTICAL_OFFSET_ATTRIBUTE, _levelSelector.GetVerticalOffset() * verticalMultiplier);
    }
}
