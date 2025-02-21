using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dumpster : MonoBehaviour
{
    void OnTriggerEnter(Collider hit)
    {
        if(hit.gameObject.tag == "WoodDebris")
        {
            score.DebrisRemaining -= 1f;
        }
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.tag == "WoodDebris")
        {
            score.DebrisRemaining += 1f;
        }
    }
}
