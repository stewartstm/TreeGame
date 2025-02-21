using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision_detect : MonoBehaviour
{

    public static GameObject tool;

    //For Cherry Picker
    //public static bool hitObject;


    void Update()
    {
        //hitObject = false;
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.layer == 6)
        {
            //hitObject = true;
        }
    }

    void OnTriggerStay(Collider hit)
    {
        if (hit.gameObject.layer == 8 || hit.gameObject.layer == 9)
        {
            tool = hit.gameObject;
        }

        if (hit.gameObject.layer == 6)
        {
            //hitObject = true;
        }
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.layer == 8 || hit.gameObject.layer == 9)
        {
            tool = null;
        }

        if (hit.gameObject.layer == 6)
        {
            //hitObject = false;
        }
    }
}
