using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_over_time : MonoBehaviour
{
    public float timer = 3f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartCallingAfterDelay());
    }

    IEnumerator StartCallingAfterDelay()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(timer);

        // Enable the function calls in Update
        Destroy(gameObject);
    }

}
