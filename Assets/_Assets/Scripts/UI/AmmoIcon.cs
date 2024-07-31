using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoIcon : MonoBehaviour {
    [SerializeField] private Image filled;
    [SerializeField] private Image empty;


    public void SetFilled() {
        filled.gameObject.SetActive(true);
        empty.gameObject.SetActive(false);
    }

    public void SetEmpty() {
        filled.gameObject.SetActive(false);
        empty.gameObject.SetActive(true);
    }
}
