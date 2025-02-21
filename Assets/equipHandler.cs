using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class equipHandler : MonoBehaviour
{
    private float pressX = 0f;

    public static float pressB = 0f;
    public static float inputRightTrigger = 0f;

    private float t = 0f;
    private float pickupTimer = 0.3f;
    private float maxCarryWeight = 35f;

    public Transform dismountLocation;
    public Transform debrisHolder;
    public Transform playerPosition;

    public Rigidbody playerRB;

    public GameObject chainsaw;
    public GameObject trimmer;
    public GameObject mower;
    public GameObject eqTool;
    public GameObject cherryPicker;
    public GameObject objInReach;

    public bool pickUp = false;
    public static bool isChainsaw = false;
    public static bool isTrimmer = false;
    public static bool isMower = false;
    public static bool isDebris = false;
    public static bool isCherryPicker = false;

    private float cooldown = 0f;

    public camera cameraScript;

    Collider playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cooldown += 1f * Time.deltaTime;

        if(cooldown < 1f)
        {
            return;
        }

        objInReach = collision_detect.tool;

        if (collision_detect.tool != null)
        {
            CollisionDetector();
        }

        if(pressX > 0.5)
        {
            t += 1f * Time.deltaTime;
        }
        else
        {
            t = 0f;
        }

        if (pickUp && isDebris == false)
        {
            eqTool.SetActive(false);

            if (isChainsaw)
            {
                chainsaw.SetActive(true);
                eqTool.transform.position = chainsaw.transform.position;
                eqTool.transform.rotation = chainsaw.transform.rotation;
            }

            if (isTrimmer)
            {
                trimmer.SetActive(true);
                eqTool.transform.position = trimmer.transform.position;
                eqTool.transform.rotation = trimmer.transform.rotation;
            }

            if(isMower)
            {
                playerRB.isKinematic = true;
                playerCollider.enabled = false;
                eqTool.transform.position = mower.transform.position;
                eqTool.transform.rotation = mower.transform.rotation;
                transform.position = mower.transform.position;
                transform.rotation = mower.transform.rotation;
                mower.SetActive(true);
                cameraScript.enabled = false;

                if (pressB > 0.5f)
                {
                    playerRB.isKinematic = false;
                    transform.position = dismountLocation.position;
                    transform.rotation = dismountLocation.rotation;
                    playerCollider.enabled = true;
                    mower.SetActive(false);
                    cameraScript.enabled = true;
                }
            }

            if (pressB > 0.5f && !isCherryPicker)
            {
                eqTool.SetActive(true);
                eqTool.transform.position = eqTool.transform.position;
                eqTool.transform.rotation = eqTool.transform.rotation;
                eqTool = null;
                pickUp = false;
                isChainsaw = false;
                isMower = false;
                isTrimmer = false;
                trimmer.SetActive(false);
                chainsaw.SetActive(false);
                isDebris = false;
            }
        }

        if(pickUp && isDebris)
        {
            Vector3 debrisPos = eqTool.transform.position;
            Quaternion debrisRot = eqTool.transform.rotation;

            Rigidbody rb = eqTool.GetComponent<Rigidbody>();
            rb.useGravity = false;
            eqTool.transform.position = Vector3.Lerp(debrisPos,debrisHolder.position,1f*Time.fixedTime);
            eqTool.transform.rotation = Quaternion.Lerp(debrisRot, debrisHolder.rotation, 1f * Time.fixedTime);

            if (pressB > 0.5f)
            {
                rb.useGravity = true;
                pickUp = false;
                isDebris = false;
                eqTool = null;
            }

        }

        if(isCherryPicker)
        {
            transform.rotation = playerPosition.rotation;

            transform.parent = playerPosition;
            playerRB.isKinematic = true;
            if(pressB > 0.5f)
            {
                transform.parent = null;
                transform.position = transform.position;
                transform.rotation = transform.rotation;
                playerRB.isKinematic = false;
                isCherryPicker = false;
                cooldown = 0f;
            }
        }

    }

    void OnTriggerStay(Collider hit)
    {
        if (hit.tag == "cherryPicker" && t > pickupTimer)
        {
            isCherryPicker = true;
        }
    }

    void CollisionDetector()
    {
        if (collision_detect.tool.tag == "chainsaw" && t > pickupTimer && pickUp == false)
        {
            eqTool = collision_detect.tool;
            pickUp = true;
            isChainsaw = true;
        }

        if (collision_detect.tool.tag == "trimmer" && t > pickupTimer && pickUp == false)
        {
            eqTool = collision_detect.tool;
            pickUp = true;
            isTrimmer = true;
        }

        if (collision_detect.tool.tag == "mower" && t > pickupTimer && pickUp == false)
        {
            eqTool = collision_detect.tool;
            pickUp = true;
            isMower = true;
        }

        if (collision_detect.tool.tag == "WoodDebris" && t > pickupTimer && pickUp == false)
        {
            float debrisSize = Mathf.Round(collision_detect.tool.GetComponent<MeshFilter>().mesh.bounds.size.magnitude * 10f);
            if (debrisSize > maxCarryWeight)
            {
                Debug.Log("Excedes Carry Weight: " + debrisSize + "lbs/ " + maxCarryWeight + "lbs!");
                return;
            }

            eqTool = collision_detect.tool;
            pickUp = true;
            isDebris = true;
        }
    }

    void OnPressX(InputValue value)
    {
        pressX = value.Get<float>();
    }

    void OnPressB(InputValue value)
    {
        pressB = value.Get<float>();
    }

    void OnTriggerRight(InputValue value)
    {
        inputRightTrigger = value.Get<float>();
    }

}
