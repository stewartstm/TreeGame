using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class lift_controls : MonoBehaviour
{
    Vector2 inputVector;
    Vector2 inputVector2;

    private float heightOffset;
    public float maxHeight = 15f;
    public float maxReach = 20f;

    public float rotationOffset;
    public float reachOffset;
    public float startOffset = 5f;
    private float rotatorInfluenceAdj;
    public float currentReachOffset;

    public Vector3 startPos;

    public Transform armBone;

    public Transform rootJoint;
    public Transform lift;

    public Transform pivot;
    public Transform ikCtrl;

    public Rigidbody RB;

    public bool hittingObj = false;
    public static bool collisionCall = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = ikCtrl.transform.position;
        heightOffset = startPos.y;
        reachOffset = startPos.z + startOffset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        //pickerMovement();
        //return;
            
        if (equipHandler.isCherryPicker)
        {
            pickerMovement();
        }
    }

    void pickerMovement()
    {
        if (collision_detect_lift.hitObject == true)
        {
            hittingObj = true;
        }

        if (collision_detect_lift.hitObject == false)
        {
            hittingObj = false;
        }
   
        heightOffset += inputVector.y * 3f * Time.fixedDeltaTime;

        //Height Limiter
        if (!hittingObj)
        {
            heightOffset = Mathf.Clamp(heightOffset, startPos.y, maxHeight);
        }

        if (hittingObj)
        {
            if (collision_detect_lift.influenceOnCollision.y > 0.1)
            {
                heightOffset = Mathf.Clamp(heightOffset, startPos.y, ikCtrl.position.y);
            }

            if (collision_detect_lift.influenceOnCollision.y < 0.1)
            {
                heightOffset = Mathf.Clamp(heightOffset, ikCtrl.position.y, maxHeight);
            }
        }
        //End

        
        reachOffset += inputVector2.y * 3f * Time.fixedDeltaTime;

        //Reach Limiter
        if (!hittingObj)
        {
            reachOffset = Mathf.Clamp(reachOffset, -maxReach, startPos.z + 5f);            
        }

        if (hittingObj)
        {
            if (collisionCall)
            {
                currentReachOffset = reachOffset;
                collisionCall = false;
            }

            if (collision_detect_lift.influenceOnCollision.z > 0.1)
            {
                reachOffset = Mathf.Clamp(reachOffset, -maxReach, currentReachOffset);
            }

            if (collision_detect_lift.influenceOnCollision.z < 0.1)
            {
                reachOffset = Mathf.Clamp(reachOffset, currentReachOffset, startPos.z + 5f);
            }
        }
        //End

        ikCtrl.position = rootJoint.transform.position + rootJoint.transform.forward * reachOffset;

        rotationOffset = -inputVector.x;

        Vector3 Position = ikCtrl.position;

        Position.y = heightOffset;

        ikCtrl.transform.position = new Vector3(Position.x, Position.y, Position.z);

        Quaternion IKRotate = Quaternion.Euler(ikCtrl.transform.rotation.eulerAngles.x, rootJoint.transform.rotation.eulerAngles.y, ikCtrl.transform.rotation.eulerAngles.z);

        ikCtrl.transform.rotation = IKRotate;

        rotatorInfluenceAdj = rootJoint.rotation.eulerAngles.y + rotationOffset;

        //Rotation Limiter:
        if (!hittingObj)
        {
            rotatorInfluenceAdj = rootJoint.rotation.eulerAngles.y + rotationOffset;
        }

        if (hittingObj)
        {
            if (collision_detect_lift.influenceOnCollision.x < 0.1)
            {
                rotatorInfluenceAdj = Mathf.Clamp(rotatorInfluenceAdj, rootJoint.rotation.eulerAngles.y - 10f, rootJoint.rotation.eulerAngles.y);
            }
            if (collision_detect_lift.influenceOnCollision.x > 0.1)
            {
                rotatorInfluenceAdj = Mathf.Clamp(rotatorInfluenceAdj, rootJoint.rotation.eulerAngles.y, rootJoint.rotation.eulerAngles.y + 10f);
            }
        }
        //End

        Quaternion pivRotater = Quaternion.Euler(pivot.rotation.eulerAngles.x, rootJoint.rotation.eulerAngles.y - 90f, pivot.rotation.eulerAngles.z);
        
        Quaternion rotater = Quaternion.Euler(0f, rotatorInfluenceAdj, 0f);

        pivot.rotation = pivRotater;
        RB.MoveRotation(rotater);

    }

    void OnMoveLeftAxis(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
    void OnMoveRightAxis(InputValue value)
    {
        inputVector2 = value.Get<Vector2>();
    }
}
