using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchCooldownUI : MonoBehaviour
{

    [SerializeField] private Image launchBar;
   

    // Update is called once per frame
    void Update()
    {
        launchBar.fillAmount = 1 - Player.Instance.GetLaunchTimerNormalized();     
    }
}
