using UnityEngine;

public class ChangeVertexColorFromBoxCast : MonoBehaviour
{
    private Mesh mesh;
    private Color[] colors;
    private Vector3[] vertices;

    //public float influenceRadius = 0.5f; // Radius to affect vertices
    public LayerMask boxcastLayer; // Layer to define which objects the box can hit
    //public Transform boxOrigin; // Transform to define the origin of the box cast
    public Transform spherePosMower;
    public Transform spherePosTrimmer;
    private Vector3 sphereSize = new Vector3(1, 1, 1);

    private float cutRadiusTrimmer = 0.5f;
    private float cutRadiusMower = 1f;

    private float cutRadius = 0f;

    private bool[] processed;

    private Vector3 spherePosition;

    void Start()
    {
        // Get the mesh from the MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            mesh = meshFilter.mesh;
            vertices = mesh.vertices;
            colors = new Color[mesh.vertexCount];

            // Initialize colors array with white
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
            }

            mesh.colors = colors; // Apply initial colors to the mesh
        }
        else
        {
            Debug.LogError("No MeshFilter found on this GameObject.");
        }
    }

    void Update()
    {
        if (equipHandler.isTrimmer)
        {
            spherePosition = spherePosTrimmer.position;
            cutRadius = cutRadiusTrimmer;
        }

        if (equipHandler.isMower)
        {
            spherePosition = spherePosMower.position;
            cutRadius = cutRadiusMower;
        }

        float sphereRadius = Mathf.Max(sphereSize.x, sphereSize.z); // Use the largest dimension as the radius
        Collider[] hitColliders = Physics.OverlapSphere(spherePosition, sphereRadius, boxcastLayer);


        if (hitColliders.Length > 0)
        {
            // Iterate through all vertices to see if they are in the sphere
            for (int i = 0; i < vertices.Length; i++)
            {
                if (processed == null || processed.Length != vertices.Length)
                {
                    processed = new bool[vertices.Length];
                }

                if (!processed[i])
                {
                    // Convert vertex position to world space
                    Vector3 worldVertex = transform.TransformPoint(vertices[i]);

                    // Check if the vertex is within the sphere
                    if (IsPointInSphere(worldVertex, spherePosition, sphereRadius))
                    {
                        processed[i] = true;
                        // Color based on mower direction
                        colors[i] = new Color(0, 0, 0, 0f);
                    }
                }
            }

        }

        mesh.colors = colors; // Update mesh colors
    }

    private bool IsPointInSphere(Vector3 point, Vector3 sphereCenter, float radius)
    {
        radius = cutRadius;
        
        // Check if the point is within the sphere
        return Vector3.Distance(point, sphereCenter) <= radius;
    }
}
