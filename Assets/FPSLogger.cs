using UnityEngine;

public class FPSLogger : MonoBehaviour
{
    public static float fps;
    private float deltaTime = 0.0f;

    void Update()
    {
        // Calculate the delta time
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // Calculate FPS
        fps = Mathf.Round(1.0f / deltaTime);

        // Log FPS to the console
        //Debug.Log("FPS: " + Mathf.Ceil(fps));
    }
}