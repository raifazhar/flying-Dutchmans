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
    public GameObject reticle;

    // Private Serialized fields

    [SerializeField] private Transform launchOrigin;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float touchSensibility = 1f;
    [SerializeField] private float launchVectorMax;
    [SerializeField] private float launchVectorMin;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float maxLaunchAngle;
    [SerializeField] private float minLaunchAngle;
    private Vector2 touchOrigin;
    private Vector2 touchOriginWorldSpace;
    private Vector2 touchPoint;
    private Vector2 touchPointWorldSpace;
    private Vector3 launchVector;
    



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //var pos = reticle.Transform.position;
        //reticle.rectTransform.localPosition = new Vector3(pos.x+1, pos.y, pos.z);
        //Physics.Raycast(Camera)
    }
    private void SetAimVector()
    {
        //touchPoint = reticle.rectTransform.localPosition;
        
        touchPointWorldSpace = Camera.main.ScreenToWorldPoint(touchPoint);
        launchVector = touchPointWorldSpace - touchOriginWorldSpace; // dont know
        launchVector *= touchSensibility; // dont know
        launchVector *= -1; //dont know
        if (launchVector.sqrMagnitude > launchVectorMax * launchVectorMax)
        {
            launchVector = launchVector.normalized * launchVectorMax; // why?
        }
        else if (launchVector.sqrMagnitude < launchVectorMin * launchVectorMin)
        {
            launchVector = launchVector.normalized * launchVectorMin;
        }
        float angle = Vector2.SignedAngle(Vector2.right, launchVector);

        if (angle > maxLaunchAngle)
        {
            launchVector = Quaternion.Euler(0, 0, maxLaunchAngle) * Vector3.right * launchVector.magnitude;
        }
        else if (angle < minLaunchAngle)
        {
            launchVector = Quaternion.Euler(0, 0, minLaunchAngle) * Vector3.right * launchVector.magnitude;
        }
        launchVector *= launchSpeed;
    }
    private void LaunchProjectile()
    {
        Instantiate(projectilePrefab, launchOrigin.position, Quaternion.identity).GetComponent<Rigidbody>().velocity = launchVector;
    }

}
