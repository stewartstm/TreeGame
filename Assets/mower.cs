using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class mower: MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    Vector2 inputVector;
    Vector2 inputVector2;
    float inputRightTrigger = 0f;
    [SerializeField] float moveSpeed = 8;
    Rigidbody myrigidbody;
    public Transform TireBL;
    public Transform TireBR;
    public Transform ArmL;
    public Transform ArmR;
    public Transform TireFL;
    public Transform TireFR;
    public Transform PlayerLookTarget;
    public Transform Deck;
    public Transform heightLever;
    private float deckHeight = 0.2f;
    private bool startCalling = false;

    public bool RotationEnabled = true;
    public bool MovementEnabled = true;


    void Start()
    {
        myrigidbody = GetComponent<Rigidbody>();
        deckHeight = Deck.transform.localPosition.y;
    }

    void OnEnable()
    {
        StartCoroutine(StartCallingAfterDelay());
    }

    void OnDisable()
    {
        startCalling = false;
    }

    IEnumerator StartCallingAfterDelay()
    {
        yield return new WaitForSeconds(1.2f);
        startCalling = true;
    }

    void FixedUpdate()
    {
        if (startCalling)
        {
            move();
        }
        deckHeightAdjust();


        //Arm Movement
        ArmL.transform.localRotation = Quaternion.Slerp(ArmL.localRotation, Quaternion.Euler(inputVector.y*17, 0, 0),9f * Time.deltaTime);
        ArmR.transform.localRotation = Quaternion.Slerp(ArmR.localRotation, Quaternion.Euler(inputVector2.y * 17, 0, 0), 9f * Time.deltaTime);



        //Height Lever Movement
        heightLever.transform.localRotation = Quaternion.Euler(inputRightTrigger * -30, 0, 0);
    }


    void move()
    {
        myrigidbody.centerOfMass = transform.Find("COM").localPosition; // Adjust values as needed
        float rotation1 = inputVector.y * rotationSpeed;
        float rotation2 = inputVector2.y * rotationSpeed;
        Vector3 currentRotation1 = ArmL.localEulerAngles;

        float combinedValue = inputVector.y + inputVector2.y;
        float currentSpeed = myrigidbody.velocity.z * -1;
        float adjustedInputL = Mathf.Abs(inputVector.y);
        float adjustedInputR = Mathf.Abs(inputVector2.y);
        float influence = adjustedInputL + adjustedInputR;

        if (RotationEnabled)
        {
            float yAngularVelocity = (rotation1 - rotation2);

            Vector3 currentAngularVelocity = myrigidbody.angularVelocity;

            myrigidbody.angularVelocity = new Vector3(currentAngularVelocity.x, yAngularVelocity, currentAngularVelocity.z);
        }

        if (MovementEnabled)
        {

            float currentYVelocity = myrigidbody.velocity.y;
            Vector3 forwardVelocity = transform.forward * combinedValue * moveSpeed;
            forwardVelocity.y = currentYVelocity;
            myrigidbody.velocity = forwardVelocity;
        }

        //Rear Tire Rotation 
        TireBL.transform.Rotate(rotation1*2 + combinedValue, 0,0);
        TireBR.transform.Rotate(rotation2*2 + combinedValue, 0, 0);

        float forwardPiv = myrigidbody.angularVelocity.y * 22.5f;
        float ReversePiv = myrigidbody.angularVelocity.y * 22.5f;
        //float lookTargetRot = myrigidbody.angularVelocity.y * 22.5f;
        float lookTargetRot = myrigidbody.angularVelocity.y;
        float LookRotClamped = Mathf.Clamp(lookTargetRot, -95, 95);



        if (influence < .1)
        {
            return;
        }


        if (combinedValue > 0.1)
        {
            Quaternion targetRotationF = Quaternion.Euler(0, forwardPiv, 0);
            Quaternion targetRotationFClamp = Quaternion.Euler(0, Mathf.Clamp(forwardPiv,-95,95), 0);

            TireFL.localRotation = Quaternion.Slerp(TireFL.localRotation,targetRotationF, influence * 8 * Time.fixedDeltaTime);
            TireFR.localRotation = Quaternion.Slerp(TireFR.localRotation,targetRotationF, influence * 8 * Time.fixedDeltaTime);
            PlayerLookTarget.localRotation = Quaternion.Slerp(PlayerLookTarget.localRotation, targetRotationFClamp, influence * 8 * Time.fixedDeltaTime);
        }

        if (combinedValue < 0.1)
        {
            Quaternion targetRotationR = Quaternion.Euler(0, -forwardPiv - 180, 0);

            TireFL.localRotation = Quaternion.Slerp(TireFL.localRotation, targetRotationR, influence * 8 * Time.fixedDeltaTime);
            TireFR.localRotation = Quaternion.Slerp(TireFR.localRotation, targetRotationR, influence * 8 * Time.fixedDeltaTime);
            //PlayerLookTarget.localRotation = Quaternion.Slerp(TireFL.localRotation, targetRotationR, 1 * Time.deltaTime);
        }

        /*
        //Player look control

        if (combinedValue > 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, LookRotClamped, 0);

            PlayerLookTarget.localRotation = Quaternion.Slerp(TireFL.localRotation, targetRotation, influence * 2 * Time.fixedDeltaTime);
        }
        */
    }

    void deckHeightAdjust()
    {
        Deck.localPosition = new Vector3(Deck.transform.localPosition.x, deckHeight + inputRightTrigger*.2f, Deck.transform.localPosition.z);
       // Debug.Log(inputRightTrigger);
    }

    void OnMoveLeftAxis(InputValue value)
    {
        inputVector = value.Get<Vector2> ();
    }
    void OnMoveRightAxis(InputValue value)
    {
        inputVector2 = value.Get<Vector2>();
    }

    void OnTriggerRight(InputValue value)
    {
        inputRightTrigger = value.Get<float>();
    }
}
