using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using TMPro;


public class UnitController : MonoBehaviour
{
    public int noOfInfected = 1;
    public int noOfRemoved = 0;
    public int noOfSusceptible = 1; // susceptible, aka uninfected
    public int totalPpl = 0;

    public Component[] units;

    public float dayTime = 10f; // amount of time equalling to a week
    private bool day = false;
    public int days = 0;

    private float timer = 0f;

    private bool gizmosEnabled = false;

    [Space]
    public Slider proximity; // proximity before getting a chance to be infected

    public Slider recoveryRate; // time before recovered/ dies = removed
    public TextMeshProUGUI recoveryText; 

    public Slider movementRestrictions; // extent of movement/ speed of units

    public Slider infectionRate; // probabilty of infection

    public Slider infectionVaccRateDose; // probability of infection after vaccination

    public Slider vaccRateDose; // percentage of vaccinated people

    public Slider totalNumber;
    public TextMeshProUGUI buttonText;

    public GameObject unit; // default susceptible unit

    public Slider dayTimer; // countdown bar to show each week
    public TextMeshProUGUI daysText; // display number of weeks passed
    public TextMeshProUGUI consoletext; // log text

    // Start is called before the first frame update
    void Start()
    {
        units = GetComponentsInChildren<Unit>();
        totalPpl = units.Length;

        SpawnUnits(20);
        ChangeProximity();

        // StartCoroutine("SimulationTime");
    }

    // Update is called once per frame
    void Update()
    {
        //time loop for each day
        if (day == false)
        {
            day = true; // flag to start counting a new day (without using recursion in iEnumerate since I have a bad feeling about it)
            StartCoroutine("dayClock");
        }
        timer += Time.deltaTime;
        if (timer >= dayTime) { timer -= dayTime; }
        dayTimer.value = timer / dayTime;
        daysText.text = days.ToString() + " week(s)";

        //changing values in units through the sliders
        // future me here: this is so performance unfriendly 
        // oh well too bad
        foreach (Unit unit in units)
        {
            //unit.infectionRadius = proximity.value;
            unit.removedTime = recoveryRate.value * dayTime; 
            unit.speed = movementRestrictions.value;
            unit.probabilityOfInfection = infectionRate.value;

            if (unit.vaccinated)
            {
                //if vaccinated, get different probability of infection
                unit.probabilityOfInfection = infectionVaccRateDose.value;
            }
            else
            {
                unit.probabilityOfInfection = infectionRate.value;
            }
        }
        
    }

    public void SpawnUnits() // function to spawn more units in game
    {
        SpawnUnits((int)totalNumber.value);
    }
    public void SpawnUnits(int noToSpawn)
    {
        //int noToSpawn = totalNumber.value;
        for (int i = 0; i < noToSpawn; i++)
        {
            GameObject obj = Instantiate(unit, this.gameObject.transform.position, Quaternion.identity);
            obj.transform.parent = this.gameObject.transform;
            if ((i / noToSpawn) < vaccRateDose.value)
            {
                //if less than infection rate, gets the infectionRate of a vaccinated person of dose 2
                obj.GetComponent<Unit>().probabilityOfInfection = infectionVaccRateDose.value;
                obj.GetComponent<Unit>().vaccinated = true;
            }
            else
            {
                obj.GetComponent<Unit>().probabilityOfInfection = infectionRate.value;
                obj.GetComponent<Unit>().vaccinated = false;
            }

            units = GetComponentsInChildren<Unit>();
        }
        totalPpl += noToSpawn;

        //Debug.Log("Spawned " + noToSpawn + " units");
        consoletext.text = "Spawned " + noToSpawn + " units";
    }

    public void ChangeProximity() // change infection radius and display of all units
    {
        foreach (Unit unit in units)
        {
            unit.infectionRadius = proximity.value;
            unit.ChangeProximity();
        }
    }

    public void ChangeRecovery()
    {
        recoveryText.text = recoveryRate.value + " week(s)";
    }

    public void SliderButton()
    {
        buttonText.text = "spawn " + totalNumber.value;
    }
    public void ShowGizmos()
    {
        gizmosEnabled = !gizmosEnabled;
        foreach (Unit unit in units)
        {
            unit.line.enabled = gizmosEnabled;
        }
    }

    private IEnumerator dayClock() // "day clock" to simulate 1 real world week in terms of game time
    {
        yield return new WaitForSeconds(dayTime); // wait for a certain amount of time before calling the end of the week
        days++;

        //to check number of infected
        noOfInfected = 0;
        noOfRemoved = 0;
        foreach (Unit controller in units)
        {
            if (controller.currentState == Unit.state.removed)
            {
                noOfRemoved++;
            }
            else if (controller.currentState == Unit.state.infected)
            {
                noOfInfected++;
            }

        }
        day = false; // flag to start counting new week

        noOfSusceptible = totalPpl - noOfRemoved - noOfInfected;
        consoletext.text = days + " WEEK(S) || " + "UNINFECTED: " + noOfSusceptible + " INFECTED: " + noOfInfected + " REMOVED: " + noOfRemoved;
    }

    // if running the time based simulation (pg 7 of our report)
    // time based one is where we run the simulation over a time of T
    // record number of infections I directly caused by patient zero
    // not in use for display version of simulation on itch.io
    IEnumerator SimulationTime()
    {
        // We found the infection phase T to be a median of 43 seconds 
        yield return new WaitForSeconds(43); 
        PrintResults();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // used to print results when we were mass collecting data for our report, not used on itch.io
    public void PrintResults()
    {
        consoletext.text = "Patient zero infected " + (noOfInfected - 1) + " units";
    }
}
