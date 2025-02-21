using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamic_child_collision : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 startRot;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = startPos + transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }
}
