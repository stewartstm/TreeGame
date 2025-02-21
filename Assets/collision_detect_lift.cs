using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision_detect_lift : MonoBehaviour
{
    public static bool hitObject;

    public float influence1X;
    public float influence1Y;
    //public static float influence2X;
    public float influence2Y;

    public static Vector3 influenceOnCollision;


    void Update()
    {
        influence1X = character_controller.inputVector.x;
        influence1Y = character_controller.inputVector.y;
        //influence2X = character_controller.inputVector2.x;
        influence2Y = character_controller.inputVector2.y;
    }

    void OnTriggerEnter(Collider hit)
    {
        
        if (hit.gameObject.layer == 6)
        {
            lift_controls.collisionCall = true;
            hitObject = true;
            influenceOnCollision = new Vector3(influence1X, influence1Y, influence2Y);
            //Debug.Log("1X: " + influence1X + " 1Y: " + influence1Y + " 2X: " + influence2X + " 2Y: " + influence2Y);
            Debug.Log(influenceOnCollision);
        }
    }
    /*
        void OnTriggerStay(Collider hit)
        {
            if (hit.gameObject.layer == 6)
            {
                hitObject = true;
            }
        }
    */
    void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.layer == 6)
        {
            hitObject = false;
        }
    }

}
