using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour
{

    public GameObject sliders;
    private bool enable = true;

    // Start is called before the first frame update
    void Start()
    {
        enable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        //disable and enable the ui
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            enable = !enable;
            sliders.SetActive(enable);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else if (Time.timeScale == 0)
            {

                Time.timeScale = 1;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.R))
            {
                //complete reset
                PlayerPrefs.SetInt("Simul Number", 1);
            }
        }
    }

}
