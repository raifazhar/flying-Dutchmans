using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotForce : MonoBehaviour
{
    public float Force;
    public int velocity;
    public float acceleration;
    public float ElasticConstant=0.001F;
    public float displacedX;
    public float mass;
    public GameObject currentBullet;
    private Vector2 touchStartPosition, touchEndPosition;
    public Touch theTouch;
    public Vector3 ForceVector;
    // Start is called before the first frame update
    void Start()
    {
        acceleration = (-ElasticConstant * displacedX) / mass;
        Force = -ElasticConstant * displacedX;
        ForceVector = new Vector3(Force, 0, 0);
    }

    void throwObject()
    {
        
        currentBullet.GetComponent<Rigidbody>().AddForce(ForceVector, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().useGravity = true;
        
    }
    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey("r"))
        {
            Destroy(currentBullet);
            
        }
         
        if (Input.touchCount > 0)
        {
            
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == UnityEngine.TouchPhase.Began)
            {
                touchStartPosition = theTouch.position;
            }
            else if (theTouch.phase == UnityEngine.TouchPhase.Moved || theTouch.phase == UnityEngine.TouchPhase.Ended)
            {
                touchEndPosition = theTouch.position;

                float x = (touchEndPosition.x - touchStartPosition.x);
                float y = (touchEndPosition.y - touchStartPosition.y);

                if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0)
                {
                    //direction = "Tapped";
                }
                Vector3 Movement = Time.deltaTime / 10 * new Vector3(x, 0, 0);
                transform.Rotate(Movement);
                if (theTouch.phase == UnityEngine.TouchPhase.Ended)
                {
                    displacedX = x ;
                    Force = ElasticConstant * displacedX;
                    ForceVector = new Vector3(0, 0, Force);
                    
                    ForceVector = Quaternion.AngleAxis(x, Vector3.right)*ForceVector;
                    Instantiate(currentBullet);
                    throwObject();
                }
            }

        }
    }
}
