using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannonVisual : MonoBehaviour {

    [SerializeField] private EnemyCannon cannon;
    private Vector3 launchVector;
    private bool launched = false;
    // Start is called before the first frame update
    void Start() {
        launchVector = Vector3.zero;
        cannon.OnLaunch += Cannon_OnLaunch;
    }

    private void Cannon_OnLaunch(object sender, EnemyCannon.LaunchEventArgs e) {
        launched = true;
        launchVector = e.launchVector;
    }

    // Update is called once per frame
    void Update() {
        Debug.DrawRay(cannon.GetLaunchOrigin().position, launchVector, Color.red);

    }
}
