using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class score : MonoBehaviour
{
    public float levelScoreMinimum = 80f;
    public float overallScore = 0f;
    public float timeCompleteMinimum = 240f;

    private float pressStart = 0f;
    public static float pressBack = 0f;
    private float t = 0f;

    public static float grassRemaining = 0f;
    public static float GrassPointsAtStart = 0f;
    public static float elapsedTime;
    public static float Damage;
    public static float DebrisRemaining;
    public static float TreesRemaining;

    public bool levelComplete = false;
    public static bool gamePause = false;

    static bool grassCut = false;
    static bool debrisCleaned = false;
    static bool treesCut = false;

    public TMP_Text grassScore;
    public TMP_Text stopwatch;
    public TMP_Text damageTotal;
    public TMP_Text debrisTotal;
    public TMP_Text doneButton;
    public TMP_Text frames;
    public TMP_Text trees;

    public TMP_Text timeScore;
    public TMP_Text damageScore;
    public TMP_Text finalScore;

    private bool called = false;

    public GameObject endScreen;
    

    // Start is called before the first frame update
    void Start()
    {
        DebrisRemaining = GameObject.FindGameObjectsWithTag("WoodDebris").Length;
        TreesRemaining = GameObject.FindGameObjectsWithTag("tree_base").Length;
        doneButton.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        frames.text = $"{FPSLogger.fps}";

        if (levelComplete)
        {
            if(gamePause)
            {
                t += 1 * Time.deltaTime;
                if(t > 4f && pressStart > 0.2f)
                {
                    Application.Quit();
                }
                //Debug.Log(t);
                return;
            }         
                CalculateScore();
                ActivateEndScreen();
                gamePause = true;          
        }

        if(pressBack > 0.2f)
        {
            reset();
            return;
        }

        GrassScore();
        Stopwatch();
        DamageCalculator();
        DebrisCalculator();
        goalTracker();
        TreesCalculator();
    }

    void ActivateEndScreen()
    {
        endScreen.SetActive(true);
        damageScore.text = $"{Damage}$";
        timeScore.text = stopwatch.text;
        finalScore.text = $"{overallScore}%";
    }

    void goalTracker()
    {
        if(grassCut && debrisCleaned)
        {
            doneButton.color = Color.white;

            if(pressStart > 0.2f)
            {
                levelComplete = true;
            }

        }
    }

    void CalculateScore()
    {
        float timeScore = Mathf.Clamp(Mathf.Abs(elapsedTime - timeCompleteMinimum),0,40f);
        float damageDeduction = Damage * 0.5f;

        if (elapsedTime < timeCompleteMinimum)
        {
            overallScore = Mathf.Clamp(100f - damageDeduction,0,100);
        }
        if (elapsedTime > timeCompleteMinimum)
        {
            overallScore = Mathf.Clamp(100f - damageDeduction - timeScore,0,100);
        }
        overallScore = Mathf.Round(overallScore);
    }

    void DebrisCalculator()
    {
        DebrisRemaining = DebrisRemaining;
        //float DebrisPercentage = DebrisRemaining/

        debrisTotal.text = $"{DebrisRemaining}";
        //Debug.Log(DebrisRemaining);

        if(DebrisRemaining == 0)
        {
            debrisCleaned = true;
        }
    }

    void TreesCalculator()
    {
        //TreesRemaining = TreesRemaining;
        //float DebrisPercentage = DebrisRemaining/

        trees.text = $"{TreesRemaining}";
        //Debug.Log(DebrisRemaining);

        if (TreesRemaining == 0)
        {
            treesCut = true;
        }
        else
        {
            treesCut = false;
        }
    }

    void DamageCalculator()
    {
        damageTotal.text = $"{Damage}$";
    }

    void Stopwatch()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milseconds = Mathf.FloorToInt(elapsedTime*100f % 100);

        stopwatch.text = $"{minutes:D2}:{seconds:D2}:{milseconds:D2}";

    }

    void GrassScore()
    {
        if (!called)
        {
            GrassPointsAtStart = GameObject.FindGameObjectsWithTag("grass").Length;
            called = true;
        }

        grassRemaining = GameObject.FindGameObjectsWithTag("grass").Length;
        float FToP = grassRemaining / GrassPointsAtStart * 100f;
        float RoundedScore = Mathf.Round(FToP);

        grassScore.text = $"{RoundedScore}%";

        if(RoundedScore == 0f)
        {
            grassCut = true;
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

        endScreen.SetActive(false);

        doneButton.color = Color.black;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnPressStart(InputValue value)
    {
        pressStart = value.Get<float>();
    }

    void OnPressBack(InputValue value)
    {
        pressBack = value.Get<float>();
    }
}
