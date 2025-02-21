using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class cherry_picker : MonoBehaviour
{
    Vector2 inputVector;
    public float heightOffset;
    public float rotationOffset;
    public Vector3 startPos;

    public Transform player;
    public Transform playerPosition;

    //public Transform pivot;
    public Rigidbody RB;

    public Rigidbody playerRB;

    public BoxCollider box;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        RB = gameObject.GetComponent<Rigidbody>();
        playerRB = player.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (equipHandler.isCherryPicker)
        {
            pickerMovement();
            playerRB.isKinematic = true;
            player.transform.position = Vector3.Lerp(player.transform.position,playerPosition.position,10f * 10f * Time.fixedDeltaTime);
            player.transform.rotation = playerPosition.transform.rotation;
        }
        else
        {
            RB.isKinematic = true;
            player.transform.position = player.transform.position;
            playerRB.isKinematic = false;
        }
    }

    void pickerMovement()
    {
        RB.isKinematic = false;
        //RB.centerOfMass = pivot.position;

        heightOffset += inputVector.y * 3 * Time.fixedDeltaTime;
        heightOffset = Mathf.Clamp(heightOffset, 0, 10f);

        rotationOffset = -inputVector.x;

        Vector3 Position = transform.position;

        Position.y = transform.parent.position.y * heightOffset;

        transform.position = new Vector3(transform.parent.position.x + startPos.x, Position.y, transform.parent.position.z + startPos.z);

        Quaternion rotater = Quaternion.Euler(transform.rotation.x, transform.rotation.eulerAngles.y + rotationOffset, transform.rotation.z);

        RB.MoveRotation(rotater);
    }

    void OnMoveLeftAxis(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
}
