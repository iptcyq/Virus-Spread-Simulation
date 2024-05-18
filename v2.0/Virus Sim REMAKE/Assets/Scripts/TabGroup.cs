using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabbuttons;

    //public for debugging
    public TabButton selectedTab;

    public List<GameObject> pages;

    public void Enter(TabButton button)
    {
        if (tabbuttons == null)
        {
            tabbuttons = new List<TabButton>();
        }

        tabbuttons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        //FindObjectOfType<AudioManager>().Play("click");
        selectedTab = button;
        ResetTabs();

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == index) { pages[i].SetActive(true); }
            else { pages[i].SetActive(false); }
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabbuttons)
        {
            if (selectedTab != null && button == selectedTab)
            {
                continue;
            }
        }
    }

}