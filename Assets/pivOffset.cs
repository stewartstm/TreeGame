using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pivOffset : MonoBehaviour
{
    public Transform rootJoint;
    public Transform lift;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = rootJoint.transform.rotation;

        transform.position = lift.transform.position + rootJoint.transform.forward * 5f;
    }
}
