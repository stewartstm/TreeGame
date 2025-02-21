using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class character_controller : MonoBehaviour
{
    Rigidbody myrigidbody;
    Rigidbody chainsawRB;

    public static Vector2 inputVector;
    public static Vector2 inputVector2;

    float inputRightTrigger = 0f;
    float inputLeftTrigger = 0f;
    private float moveSpeed = 3f;
    public float rotateSpeed = 5f;
    public float chainsawMaxReach = 5f;

    public Transform chainsaw;
    public Transform toolAction;
    public Transform toolRest;
    public Transform armPivot;

    public Vector3 armRestPos;

    public static bool trimmerEq = false;
    public static bool chainsawEq = false;
    public static bool mowerEq = false;
    public static bool movement = false;

    public Transform trimmerTool;
    public Transform chainsawTool;

    

    // Start is called before the first frame update
    void Start()
    {
        myrigidbody = GetComponent<Rigidbody>();
        chainsawRB = chainsaw.GetComponent<Rigidbody>();
        armRestPos = armPivot.transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveLeftInput = Mathf.Abs(inputVector.magnitude);
        float moveRightInput = Mathf.Abs(inputVector2.magnitude);

        float combinedInput = moveLeftInput + moveRightInput;

        //Debug.Log(combinedInput);

        if (combinedInput > 0f)
        {
            movement = true;
        }
        else
        {
            movement = false;
        }
        if (equipHandler.isChainsaw)
        {
            //Quaternion armPosition = Quaternion.Euler(inputVector2.y * 45f, inputVector2.x * 45f, 0);
            //armPivot.localRotation = Quaternion.Slerp(armPivot.localRotation, armPosition, 3f * Time.deltaTime);
            ChainsawAction();
        }

        if (equipHandler.isTrimmer)
        {
            Quaternion armPosition = Quaternion.Euler(0, inputVector2.x * 45f, 0);
            armPivot.localRotation = Quaternion.Slerp(armPivot.localRotation, armPosition, 3f * Time.deltaTime);
        }
        //Debug.Log(inputVector2.x);

        if (inputLeftTrigger < 0.1f)
        {
            BasicMove();
        }

        if (inputLeftTrigger > 0.1f)
        {
            ActionMove();
        }

        if (inputRightTrigger < 0.1f)
        {
            moveSpeed = 4f;
            rotateSpeed = 4f;
            Physics.IgnoreLayerCollision(11, 12,false);
            chainsawRB.useGravity = true;
        }

        if (inputRightTrigger > 0.1f)
        {
            moveSpeed = 2f;
            rotateSpeed = 2f;
            Physics.IgnoreLayerCollision(11, 12);
            chainsawRB.useGravity = false;
        }
        

        Physics.IgnoreLayerCollision(8, 10);
        Physics.IgnoreLayerCollision(10, 12);


    }

    void ChainsawAction()
    {
        if (inputRightTrigger > 0.1f)
        {
            Quaternion chainsawRotateAction = Quaternion.Euler(inputVector2.y * 60f, inputVector2.x * 90f, inputVector2.x * -90f);
            Vector3 reachOffset = new Vector3(armRestPos.x + Mathf.Clamp(inputVector2.x, -1, 0) * 0.7f, armPivot.localPosition.y, armRestPos.z + Mathf.Clamp(inputVector.y, 0, 1 * chainsawMaxReach) + Mathf.Abs(Mathf.Clamp(inputVector2.x, -1, 0.5f)));
            chainsaw.localRotation = Quaternion.Lerp(chainsaw.localRotation, chainsawRotateAction, 7f * Time.deltaTime);
            armPivot.localPosition = Vector3.Lerp(armPivot.localPosition, reachOffset, 10f * Time.fixedDeltaTime);
            return;
        }
        else
        {
            //chainsaw.position = Vector3.Lerp(chainsaw.position, toolAction.position, inputVector2.x * 7f * Time.deltaTime);
            Quaternion chainsawRotateAction = Quaternion.Euler(inputVector2.y * 60f, inputVector2.x * 90f, inputVector2.x * -90f);
            chainsaw.localRotation = Quaternion.Lerp(chainsaw.localRotation, chainsawRotateAction, 7f * Time.deltaTime);
            Vector3 reachOffset = new Vector3(armRestPos.x + Mathf.Clamp(inputVector2.x, -1, 0) * 0.7f, armPivot.localPosition.y, armRestPos.z + Mathf.Abs(Mathf.Clamp(inputVector2.x, -1,0.5f)) * 0.9f);
            armPivot.localPosition = Vector3.Lerp(armPivot.localPosition, reachOffset, 10f * Time.fixedDeltaTime);
        }
    }

    void BasicMove()
    {
        if (equipHandler.isChainsaw && inputRightTrigger > 0.1f)
        {
            moveSpeed = moveSpeed / 2;
        }
            if (equipHandler.isCherryPicker == false)
        {
            //myrigidbody.freezeRotation.y = false;
            Vector3 currentVelocity = myrigidbody.velocity;
            Quaternion currentRotation = Quaternion.identity;

            Vector3 forwardVelocity = transform.forward * inputVector.y * moveSpeed;
            forwardVelocity.y = currentVelocity.y;
            myrigidbody.velocity = forwardVelocity;

            myrigidbody.angularVelocity = new Vector3(currentRotation.x, inputVector.x * rotateSpeed, currentRotation.z);
            myrigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //MAKE LOOK AT TARGET SO WALKING IN REVERSE LOOKS MORE NATURAL
        }
    }

    void ActionMove()
    {
        if (equipHandler.isChainsaw && inputRightTrigger > 0.1f)
        {
            moveSpeed = moveSpeed / 2;
        }

        if (equipHandler.isCherryPicker == false)
        {
            Vector3 currentVelocity = myrigidbody.velocity;
            Quaternion currentRotation = Quaternion.identity;

            Vector3 forwardVelocity = transform.forward * inputVector.y * moveSpeed;
            Vector3 horizontalVelocity = transform.right * inputVector.x * moveSpeed / 2;
            forwardVelocity.y = currentVelocity.y;
            myrigidbody.velocity = forwardVelocity + horizontalVelocity;

            myrigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            //Debug.Log("ActionMove");
        }
    }

    void OnMoveLeftAxis(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
    void OnMoveRightAxis(InputValue value)
    {
        inputVector2 = value.Get<Vector2>();
    }

    void OnTriggerRight(InputValue value)
    {
        inputRightTrigger = value.Get<float>();
    }

    void OnTriggerLeft(InputValue value)
    {
        inputLeftTrigger = value.Get<float>();
    }
}
