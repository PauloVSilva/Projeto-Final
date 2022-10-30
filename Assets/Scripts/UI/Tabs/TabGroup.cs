using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons = new List<TabButton>();
    public List<Image> tabLabels = new List<Image>();
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabButton selectedTab;
    public int index;
    public ButtonType[] buttonTypes;

    private void Awake(){
        tabButtons = GetComponentsInChildren<TabButton>().ToList();

        List<Image> images = GetComponentsInChildren<Image>().ToList();
        tabLabels.Add(images.First());
        tabLabels.Add(images.Last());
    }

    private void OnEnable(){
        index = 0;
        StartCoroutine(OnEnableDelay());
        IEnumerator OnEnableDelay(){
            yield return new WaitForSecondsRealtime(0.01f);
            OnTabSelected(tabButtons[index]);
        }
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

        //index = button.transform.GetSiblingIndex();
        index = tabButtons.FindIndex(a => a == button);
    }
}
