using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Menu{MainMenu, ControlsMenu, SettingsMenu, MiniGameSetupMenu, PauseMenu, CharacterSelectionMenu}
public enum ButtonType{Back, Submit, Navigate, Interact}

[System.Serializable]
public class CanvasButtonDisplay{
    public ButtonType buttonType;
    public string buttonString;
    public Sprite[] buttonSprite;

}

public class CanvasManager : MonoBehaviour{
    //INSTANCES
    public static CanvasManager instance = null;
    public GameObject playerPanels;
    public GameObject miniGameUI;
    [SerializeField] private List<MenuController> menuControllersList;
    [SerializeField] private List<MenuController> allActiveMenus = new List<MenuController>();
    [SerializeField] public MenuController currentMenu;
    [SerializeField] public List<CanvasButtonDisplay> canvasButtonsList;
    [SerializeField] public GameObject buttonDisplayPrefab;

    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null){
            Destroy(gameObject);
        }

        //menuControllersList = GetComponentsInChildren<MenuController>().ToList();
        menuControllersList.ForEach(x => x.gameObject.SetActive(false)); //x is menuController
        //SwitchMenu(Menu.MiniGameSetupMenu); this line is only used if I want a specific menu to pop up as soon as the scene opens
    }

    public void OpenMenu(int menuIndex){
        OpenMenu((Menu)menuIndex);
    }

    public void SwitchMenu(int menuIndex){
        SwitchMenu((Menu)menuIndex);
    }

    public void OpenMenu(Menu _menu){
        MenuController desiredMenu = menuControllersList.Find(x => x.menu == _menu);
        if(desiredMenu != null){
            desiredMenu.gameObject.SetActive(true);
            allActiveMenus.Add(desiredMenu);
            currentMenu = desiredMenu;
            currentMenu.firstSelected.Select();
        }
        else{
            Debug.LogWarning("Desired menu was not found D:");
        }
    }

    public void SwitchMenu(Menu _menu){
        CloseMenu();
        OpenMenu(_menu);
    }

    public void CloseMenu(){
        if(currentMenu != null){
            allActiveMenus.Remove(currentMenu);
            currentMenu.gameObject.SetActive(false);
        }
        if(allActiveMenus.Count() > 0){
            currentMenu = allActiveMenus[allActiveMenus.Count() - 1];
            currentMenu.firstSelected.Select();
        }
        else{
            currentMenu = null;
            PauseMenu.instance.Resume();
        }
    }
}
