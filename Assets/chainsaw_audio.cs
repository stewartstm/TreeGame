using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

public class chainsaw_audio : MonoBehaviour
{
    public bool cutting = false;

    public ParticleSystem chipEmitter;

    private float inputRightTrigger = 0f;
    public float defaultPitch = 0.7f;
    private float enginePitch;


    AudioSource engine;

    // Start is called before the first frame update
    void Start()
    {
        engine = GetComponent<AudioSource>();
        enginePitch = defaultPitch;
    }

    // Update is called once per frame
    void Update()
    {
        engine.volume = inputRightTrigger * 1.4f;
        if(!cutting)
        {
            if (inputRightTrigger > 0.1f)
            {
                engine.pitch = Mathf.Lerp(enginePitch, defaultPitch * inputRightTrigger, 10f * Time.deltaTime);
            }
            if (inputRightTrigger < 0.1f)
            {
                engine.pitch = Mathf.Lerp(enginePitch, defaultPitch, 10f * Time.deltaTime);
            }
        }
            
    }

    void OnTriggerStay(Collider hit)
    {
        var emission = chipEmitter.emission;

        if (equipHandler.isChainsaw && inputRightTrigger > 0.2f)
        {
            if (hit.gameObject.tag == "Cutable" || hit.gameObject.tag == "WoodDebris" || hit.gameObject.tag == "tree_base")
            {
                //planeDebug.SetActive(true);
                emission.rateOverTime = 1000f;
                cutting = true;
                engine.pitch = Mathf.Lerp(enginePitch, defaultPitch * 0.5f, 10f * Time.deltaTime);
            }
            
        }
        
    }

    void OnTriggerExit(Collider hit)
    {
        var emission = chipEmitter.emission;
        if (equipHandler.isChainsaw && inputRightTrigger > 0.2f)
        {
            //Vector3 chainsawAngle = chainsawBase.transform.rotation.eulerAngles;
            if (hit.gameObject.tag == "Cutable" || hit.gameObject.tag == "WoodDebris" || hit.gameObject.tag == "tree_base")
            {
                emission.rateOverTime = 0f;
                //planeDebug.SetActive(false);
                cutting = false;
                engine.pitch = Mathf.Lerp(enginePitch, defaultPitch, 10f * Time.deltaTime);
            }
        }
    }

    void OnTriggerRight(InputValue value)
    {
        inputRightTrigger = value.Get<float>();
    }

}

