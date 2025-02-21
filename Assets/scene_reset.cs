using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class scene_reset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        reset();
    }

    // Update is called once per frame
    void Update()
    {
        if(score.pressBack > 0.2)
        {
            reset();
        }
    }

    void reset()
    {
        equipHandler.isChainsaw = false;
        equipHandler.isTrimmer = false;
        equipHandler.isMower = false;
        equipHandler.isDebris = false;

        score.Damage = 0f;
        score.elapsedTime = 0f;
        score.DebrisRemaining = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}