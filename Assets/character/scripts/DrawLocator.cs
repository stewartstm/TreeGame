using UnityEngine;


public enum ShapeTypeEnum
{
    Cube,
    Circle,
    Sphere,
    Cross
}
public class DrawLocator : MonoBehaviour
{
    public float gizmoSize = 1.0f;
    public Vector3 translationOffset = Vector3.zero;
    public ShapeTypeEnum Shape;
    public Color gizmoColor = Color.green;


    private void OnDrawGizmos()
    {
        Matrix4x4 originalMatrix = SetShapePosition();
        DrawShape(gizmoColor, gizmoSize);
        // Restore the original Gizmos matrix
        Gizmos.matrix = originalMatrix;
    }

    private Matrix4x4 SetShapePosition()
    {
        // Save the current Gizmos matrix
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Matrix4x4 translationMatrix = Matrix4x4.TRS(translationOffset, Quaternion.identity, Vector3.one);

        // Set the Gizmos matrix to the GameObject's transformation (position, rotation, scale)
        Gizmos.matrix = transform.localToWorldMatrix * translationMatrix;

        return originalMatrix;
    }

    private void DrawShape(Color shapeColor, float size)
    {
        // Draw the specified shape
        if (Shape == ShapeTypeEnum.Sphere)
        {
            Gizmos.DrawWireSphere(Vector3.zero, size);
        }
        if (Shape == ShapeTypeEnum.Cube)
        {
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size, size, size));
        }
        if (Shape == ShapeTypeEnum.Circle)
        {
            DrawCircle(30, size);
        }
        if (Shape == ShapeTypeEnum.Cross)
        {
            DrawCross(size);
        }
        Gizmos.color = shapeColor;
    }

    // Draw a circle using line segments
    private void DrawCircle(float segments, float radius)
    {
        // Loop through the number of segments and draw lines between consecutive points
        Vector3 previousPoint = Vector3.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            // Calculate the next point on the circle's circumference
            float angle = i * 2 * Mathf.PI / segments;
            Vector3 currentPoint = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            // Draw a line from the previous point to the current point
            Gizmos.DrawLine(previousPoint, currentPoint);

            // Set the previous point for the next segment
            previousPoint = currentPoint;
        }
    }

    // Draw a locator cross
    private void DrawCross(float lineLength)
    {
        // Define the positions of the four points of the cross
        Vector3 top = new Vector3(0, 0, lineLength);
        Vector3 bottom = new Vector3(0, 0, -lineLength);
        Vector3 left = new Vector3(-lineLength, 0, 0);
        Vector3 right = new Vector3(lineLength, 0, 0);
        Vector3 front = new Vector3(0, -lineLength, 0);
        Vector3 back = new Vector3(0, lineLength, 0);

        Gizmos.DrawLine(top, bottom);
        Gizmos.DrawLine(left, right);
        Gizmos.DrawLine(front, back);
    }

    // This is called when the gizmo is drawn only when the object is selected
    private void OnDrawGizmosSelected()
    {
        Matrix4x4 originalMatrix = SetShapePosition();
        DrawShape(Color.blue, gizmoSize);
        Gizmos.matrix = originalMatrix;
    }
}