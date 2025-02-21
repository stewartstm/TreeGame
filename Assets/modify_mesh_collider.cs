using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modify_mesh_collider : MonoBehaviour
{
    public Transform newColliderMesh;
    public MeshCollider currentCollider;
    //public Transform treeBaseCol;
    public bool isTriggerCol = false;
    public bool swapToNonTrigger = false;
    public List<GameObject> objectsToIgnore = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        currentCollider = GetComponent<MeshCollider>();
        //Invoke("AddCollider", 0.01f);
        if (swapToNonTrigger)
        {
            Invoke("SwapCollider", 0.01f);
        }
    }

    // Update is called once per frame
    void Update()
    {
            IgnoreCollision(gameObject, objectsToIgnore);
    }

    void SwapCollider()
    {
        currentCollider.isTrigger = false;
    }

    void AddCollider()
    {

        if (newColliderMesh != null)
        {
            MeshCollider newMeshCollider = gameObject.AddComponent<MeshCollider>();
            MeshFilter meshFilter = newColliderMesh.GetComponent<MeshFilter>();
            newMeshCollider.sharedMesh = meshFilter.sharedMesh;
            newMeshCollider.convex = true;
        }
    }

    void IgnoreCollision(GameObject obj1, List<GameObject> obj2List)
    {
        Collider col1 = obj1.GetComponent<Collider>();

        foreach (GameObject obj2 in obj2List)
        {
            if (obj2 == null) continue; // Skip null objects

            Collider col2 = obj2.GetComponent<Collider>();

            if (col2 != null)
            {
                Physics.IgnoreCollision(col1, col2, true);
            }
        }
    }
}
