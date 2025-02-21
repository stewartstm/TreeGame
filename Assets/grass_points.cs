using UnityEngine;

public class grass_points : MonoBehaviour
{
    public float colliderRadius = 0.1f; // Radius of each collider
    private Mesh mesh;

    void Start()
    {
        // Get the mesh and all vertices
        mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // Create a parent object to keep the scene organized
        GameObject colliderContainer = new GameObject("VertexColliders");
        colliderContainer.transform.SetParent(transform);
        colliderContainer.transform.localPosition = transform.position;

        // Create a collider for each vertex
        foreach (var vertex in vertices)
        {
            // Instantiate a small sphere collider at each vertex position
            GameObject vertexCollider = new GameObject("VertexCollider");
            vertexCollider.transform.SetParent(colliderContainer.transform);
            vertexCollider.transform.position = transform.TransformPoint(vertex); // Convert to world space

            // Add a SphereCollider component and set its radius
            SphereCollider sphereCollider = vertexCollider.AddComponent<SphereCollider>();
            sphereCollider.radius = colliderRadius;

            // Optionally, make the collider a trigger if you only need to detect overlaps
            sphereCollider.isTrigger = true;

            vertexCollider.tag = "grass";
        }
    }
}
