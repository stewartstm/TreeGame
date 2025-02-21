using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class project_along_object : MonoBehaviour
{

    public Vector3 startPoint; // Start of the sliding path
    public Vector3 endPoint;   // End of the sliding path
    public Transform slideObj;
    public Transform objectC;  // The influencing object

    void Start()
    {
        objectC = GameObject.Find("slice_plane_offset").transform;
    }

    void Update()
    {
        if (slideObj == null)
        {
            Destroy(gameObject);
            return;
        }



        Renderer rend = slideObj.GetComponent<Renderer>();
        startPoint = rend.bounds.min;
        endPoint = rend.bounds.max;
        
        /*
        Vector3 startPointAdj = new Vector3(rend.bounds.min.x/2, rend.bounds.min.y, rend.bounds.min.z/2);
        Vector3 endPointAdj = new Vector3(rend.bounds.max.x / 2, rend.bounds.max.y, rend.bounds.max.z / 2);

        startPoint = startPoint - startPointAdj;
        endPoint = endPoint - endPointAdj;
        */
        // Get the direction of the slide path
        Vector3 slideDirection = (endPoint - startPoint).normalized;

        // Project objectC's position onto the slide path
        Vector3 projectedPosition = startPoint+
            Vector3.Project(objectC.position - startPoint, slideDirection);

        // Set objectA's position to the projected position
        transform.position = projectedPosition;

        transform.rotation = slideObj.rotation;
    }
}
