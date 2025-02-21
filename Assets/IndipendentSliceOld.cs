using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class IndipendentSliceOld : MonoBehaviour
{
    public GameObject planeDebug;
    public GameObject root;
    public List<GameObject> hitObjects = new List<GameObject>();

    public Transform player;
    public Transform chainsawBase;

    public GameObject sliceDebug;

    public bool upright = false;
    public bool branchUpright = false;
    public bool isBranch = false;
    public bool branchForward = false;
    public bool cut = false;
    public bool makeKinematic = true;
    private int frameCall;

    public bool showBranchRelPos = false;

    public List<Transform> children = new List<Transform>();
    public List<Transform> subChildren = new List<Transform>();

    public List<Transform> childrenAbove = new List<Transform>();
    public List<Transform> childrenBelow = new List<Transform>();

    private Vector3 fallAngle = Vector3.zero;
    private Vector3 sawPointA;
    private Vector3 sawPointB;

    public Quaternion branchUp;

    private float sawRotateA;
    private float sawRotateB;

    private Vector3 planeOrient;
    private Vector3 preservedScale;

    private Material crossSectionMaterial;

    private MeshFilter meshFilter;
    private Mesh originalMesh;
    private Mesh modifiedMesh;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        planeDebug = GameObject.Find("SlicePlane");

        /*
        if (isBranch)
        {
            if (root == null)
            {
                root = transform.parent.parent.gameObject;
            }
        }
        */

        sliceDebug = Resources.Load<GameObject>("SliceIndicatorDebug");
        if (!isBranch)
        {
            sliceDebug = Instantiate(sliceDebug, transform.position, transform.rotation);
            sliceDebug.GetComponent<project_along_object>().slideObj = transform;
        }

        if (chainsawBase == null)
        {
            chainsawBase = GameObject.Find("chainsaw_final_v1").transform;
        }
        crossSectionMaterial = Resources.Load<Material>($"mat_wood_grain");

        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            originalMesh = Instantiate(meshFilter.sharedMesh); // Save a copy
            modifiedMesh = meshFilter.mesh;
        }

        if (isBranch && makeKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        //transform.localScale = preservedScale;

        // Get child transforms
        GetChildren(transform, children);


        //Remove Null Objs 
        children.RemoveAll(obj => obj == null);
        childrenAbove.RemoveAll(obj => obj == null);
        childrenBelow.RemoveAll(obj => obj == null);

        // If tree chunk determine which branches are above and below player
        if (!isBranch && sliceDebug != null)
        {
            if (children != null)
            {
                Vector3 relativePosToPlayer = transform.InverseTransformPoint(player.position);

                if (upright)
                {
                    foreach (Transform child in children)
                    {
                        if (child.transform.InverseTransformPoint(transform.position).y * -1f * child.localScale.y > sliceDebug.transform.InverseTransformPoint(transform.position).y * -1f)
                        {
                            if (!childrenAbove.Contains(child))
                            {
                                childrenAbove.Add(child);
                            }
                            childrenBelow.Remove(child);
                        }
                        else
                        {
                            if (!childrenBelow.Contains(child))
                            {
                                childrenBelow.Add(child);
                            }
                            childrenAbove.Remove(child);
                        }
                    }
                }

                else
                {
                    if (relativePosToPlayer.z < 0)
                    {
                        foreach (Transform child in children)
                        {
                            if (child.transform.InverseTransformPoint(transform.position).y * -1f * child.localScale.y > sliceDebug.transform.InverseTransformPoint(transform.position).y * -1f)
                            {
                                if (!childrenAbove.Contains(child))
                                {
                                    childrenAbove.Add(child);
                                }
                                childrenBelow.Remove(child);
                            }
                            else
                            {
                                if (!childrenBelow.Contains(child))
                                {
                                    childrenBelow.Add(child);
                                }
                                childrenAbove.Remove(child);
                            }
                        }
                    }
                    else
                    {
                        foreach (Transform child in children)
                        {
                            if (child.transform.InverseTransformPoint(transform.position).y * -1f * child.localScale.y < sliceDebug.transform.InverseTransformPoint(transform.position).y * -1f)
                            {
                                if (!childrenAbove.Contains(child))
                                {
                                    childrenAbove.Add(child);
                                }
                                childrenBelow.Remove(child);
                            }
                            else
                            {
                                if (!childrenBelow.Contains(child))
                                {
                                    childrenBelow.Add(child);
                                }
                                childrenAbove.Remove(child);
                            }
                        }
                    }
                }
            }
        }

        // cooldown a few frames after cutting
        if (cut)
        {
            frameCall++;
            if (frameCall > 3)
            {
                cut = false;
            }
        }

        // REWORK VV Better system for calculating upper and lower branch!!

        // Component in later determining which end to make upper branch and lower branch (uses saw orientation) 
        if (isBranch)
        {
            Vector3 localBoundsMax = meshFilter.sharedMesh.bounds.max;
            Vector3 worldBoundsMax = transform.TransformPoint(localBoundsMax); // Convert local bounds to world space
            Vector3 playerRelativePosToBranchTip = player.InverseTransformPoint(worldBoundsMax); // Convert to root's local space



            //if(transform.rotation.eulerAngles.x )
            Quaternion relativeRotation = Quaternion.Inverse(player.rotation) * transform.rotation;
            //branchUp = transform.rotation;

            if (showBranchRelPos) Debug.Log(relativeRotation.eulerAngles);

            if (playerRelativePosToBranchTip.y < 3)
            {
                if (relativeRotation.eulerAngles.y > 0 && relativeRotation.eulerAngles.y < 180)
                {
                    branchForward = false;
                }
                else
                {
                    branchForward = true;
                }

                branchUpright = false;
            }
            else
            {
                branchUpright = true;
            }

        }

        // Determine if the object you are cutting is upright
        if (transform.rotation.x == 0 && transform.rotation.z == 0)
        {
            upright = true;
        }
        else
        {
            upright = false;
        }
    }

    // Properties used to ensure you rotate or move the saw before executing cut (not currently used)
    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "chain")
        {
            sawPointA = chainsawBase.transform.position;
            sawRotateA = chainsawBase.transform.localRotation.y * -90f;
        }

    }

    // Used to assign (root if branch) (child rigidbodies if tree chunk)
    void OnTriggerStay(Collider hit)
    {

        if (isBranch)
        {
            if (root == null)
            {
                if (hit.tag == "tree_base")
                {
                    root = hit.gameObject;
                }
            }
        }

        if (!isBranch && cut)
        {
            if (hit.tag == "mainBranch")
            {
                if (!children.Contains(hit.gameObject.transform))
                {
                    //children.Add(hit.gameObject.transform);
                }
            }
        }

    }
    void OnCollisionStay(Collision hit)
    {
        if (isBranch)
        {
            if (root == null)
            {
                if (hit.gameObject.CompareTag("tree_base"))
                {
                    root = hit.gameObject;
                }
            }
        }
    }


    void OnTriggerExit(Collider hit)
    {
        // do not execute if cut cooldown not exceeded
        if (cut) return;

        if (equipHandler.isChainsaw && equipHandler.inputRightTrigger > 0.2f)
        {
            // All properties used as a movement and rotation threshold to determine if cut should be made (not surrently used)
            Vector3 chainsawAngle = chainsawBase.transform.rotation.eulerAngles;
            sawPointB = chainsawBase.transform.position;
            sawRotateB = chainsawBase.transform.localRotation.y * -90f;

            Vector3 playerPosDiff = sawPointA - sawPointB;
            float RotDiff = Mathf.Abs(sawRotateA - sawRotateB);

            float PosDiff = playerPosDiff.magnitude;
            // end

            if (PosDiff > 0f || RotDiff > 0f)
            {
                if (hit.gameObject.tag == "chain")
                {

                    // Determines which end of tree chunk to make upper and lower (uses saw orientation)
                    Quaternion planeRotAdj = planeDebug.transform.rotation;

                    // Determines which end of tree chunk to make upper and lower hull based on saw rotation
                    if (upright || branchUpright)
                    {
                        if (!isBranch)
                        {
                            if (character_controller.inputVector2.x < 0 || character_controller.inputVector2.x == 0)
                            {
                                planeOrient = new Vector3(chainsawBase.rotation.eulerAngles.x, chainsawBase.rotation.eulerAngles.y, chainsawBase.rotation.eulerAngles.z + 90f);
                            }
                            if (character_controller.inputVector2.x > 0)
                            {
                                planeOrient = new Vector3(chainsawBase.rotation.eulerAngles.x, chainsawBase.rotation.eulerAngles.y, chainsawBase.rotation.eulerAngles.z - 90f);
                            }
                        }
                        else
                        {
                            if (character_controller.inputVector2.x < 0 || character_controller.inputVector2.x == 0)
                            {
                                planeOrient = new Vector3(chainsawBase.rotation.eulerAngles.x, chainsawBase.rotation.eulerAngles.y, chainsawBase.rotation.eulerAngles.z - 90f);
                            }
                            if (character_controller.inputVector2.x > 0)
                            {
                                planeOrient = new Vector3(chainsawBase.rotation.eulerAngles.x, chainsawBase.rotation.eulerAngles.y, chainsawBase.rotation.eulerAngles.z + 90f);
                            }
                        }
                    }
                    else
                    {
                        planeOrient = new Vector3(chainsawBase.rotation.eulerAngles.x, chainsawBase.rotation.eulerAngles.y, chainsawBase.rotation.eulerAngles.z + 90f);
                    }


                    // Determines which end to make upper branch and lower branch (uses saw orientation)
                    if (isBranch && !branchForward)
                    {
                        planeOrient = new Vector3(chainsawBase.rotation.eulerAngles.x, chainsawBase.rotation.eulerAngles.y, chainsawBase.rotation.eulerAngles.z + 90f);
                    }

                    if (isBranch && branchForward)
                    {
                        planeOrient = new Vector3(chainsawBase.rotation.eulerAngles.x, chainsawBase.rotation.eulerAngles.y, chainsawBase.rotation.eulerAngles.z - 90f);
                    }

                    // Moves slice plane to cut point
                    planeDebug.transform.position = hit.transform.position;
                    planeRotAdj = Quaternion.Euler(planeOrient);


                    planeDebug.transform.rotation = planeRotAdj;

                    Slice(gameObject);
                }
            }
        }
    }

    // Uses SlicedHull script to split mesh
    public void Slice(GameObject target)
    {
        //Start cut cooldown timer
        cut = true;
        if (root != null) root.GetComponent<IndipendentSlice>().cut = true;
        frameCall = 0;
        //

        if (!isBranch)
        {
            foreach (Transform child in children)
            {
                child.GetComponent<IndipendentSlice>().root = null;
            }
        }

        // Send slice plane position and orientation to SlicedHull script
        SlicedHull hull = target.Slice(planeDebug.transform.position, planeDebug.transform.up);


        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);

            if (upright && !isBranch)
            {
                SetupSlicedComponentLowerInitial(lowerHull);
                SetupSlicedComponentUpperInitial(upperHull);
            }
            //score.DebrisRemaining += 1f;
            if (!upright && !isBranch)
            {
                SetupSlicedComponent(lowerHull);
                IndipendentSlice SliceCtrl = lowerHull.GetComponent<IndipendentSlice>();
                branch branchCtrl = lowerHull.GetComponent<branch>();

                branchCtrl.childObjects.Clear();
                SliceCtrl.children.Clear();

                foreach (Transform child in childrenBelow)
                {
                    if (child != null)
                    {
                        Rigidbody crb = child.GetComponent<Rigidbody>(); // Get Rigidbody
                        if (crb != null) // Only add if Rigidbody exists
                        {
                            branchCtrl.childObjects.Add(crb);
                            SliceCtrl.children.Add(child);
                        }
                    }
                }
                branchCtrl.addJoints = true;

                SetupSlicedComponent(upperHull);
                IndipendentSlice SliceCtrl2 = upperHull.GetComponent<IndipendentSlice>();
                branch branchCtrl2 = upperHull.GetComponent<branch>();

                branchCtrl2.childObjects.Clear();
                SliceCtrl2.children.Clear();

                foreach (Transform child in childrenAbove)
                {
                    if (child != null)
                    {
                        Rigidbody crb = child.GetComponent<Rigidbody>(); // Get Rigidbody
                        if (crb != null) // Only add if Rigidbody exists
                        {
                            branchCtrl2.childObjects.Add(crb);
                            SliceCtrl2.children.Add(child);
                        }
                    }
                }

                branchCtrl2.addJoints = true;
            }

            if (isBranch)

            {
                SetupSlicedComponentLowerBranch(lowerHull);
                SetupSlicedComponentUpperBranch(upperHull);

                // Used to reassign Sub Branches
                if (children != null)
                {
                    foreach (Transform child in children)
                    {
                        child.SetParent(null, true); // Keep world position
                    }
                }
            }

        }
        // Destroy Sliced object after creating new split meshes

        if (!isBranch) sliceDebug.GetComponent<project_along_object>().slideObj = null; Destroy(gameObject);

        if (isBranch) Destroy(gameObject);


    }

    public void SetupSlicedComponent(GameObject slicedObject)
    {
        float debrisSize = slicedObject.GetComponent<MeshFilter>().mesh.bounds.size.magnitude;

        // Destroy if too small
        if (debrisSize < 0.5f)
        {
            Destroy(slicedObject);
            return;
        }

        // Changes all vertex posiitons to offset pivot
        Vector3 slicedCenter = slicedObject.GetComponent<MeshFilter>().mesh.bounds.center;
        ChangePivot(slicedObject, slicedCenter);

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = debrisSize * 3f;
        //rb.centerOfMass = slicedCenter;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.isTrigger = true;

        slicedObject.tag = "tree_base";
        slicedObject.layer = 9;

        IndipendentSlice SliceCtrl = slicedObject.AddComponent<IndipendentSlice>();
        SliceCtrl.cut = true;

        branch branchCtrl = slicedObject.AddComponent<branch>();
        branchCtrl.cut = true;
        branchCtrl.isTreeBase = true;

        modify_mesh_collider meshColScript = slicedObject.AddComponent<modify_mesh_collider>();
        foreach (Transform child in children)
        {
            if (child != null)
            {
                meshColScript.objectsToIgnore.Add(child.gameObject);
            }
        }
        meshColScript.swapToNonTrigger = true;
    }

    public void SetupSlicedComponentUpperBranch(GameObject slicedObject)
    {
        float debrisSize = slicedObject.GetComponent<MeshFilter>().mesh.bounds.size.magnitude;
        if (debrisSize < 0.5f)
        {
            Destroy(slicedObject);
            return;
        }
        Vector3 slicedCenter = slicedObject.GetComponent<MeshFilter>().mesh.bounds.center;
        ChangePivot(slicedObject, slicedCenter);

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = debrisSize * 3f;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        slicedObject.tag = "mainBranch";
        slicedObject.layer = 9;
        collider.convex = true;
        IndipendentSlice sliceScript = slicedObject.AddComponent<IndipendentSlice>();
        sliceScript.makeKinematic = false;
        sliceScript.isBranch = true;
        sliceScript.cut = true;
    }


    public void SetupSlicedComponentLowerBranch(GameObject slicedObject)
    {
        slicedObject.transform.localScale = transform.localScale;

        float chunkSize = slicedObject.GetComponent<MeshFilter>().mesh.bounds.size.magnitude;
        //Debug.Log(chunkSize);
        slicedObject.transform.position = transform.position;
        slicedObject.transform.rotation = transform.rotation;

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();

        //rb.isKinematic = true;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.isTrigger = true;
        slicedObject.layer = 6;

        if (chunkSize < 1.15f && upright)
        {
            score.TreesRemaining -= 1f;
            return;
        }
        slicedObject.tag = "mainBranch";

        IndipendentSlice sliceScript = slicedObject.AddComponent<IndipendentSlice>();
        sliceScript.root = root;
        sliceScript.isBranch = true;
        sliceScript.cut = true;

        branch branchCtrlRoot = root.GetComponent<branch>();
        branchCtrlRoot.childObjects.Add(rb);

        branch branchCtrl = slicedObject.AddComponent<branch>();
        branchCtrl.isMainBranch = true;
        branchCtrl.root = root;
        root.GetComponent<IndipendentSlice>().children.Add(slicedObject.transform);

        modify_mesh_collider meshColScript = slicedObject.AddComponent<modify_mesh_collider>();
        meshColScript.swapToNonTrigger = true;
        if (root != null)
        {
            meshColScript.objectsToIgnore.Add(root);
        }

    }

    public void SetupSlicedComponentUpperInitial(GameObject slicedObject)
    {

        slicedObject.transform.position = transform.position;
        slicedObject.transform.rotation = transform.rotation;

        fallAngle = player.transform.forward;
        float debrisSize = slicedObject.GetComponent<MeshFilter>().mesh.bounds.size.magnitude;
        Vector3 slicedCenter = slicedObject.GetComponent<MeshFilter>().mesh.bounds.center;
        ChangePivot(slicedObject, slicedCenter);

        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.isTrigger = true;


        slicedObject.tag = "tree_base";
        slicedObject.layer = 9;

        modify_mesh_collider meshColScript = slicedObject.AddComponent<modify_mesh_collider>();

        foreach (Transform child in childrenAbove)
        {
            if (child != null)
            {
                meshColScript.objectsToIgnore.Add(child.gameObject);
            }
        }

        meshColScript.swapToNonTrigger = true;

        IndipendentSlice sliceScript = slicedObject.AddComponent<IndipendentSlice>();
        sliceScript.cut = true;
        sliceScript.root = root;

        //score.DebrisRemaining += 1f;

        if (childrenAbove != null)
        {
            branch branchCtrl = slicedObject.AddComponent<branch>();
            branchCtrl.cut = true;
            branchCtrl.isTreeBase = true;

            foreach (Transform child in childrenAbove)
            {
                if (child != null)
                {
                    child.GetComponent<IndipendentSlice>().root = null;
                    child.GetComponent<IndipendentSlice>().root = slicedObject;
                    Rigidbody crb = child.GetComponent<Rigidbody>(); // Get Rigidbody
                    if (crb != null) // Only add if Rigidbody exists
                    {
                        branchCtrl.childObjects.Add(crb);
                        sliceScript.children.Add(child);

                    }
                }
            }

            Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
            rb.mass = debrisSize * 3f;
            rb.centerOfMass = rb.centerOfMass + fallAngle * 2f;

            branchCtrl.addJoints = true;
        }
    }

    public void SetupSlicedComponentLowerInitial(GameObject slicedObject)
    {

        float chunkSize = slicedObject.GetComponent<MeshFilter>().mesh.bounds.size.magnitude;
        Debug.Log(chunkSize);
        slicedObject.transform.position = transform.position;
        slicedObject.transform.rotation = transform.rotation;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        slicedObject.layer = 6;

        if (chunkSize < 1.15f && upright)
        {
            score.TreesRemaining -= 1f;
            return;
        }
        slicedObject.tag = "tree_base";

        modify_mesh_collider meshColScript = slicedObject.AddComponent<modify_mesh_collider>();
        meshColScript.swapToNonTrigger = true;

        foreach (Transform child in childrenBelow)
        {
            if (child != null)
            {
                meshColScript.objectsToIgnore.Add(child.gameObject);
            }
        }

        IndipendentSlice sliceCtrl = slicedObject.AddComponent<IndipendentSlice>();
        sliceCtrl.cut = true;
        sliceCtrl.root = root;

        if (childrenBelow != null)
        {

            Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            branch branchCtrl = slicedObject.AddComponent<branch>();
            branchCtrl.cut = true;
            branchCtrl.isTreeBase = true;

            foreach (Transform child in childrenBelow)
            {
                if (child != null)
                {
                    Rigidbody crb = child.GetComponent<Rigidbody>(); // Get Rigidbody
                    if (crb != null) // Only add if Rigidbody exists
                    {
                        branchCtrl.childObjects.Add(crb);
                        sliceCtrl.children.Add(child);
                        child.GetComponent<IndipendentSlice>().root = slicedObject;
                    }
                }
            }
            branchCtrl.addJoints = true;
        }
    }



    void ChangePivot(GameObject obj, Vector3 newPivot)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("No MeshFilter or Mesh found on the GameObject!");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        // Offset all vertices by the new pivot
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= newPivot;
        }

        mesh.vertices = vertices;

        // Recalculate bounds to ensure the mesh's bounding box is updated
        mesh.RecalculateBounds();

        // Move the object to align the new pivot position with the world position
        obj.transform.position += obj.transform.TransformVector(newPivot);
    }

    void GetChildren(Transform parent, List<Transform> children)
    {
        // Get direct children
        foreach (Transform child in parent)
        {
            if (!children.Contains(child) && child.tag != "tree_base")
            {
                children.Add(child);
            }
        }
    }


    void OnDisable() // Runs when exiting Play Mode
    {
        if (meshFilter != null && originalMesh != null)
        {
            meshFilter.mesh = originalMesh; // Restore original
        }
    }

}
