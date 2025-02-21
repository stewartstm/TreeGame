using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grass_cut : MonoBehaviour
{
    public float EmissionRate = 2500f;
    public ParticleSystem system;
    public float cuttingVolume = 1f;
    public float enginePitch = 1.19f;
    public float engineVolume = 0f;
    public float defaultPitch = 1.3f;
    //AudioSource cutting;
    static float t = 0.0f;
    public AudioClip engineClip;
    public AudioSource engine;
    public AudioSource fireEngine;
    public bool startCalling = false;
    public bool isTrimmer = false;
    public Transform grassBurst;

    // Start is called before the first frame update
    void OnEnable()
    {
            engine.clip = engineClip;
            engineVolume = 0f;
            enginePitch = .3f;

            engine.volume = engineVolume;

            StartCoroutine(StartCallingAfterDelay());

            fireEngine.PlayDelayed(.5f);
            t = -2f;
    }

    void OnDisable()
    {
        engine.volume = 0f;
    }

    IEnumerator StartCallingAfterDelay()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(1f);
        t = 0f;
        engine.Play();

        // Enable the function calls in Update
        startCalling = true;
    }
    // Update is called once per frame
    void Update()
    {

        if (startCalling)
        {
            startup();
        }

    }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the collider has the tag of the vertex collider
            if (other.gameObject.tag == "grass")
            {

            if (!isTrimmer)
            {
                {
                    var emission = system.emission;
                    var hitPosiition = other.gameObject.transform.position;
                    emission.rateOverTime = EmissionRate; // Start emitting particles
                    hitPosiition.y += 100;
                    GameObject hitObject = other.gameObject;
                    hitObject.transform.position += -Vector3.up * .5f;
                    cuttingVolume = 0.5f;
                    enginePitch = defaultPitch * 0.9f;
                    t = 0f;
                }
            }

            if(isTrimmer)
            {
                if (trimmer.inputRightTrigger > 0.1f)
                {
                    Instantiate(grassBurst, transform.position, transform.rotation);
                    var hitPosiition = other.gameObject.transform.position;
                    hitPosiition.y += 100;
                    GameObject hitObject = other.gameObject;
                    hitObject.transform.position += -Vector3.up * .5f;
                    enginePitch = defaultPitch * 0.9f;
                    t = 0f;
                }
                //cutting = true;
            }
            }
            else
            {
                //Debug.LogError("Blade Damage!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "grass")
            {
                if (equipHandler.isTrimmer == false)
                {
                Destroy(other.gameObject);
                var emission = system.emission;
                emission.rateOverTime = 0f;
                }
                if (isTrimmer && trimmer.inputRightTrigger > 0.1f)
                {
                Destroy(other.gameObject);
                t = 0f;
                }
            }

        }

    private void startup()
    {
            engine.volume = engineVolume;
            engine.pitch = enginePitch;

            t += 0.5f * Time.deltaTime;

        if (!isTrimmer)
        {
            enginePitch = Mathf.Lerp(enginePitch, defaultPitch, t * 1.5f);
            engineVolume = Mathf.Lerp(engineVolume, 1f, t);
        }

        if (isTrimmer && trimmer.inputRightTrigger > 0.1f)
        {
            enginePitch = Mathf.Lerp(enginePitch, defaultPitch, 10f * Time.deltaTime);
            engineVolume = Mathf.Lerp(engineVolume, 1f, 10f * Time.deltaTime);
        }
        if (isTrimmer && trimmer.inputRightTrigger < 0.1f)
        {
            enginePitch = Mathf.Lerp(enginePitch, 0.4f, 10f * Time.deltaTime);
            engineVolume = Mathf.Lerp(engineVolume, 0f, 10f * Time.deltaTime);
        }
    }

}
