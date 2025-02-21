using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class branch_backup : MonoBehaviour
{
    public GameObject root;

    public bool isMainBranch = false;
    public bool cut = false;
    public bool isTreeBase = false;
    public bool isSubBranch = false;
    public bool branchesAssigned = false;
    public bool addJoints = true;

    int frameCall = 0;

    //public Transform treeBase;

    public Rigidbody parentRb; // The main object holding the joints
    public List<Rigidbody> childObjects = new List<Rigidbody>();

    public List<FixedJoint> joints = new List<FixedJoint>();


    //public List<GameObject> hitObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

        if(transform.parent!=null)
        {
            root = transform.parent.gameObject;
        }

        transform.SetParent(null);

        if (isSubBranch)
        {
            transform.tag = "subBranch";
            root = null;
        }

        if (isMainBranch)
        {
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
        RemoveNullJoints();

        if(addJoints)
        {
            AddJoint();
            //addJoints = false;
        }

        frameCall++;

        if (cut)

        {

            foreach (FixedJoint joint in joints)
            {
                if (joint != null)
                {
                    Destroy(joint);
                }
            }
            joints.Clear(); // Clear the list after destroying

            if (frameCall > 2)
            {
                //childObjects.Clear();
                cut = false;
                AddJoint();
                frameCall = 0;
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

        // NEW NOT TESTED Supposed to make branch trigger but doesn't 
        /*
        if (isMainBranch && root != null)
        {
            IndipendentSlice sliceScript = root.gameObject.GetComponent<IndipendentSlice>();

            if (sliceScript.cut)
            {
                transform.gameObject.GetComponent<MeshCollider>().isTrigger = true;
                //childObjects.Clear();
            }
        }
        */
    }
 
    void AddJoint()
    {
        foreach (Rigidbody child in childObjects)
        {
            bool jointExists = false;

            foreach (FixedJoint existingJoint in parentRb.GetComponents<FixedJoint>())
            {
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
                joint.breakForce = 20f;
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

    void OnTriggerStay(Collider hit)
    {
        
        /*
        if(cut)
        {       
            return;
        }
        */
        if(hit.tag == "mainBranch" && isTreeBase)
        {
            //if (cut) return;
            //Debug.Log("foundBranch");
            /*
            frameCall++;

            if(cut)
            {

                frameCall = 0;
            }
            

            if (frameCall < 10)
            {
                //Transform branchCollider = hit.transform.Find("collider_bounds");
                Rigidbody jointTarget = hit.GetComponent<Rigidbody>();
                childObjects.Add(jointTarget);
                //AttachObjects();
                AddJoint();
            }
            */

            //Transform branchCollider = hit.transform.Find("collider_bounds");
            Rigidbody jointTarget = hit.GetComponent<Rigidbody>();

            if (jointTarget != null && !childObjects.Contains(jointTarget))
            {
                childObjects.Add(jointTarget);
                AddJoint();
            }
            //AttachObjects();
           
        }


        if(hit.tag == "mainBranch" && isSubBranch)
        {
            root = hit.gameObject;
            IndipendentSlice sliceScript = hit.gameObject.GetComponent<IndipendentSlice>();

            if(sliceScript.cut!=true)
            {
                transform.SetParent(hit.gameObject.transform);
            }
        }

        if(hit.tag == "tree_base" && isMainBranch)
        {

            /*
            root = hit.gameObject;
            IndipendentSlice sliceScript = root.GetComponent<IndipendentSlice>();

            if (sliceScript.cut != true)
            {
                transform.SetParent(root.transform);
            }
            */
        }

        if (hit.tag == "WoodDebris")
        {
            //transform.parent = hit.gameObject.transform;
        }
    }

}
