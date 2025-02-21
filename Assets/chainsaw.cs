using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class chainsaw : MonoBehaviour
{
    public Collider chainsawCollider;
    private float inputRightTrigger = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       if(equipHandler.isChainsaw)
        {
            if (inputRightTrigger > 0.2)
            {
                chainsawCollider.isTrigger = true;
            }
            else
            {
                chainsawCollider.isTrigger = false;
            }
        }
    }

    void OnTriggerRight(InputValue value)
    {
        inputRightTrigger = value.Get<float>();
    }
}
