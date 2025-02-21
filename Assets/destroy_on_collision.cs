using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_on_collision : MonoBehaviour
{
    public string collider_name;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerExit(Collider Hit)
    {
        if(Hit.tag == collider_name)
        {
            Destroy(gameObject);
        }
    }
}
