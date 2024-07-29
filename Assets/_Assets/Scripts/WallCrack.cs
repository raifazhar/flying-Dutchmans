using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCrack : MonoBehaviour {

    [SerializeField] private Texture[] images;
    [SerializeField] private Material crackMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    void Start() {
        meshRenderer.material = Instantiate(crackMaterial);
        meshRenderer.material.mainTexture = images[Random.Range(0, images.Length)];
    }
}
