using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAmmoUI : MonoBehaviour {
    [SerializeField] private Transform ammoTemplate;
    [SerializeField] private Transform parent;
    private AmmoIcon[] icons;
    bool initialized = false;
    private void Start() {
        Player.Instance.OnAmmoChange += Player_OnAmmoChange;
    }

    private void Player_OnAmmoChange(object sender, System.EventArgs e) {
        if (!initialized) {
            InitializeAmmo();
            initialized = true;
        }
        RefreshAmmo();
    }

    private void InitializeAmmo() {
        icons = new AmmoIcon[Player.Instance.GetMaxAmmo()];
        for (int i = 0; i < icons.Length; i++) {
            Transform ammo = Instantiate(ammoTemplate, parent);
            ammo.gameObject.SetActive(true);
            icons[i] = ammo.GetComponent<AmmoIcon>();
        }
    }

    private void RefreshAmmo() {
        int ammo = Player.Instance.GetRemainingAmmo();
        for (int i = 0; i < icons.Length; i++) {
            if (i < ammo) {
                icons[i].SetFilled();
            }
            else {
                icons[i].SetEmpty();
            }
        }
    }
}
