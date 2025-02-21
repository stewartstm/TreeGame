using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamic_collider : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    MeshCollider meshCollider;
    MeshCollider meshTriggerCollider;

    public bool addTrigger = false;
    public bool addStandardCollider = true;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        if (addStandardCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
            Mesh bakedMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh);
            meshCollider.sharedMesh = bakedMesh;
            meshCollider.convex = true;
        }

        if (addTrigger)
        {
            meshTriggerCollider = gameObject.AddComponent<MeshCollider>();
            Mesh bakedMesh2 = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh2);
            meshTriggerCollider.sharedMesh = bakedMesh2;
            meshTriggerCollider.convex = true;
            meshTriggerCollider.isTrigger = true;
        }
    }

    void Update()
    {
        if (character_controller.movement && equipHandler.isCherryPicker)
        {
            if (addStandardCollider)
            {
                Mesh bakedMesh = new Mesh();
                skinnedMeshRenderer.BakeMesh(bakedMesh);
                meshCollider.sharedMesh = bakedMesh;
                meshCollider.convex = true;
            }

            if (addTrigger)
            {
                Mesh bakedMesh2 = new Mesh();
                skinnedMeshRenderer.BakeMesh(bakedMesh2);
                meshTriggerCollider.sharedMesh = bakedMesh2;
                meshTriggerCollider.convex = true;
                meshTriggerCollider.isTrigger = true;
            }
        }
    }
}
