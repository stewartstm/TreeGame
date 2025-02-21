using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class branch : MonoBehaviour
{
    public GameObject root;

    public bool isMainBranch = false;
    public bool cut = false;
    public bool isTreeBase = false;
    public bool isSubBranch = false;
    public bool branchesAssigned = false;
    public bool addJoints = true;

    public Rigidbody thisRB;

    int frameCall = 0;

    public Rigidbody parentRb; // The main object holding the joints
    public List<Rigidbody> childObjects = new List<Rigidbody>();

    public List<FixedJoint> joints = new List<FixedJoint>();

    void Start()
    {
        thisRB = GetComponent<Rigidbody>();

        if(transform.parent!=null)
        {
            root = transform.parent.gameObject;
        }

        transform.SetParent(null);

        if (isSubBranch)
        {
            thisRB.isKinematic = true;
            transform.tag = "subBranch";
            root = null;
        }

        if (isMainBranch)
        {
            foreach(Rigidbody child in transform)
            {
                childObjects.Add(child);
            }

            transform.tag = "mainBranch";
            
            if (root.tag != "tree_base")
            {
                transform.SetParent(null);
                //transform.parent = null;
                Destroy(root);
                root = null;
            }
        }
        
        if(isTreeBase)
        { 
            parentRb = GetComponent<Rigidbody>();
            transform.SetParent(null);

            if (root != null)
            {
                Destroy(root);
            }
        }

        

    }

    // Update is called once per frame
    void Update()
    {

            if(isMainBranch && thisRB.isKinematic)
            {
                MakeNonKinematic();
            }

        RemoveNullJoints();


        if (isTreeBase && addJoints)
        {
            AddJoint();
        }

        frameCall++;

        if (cut)

        {
            foreach (Rigidbody child in childObjects)
            {
                child.isKinematic = true;
            }
            
                foreach (FixedJoint joint in joints)
            {
                if (joint != null)
                {
                    Destroy(joint);
                }
            }
            //Destroy(joint);
            joints.Clear(); // Clear the list after destroying

            if (frameCall > 2)
            {
                cut = false;
                frameCall = 0;
                //childObjects.Clear();
                AddJoint();
                if (!isSubBranch)
                {
                    MakeNonKinematic();
                }
            }
         }
        else
        {
            foreach (Rigidbody child in childObjects)
            {
                //child.isKinematic = false;
            }
        }

        if (isSubBranch && root != null)
        {
            IndipendentSlice sliceScript = root.gameObject.GetComponent<IndipendentSlice>();

            if (sliceScript.cut)
            {
                transform.SetParent(null);
                //childObjects.Clear();
            }
        }
    }

    void MakeNonKinematic()
    {
        if (frameCall > 2)
        {
            thisRB.isKinematic = false;
            if (childObjects != null)
            {
                foreach (Rigidbody child in childObjects)
                {
                    child.isKinematic = false;
                }
            }
        }
    }


    void AddJoint()
    {
        if (cut)
        {
            if (isMainBranch)
            {
                foreach (FixedJoint existingJoint in parentRb.GetComponents<FixedJoint>())
                {
                    Destroy(existingJoint);
                }
            }

            if (isTreeBase)
            {
                FixedJoint[] joints = GetComponents<FixedJoint>();

                for (int i = joints.Length - 1; i >= 0; i--)
                {
                    Destroy(joints[i]);
                }
            }

            return;
        }
        foreach (Rigidbody child in childObjects)
        { 
            bool jointExists = false;

            foreach (FixedJoint existingJoint in parentRb.GetComponents<FixedJoint>())
            {
                if(existingJoint.connectedBody == null)
                {
                    Destroy(existingJoint);
                }
                if (existingJoint.connectedBody == child)
                {
                    jointExists = true;
                    break; // Stop checking if we already found a joint
                }
            }

            if (!jointExists) // Only add a new joint if it doesn't already exist
            {
                FixedJoint joint = parentRb.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = child;
                joint.massScale = 10f;
                joint.connectedMassScale = 100f;
                //joint.breakForce = 20f;
                joints.Add(joint); // Store the joint in a list
            }
        }
    }

    void RemoveNullJoints()
    {
        // Remove null Rigidbody references
        for (int i = childObjects.Count - 1; i >= 0; i--)
        {
            if (childObjects[i] == null)
            {
                childObjects.RemoveAt(i);
            }
        }

        // Remove null FixedJoints
        for (int i = joints.Count - 1; i >= 0; i--)
        {
            if (joints[i] == null)
            {
                joints.RemoveAt(i);
            }
        }
    }

    void RemoveAllJoints()
    {
        if (isTreeBase)
        {
            foreach (FixedJoint existingJoint in GetComponents<FixedJoint>())
            {
                Destroy(existingJoint);
            }
        }
    }

    void OnTriggerStay(Collider hit)
    {
        
        if(hit.tag == "mainBranch" && isTreeBase)
        {
            Rigidbody jointTarget = hit.GetComponent<Rigidbody>();

            if (jointTarget != null && !childObjects.Contains(jointTarget))
            {
                childObjects.Add(jointTarget);
                AddJoint();
            }
        }

        if (hit.tag == "mainBranch" && isSubBranch)
        {
            root = hit.gameObject;
            IndipendentSlice sliceScript = hit.gameObject.GetComponent<IndipendentSlice>();

            if(sliceScript.cut!=true)
            {
                transform.SetParent(hit.gameObject.transform);
            }
        }
    }

}
