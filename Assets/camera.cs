using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform mainCamera;
    public Transform cameraFocus;
    public float cameraHeight = 10f;
    public float smoothFollowSpeed = 4f*0.1f;
    public float smoothRotateSpeed = 4f;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //var cameraRot = mainCamera.rotation;
        float dist = Vector3.Distance(mainCamera.position, cameraFocus.position);
        Vector3 focusPosition = new Vector3(cameraFocus.position.x, transform.position.y + cameraHeight, cameraFocus.position.z);
        if (equipHandler.isCherryPicker)
        {
            mainCamera.parent = transform;
        }
        else
        {
            mainCamera.position = Vector3.Lerp(mainCamera.position, focusPosition, smoothFollowSpeed * dist * Time.fixedDeltaTime);
        }
        //mainCamera.rotation = Quaternion.Slerp(mainCamera.localRotation, cameraFocus.rotation, smoothRotateSpeed * Time.fixedDeltaTime);
        mainCamera.LookAt(transform.position);

        //cameraRot.y = 45f;
    }
}
