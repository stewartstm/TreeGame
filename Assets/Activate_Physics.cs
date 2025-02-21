using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate_Physics : MonoBehaviour
{
    Rigidbody self;
    SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        //self = gameObject.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 objCenter = GetComponent<Renderer>().bounds.center;


        if (Physics.Raycast(objCenter, Vector3.up, out RaycastHit hit, 1f))
        {
            if (hit.collider.tag == "WoodDebris"|| hit.collider.tag == "mainBranch" || hit.collider.tag == "tree_base" || hit.collider.tag == "subBranch")
            {
                if (self == null)
                {
                    self = gameObject.AddComponent<Rigidbody>();
                }
            }
        }
        //Debug.DrawRay(objCenter, Vector3.up * 1f, Color.green);
    }
    
    private void OnCollisionEnter(Collision hit)
    {
        float hitForce = hit.impulse.magnitude;


        if (hitForce > 10f)
        {
            if (self == null)
            {
                self = gameObject.AddComponent<Rigidbody>();
            }
            score.Damage += 5f;
        }

        if (hitForce > 10f && hit.gameObject.tag == "WoodDebris")
        {
            if (self == null)
            {
                self = gameObject.AddComponent<Rigidbody>();
            }
            score.Damage += 5f;
        }

    }
} 
       
    

