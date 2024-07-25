using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    [SerializeField] private Transform menu;
    [SerializeField] private Transform levelSelect;
    [SerializeField] private Transform options;





    public void OnPlayClick() {
        menu.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(true);
    }

    public void OnOptionsClick() {
        menu.gameObject.SetActive(false);
        options.gameObject.SetActive(true);
    }

    public void OnBackClick() {
        levelSelect.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }

    public void OnExitClick() {
        Application.Quit();
    }




}

