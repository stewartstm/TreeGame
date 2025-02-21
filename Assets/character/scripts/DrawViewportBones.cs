using UnityEngine;

[ExecuteInEditMode]
public class DrawViewportBones : MonoBehaviour
{
    public bool spheres = true;
    public bool lines = true;
    public float gizmo_size = 0.05f;

    private void OnDrawGizmos()
    {
        DrawBones(transform);
    }

    private void DrawBones(Transform parent)
    {
        if (!spheres && !lines)
        {
            return;
        }

        foreach (Transform child in parent)
        {
            if (spheres)
            {
                Gizmos.color = Color.green; // You can change the color as you like
                Gizmos.DrawSphere(child.position, gizmo_size); // Sphere at bone position
            }

            if (lines == true)
            {

                if (parent != transform)
                {
                    Gizmos.color = Color.red; // You can change the color as you like
                    Gizmos.DrawLine(parent.position, child.position); // Line connecting parent to child
                }
            }
            DrawBones(child); // Recursively draw bones for child transforms
        }
    }
}