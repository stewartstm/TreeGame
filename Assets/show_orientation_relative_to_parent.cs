using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class show_orientation_relative_to_parent : MonoBehaviour
{
    public Transform Parent;
    public Vector3 relativePosition;
    public Quaternion relativeRotation;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        relativeRotation = Quaternion.Inverse(Parent.rotation) * transform.rotation;
        relativePosition = transform.InverseTransformPoint(Parent.position);
    }
}
