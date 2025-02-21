using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree_top : MonoBehaviour
{
    public bool grounded = false;
    Rigidbody treeTopRB;
    public Transform COM;
    public Transform chain;
    public Transform cutPoints;
    public Transform cutPointA;
    public Transform cutPointB;
    private Vector3 fallAngle;


    // Start is called before the first frame update
    void Start()
    {
        treeTopRB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cutPointA != null )
        {
           // fallAngle = (cutPointA.position - cutPointB.position).normalized;
            fallAngle = -chain.forward;
        }

        treeTopRB.centerOfMass = COM.position;
        Vector3 currentVelocity = treeTopRB.angularVelocity;

        if (grounded)
        {
            currentVelocity = Vector3.zero;
        }

        int totalPoints = cutPoints.childCount;

        if (totalPoints == 0)
        {
            if (grounded == false)
            {
                fall();
            }
        }
    }

    void fall()
    {
        Vector3 comAdjust = COM.position;
        Vector3 rotatedDirection = Quaternion.Euler(0, -50, 0) * fallAngle;

        treeTopRB.isKinematic = false;
        treeTopRB.angularVelocity = fallAngle/2f;
        //treeTopRB.angularVelocity = rotatedDirection;
        COM.position += transform.up * 4f * Time.deltaTime;
    }

    void OnCollisionEnter(Collision Hit)
    {
        //grounded = true;
        if (Hit.gameObject.tag == "ground")
        {
            grounded = true;
        }
    }
}
