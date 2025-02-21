using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vibration : MonoBehaviour

{
    public float frequency = .1f;
    public float throttle = .1f;
    private Vector3 objectPosiiton = new Vector3(1, 1, 1);
    private float timer = 0f;
    private bool startCalling = false;

    // Start is called before the first frame update
    void Start()
    {
        objectPosiiton = transform.localPosition;
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
        yield return new WaitForSeconds(1f);
        startCalling = true;
    }

    void Update()
    {
        if (startCalling)
        {
            timer += Time.deltaTime; // Increment timer by time passed since the last frame
            if (timer >= frequency)
            {
                MyMethod();
                timer = 0f; // Reset timer after method call
            }
            if (timer >= frequency / 2)
            {
                transform.localPosition = new Vector3(objectPosiiton.x + -throttle * 2f, objectPosiiton.y + -throttle * 2f, objectPosiiton.z + -throttle * 2f);
            }
        }
    }

    void MyMethod()
    {
        transform.localPosition = new Vector3(objectPosiiton.x + throttle, objectPosiiton.y + throttle, objectPosiiton.z + throttle);
    }
}
