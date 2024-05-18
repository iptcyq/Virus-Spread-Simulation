using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// mainly to handle UI - nothing important
public class GameController : MonoBehaviour
{
    public GameObject ui;
    public GameObject settings;
    private bool enabled = false;
    private bool settingsEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // restart simulation
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            ChangeScene("");
        }
        
        // enable/ disable settings ui
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            enabled = !enabled;
            ui.SetActive(enabled);
        }

        // pausing/ playing simulation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else if (Time.timeScale == 0)
            {

                Time.timeScale = 1;
            }
        }

        // escape to main menu - or open another settings menu idk yet
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsEnabled) { Time.timeScale = 1; }
            else { Time.timeScale = 0; }

            settingsEnabled = !settingsEnabled;
            settings.SetActive(settingsEnabled);
            // SceneManager.LoadScene("Menu");
        }
    }

    public void ChangeScene(string SceneName)
    {
        if (SceneName == null || SceneName == "")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reload current scene
        }
        else
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
