using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class UnitController : MonoBehaviour
{
    public int noOfInfected = 1;
    public int noOfRemoved = 0;
    public int noOfSusceptible = 1;
    public int totalPpl = 0;

    public Component[] units;

    public float dayTime = 50f;
    private bool day = false;
    public int days = 0;

    private float timer = 0f;
    public int sec = 0;

    [Space]
    public Slider proximity;
    public Slider recoveryRate;
    public Slider movementRestrictions;
    public Slider infectionRate;
    public Slider infectionVaccRateDose;
    public Slider vaccRateDose;

    public Slider totalNumber;
    public GameObject unit;

    public Text consoletext;
    
    [HideInInspector]
    public int simul;

    // Start is called before the first frame update
    void Start()
    {
        units = GetComponentsInChildren<unit>();
        totalPpl = units.Length;
        days = 0;

        totalNumber.value = 20;
        SpawnUnits();
        
        noOfInfected = 1;
        noOfSusceptible = 1;

        StartCoroutine("SimulationTime");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        sec = (int)(days*dayTime) + (int)(timer % 60);
        
        units = GetComponentsInChildren<unit>();
        totalPpl = units.Length;

        //for each day
        if (day == false)
        {
            day = true;
            StartCoroutine("dayClock");
        }

        //changing values in units
        foreach (unit unit in units)
        {
            unit.infectionRadius = proximity.value;
            unit.removedTime = recoveryRate.value * dayTime; //daytime would be the accurate 1 day
            unit.speed = movementRestrictions.value;
            //unit.probabilityOfInfection = infectionRate.value;
            
            if (unit.vaccinated == true)
            {
                unit.probabilityOfInfection = infectionVaccRateDose.value; //if vaccinated at one dose
            }
        }
        

        //temporary
        noOfInfected = 0;
        foreach (unit controller in units)
        {
            if (controller.infected == true)
            {
                noOfInfected++;
            }
        }
        noOfSusceptible = totalPpl - noOfInfected;



    }

    public void SpawnUnits()
    {
        for(int i = 0; i < totalNumber.value; i++)
        {
            GameObject obj = Instantiate(unit,this.gameObject.transform.position,Quaternion.identity);
            obj.transform.parent = this.gameObject.transform;
            if ((i / totalNumber.value) < vaccRateDose.value)
            {
                //if less than infection rate, gets the infectionRate of a vaccinated person of dose 2
                obj.GetComponent<unit>().probabilityOfInfection = infectionVaccRateDose.value;
                obj.GetComponent<unit>().vaccinated = true;
            }
            else
            {
                obj.GetComponent<unit>().probabilityOfInfection = infectionRate.value;
                obj.GetComponent<unit>().vaccinated = false;
            }
        }
        totalPpl = units.Length;
        Debug.Log("Spawned " + totalNumber.value + " units");
        consoletext.text = "Spawned " + totalNumber.value + " units";
    }

    private IEnumerator dayClock()
    {
        yield return new WaitForSeconds(dayTime);
        days++;
        //to check number of infected
        noOfInfected = 0;
        noOfRemoved = 0;
        foreach (unit controller in units)
        {
            if (controller.removed == true)
            {
                noOfRemoved++;
            }
            else if (controller.infected == true)
            {
                noOfInfected++;
            }

        }
        day = false;
        
        noOfSusceptible = totalPpl - noOfRemoved - noOfInfected;
        Debug.Log("SEC" + sec + " SUSCEPTIBLE" + noOfSusceptible + " INFECTED" + noOfInfected + " REMOVED" + noOfRemoved  );
        consoletext.text = "SEC" + sec + " SUSCEPTIBLE: " + noOfSusceptible + " INFECTED: " + noOfInfected + " REMOVED: " + noOfRemoved ;
        //WriteString("SEC" + sec +  " SUSCEPTIBLE" + noOfSusceptible + " INFECTED" + noOfInfected + " REMOVED" + noOfRemoved );
    }
    static void WriteString(string input)
    {
        string path = "Assets/Resources/simul.txt"; //swapped text file

        //Write text
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(input);
        writer.Close();
        
    }
    
    public void PrintResults()
    {
        //WriteString("New Test " + PlayerPrefs.GetInt("Simul Number") + " Seconds: " +sec); //write only at the end of a simul (copy and paste over later)
        WriteString("Test " + PlayerPrefs.GetInt("Simul Number") + " INFECTED: " + (noOfInfected -1)); //write only at the end of a simul (copy and paste over later)
    }

    public void DisableRecovery()
    {
        foreach (unit unit in units)
        {
            units = GetComponentsInChildren<unit>();
            unit.recovery = !unit.recovery;
        }
    }

    IEnumerator SimulationTime()
    {
        yield return new WaitForSeconds(43);
        PrintResults();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
