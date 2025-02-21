using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree : MonoBehaviour
{
    public Transform cutPoints;
    public Transform chain;
    public Transform treeTop;
    public Transform COM;
    private Rigidbody treeTopRB;
    public bool fallen = false;

    // Start is called before the first frame update
    void Start()
    {
        treeTopRB = treeTop.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        treeTopRB.centerOfMass = COM.position;
        int totalPoints = cutPoints.childCount;
        //Debug.Log(totalPoints);

        if (totalPoints == 0)
        {
                fall();
        }
    }

    void fall()
    {
        treeTopRB.isKinematic = false;
        treeTopRB.angularVelocity = -chain.forward;
    }

}
