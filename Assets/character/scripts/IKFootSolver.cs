using UnityEngine;
using UnityEngine.InputSystem.HID;

public class IKFootSolver : MonoBehaviour
{
    public float ray_distance = 5f;
    public float ray_offset = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 raycast_position = transform.position + Vector3.up * ray_offset;
        RaycastHit hit;
        if (Physics.Raycast(raycast_position, Vector3.down, out hit, ray_distance))

        {
            transform.position = hit.point;
        }
    }
}