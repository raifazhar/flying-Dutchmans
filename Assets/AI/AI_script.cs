using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_script : MonoBehaviour
{
    //First,  we describe the scene to AI . Alot of initial values.

    public  int maxDistance = 10; //Along Z - axis
    public int maxHeight = 10;  //Along Y - axis
    public int PlayerHP;
    public int hpSelf;
    public double radius = 0.4; // Along X- axis , radius of Base
    public int posx, posy;
    public struct target
    {
        int x;
        int y;
        int z;
    }
    public Image reticle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pos = reticle.rectTransform.localPosition;
        reticle.rectTransform.localPosition = new Vector3(pos.x+1, pos.y, pos.z);
        //Physics.Raycast(Camera)
    }
}
