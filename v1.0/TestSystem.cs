using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TestSystem : MonoBehaviour
{
    public GameObject gc;
    private UnitController uc;
    public int totalInfected;
    public float probRangeMax;
    public float probRangeMin;

    private bool ended = false;
    
    void Awake()
    {
        
        uc = gc.GetComponent<UnitController>();
        int test = PlayerPrefs.GetInt("Simul Number") + 1;
        PlayerPrefs.SetInt("Simul Number", test);
        uc.simul = test;
        Time.timeScale = 1;
        ended = false;
    }


    // Update is called once per frame
    void Update()
    {
        //check when 0 infected
        //for the sake of testing for time scale, change to when 0 susceptible for now)
        if (uc.noOfInfected <= 0 && ended == false)
        {
            //stop simulation and print result
            Time.timeScale = 0;
            uc.PrintResults();
            ended = true;
            //load next simulation
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //done manually now
        }
    }
    
}
