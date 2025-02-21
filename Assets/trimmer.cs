using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class trimmer : MonoBehaviour
{
    public Transform trimmerHead;
    public static float inputRightTrigger = 0f;
    //public SkinnedMeshRenderer skinnedMeshRenderer1; // Assign this via the Inspector or dynamically
    //public SkinnedMeshRenderer skinnedMeshRenderer2;
    public int blendShapeIndex = 0; // Index of the blend shape to modify (starts at 0)
    public float blendWeight = 100f; // Blend weight (0 to 100)

    public Collider trimmerCollider;

    void Start()
    {
        trimmerCollider.enabled = false;
    }

    void Update()
    {
        Quaternion trimmerRotate = trimmerHead.localRotation;

        trimmerRotate *= Quaternion.Euler(0, inputRightTrigger * 120f, 0);

        trimmerHead.localRotation = trimmerRotate;

        //skinnedMeshRenderer1.SetBlendShapeWeight(blendShapeIndex, 100f - inputRightTrigger*100f);
        //skinnedMeshRenderer2.SetBlendShapeWeight(blendShapeIndex, 100f - inputRightTrigger *100f);

        if(inputRightTrigger < 0.2f)
        {
            trimmerCollider.enabled = false;
        }
        else
        {
            trimmerCollider.enabled = true;
        }

    }
    void OnTriggerRight(InputValue value)
    {
        inputRightTrigger = value.Get<float>();
    }
}
