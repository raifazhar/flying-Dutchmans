using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrientationSwitcher : MonoBehaviour
{

    [SerializeField]private Canvas portrait;
    [SerializeField]private Canvas landscape;
    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.deviceOrientation);
        if (Input.deviceOrientation==DeviceOrientation.Portrait||Input.deviceOrientation==DeviceOrientation.PortraitUpsideDown) 
        {
            Debug.Log("portrait");
            landscape.enabled = false;
            portrait.enabled = true;
        }
        else
        {
            Debug.Log("landscape");
            portrait.enabled=false;
            landscape.enabled = true;
        }
    }
}
