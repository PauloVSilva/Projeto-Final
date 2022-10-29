using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons = new List<TabButton>();
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabButton selectedTab;
    public int index;

    private void Awake(){
        tabButtons = GetComponentsInChildren<TabButton>().ToList();
    }

    private void OnEnable(){
        index = 0;
        OnTabSelected(tabButtons[index]);
    }

    public void SelectNextTab(){
        index++;
        if(index == tabButtons.Count)
        {
            index = 0;
        }
        OnTabSelected(tabButtons[index]);
    }

    public void SelectPreviousTab(){
        index--;
        if(index < 0)
        {
            index = tabButtons.Count - 1;
        }
        OnTabSelected(tabButtons[index]);
    }

    public void OnTabHovered(TabButton button)
    {
        if(selectedTab == null || button != selectedTab)
        {
            button.SetBackground(tabHover);
        }
    }

    public void OnTabExit(TabButton button)
    {
        if(button != selectedTab)
        {
            button.SetBackground(tabIdle);
        }
    }

    public void OnTabSelected(TabButton button)
    {
        selectedTab?.Deselect();

        selectedTab = button;

        selectedTab.Select();

        index = button.transform.GetSiblingIndex();
    }
}
