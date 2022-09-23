using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Menu{MiniGameSetupMenu, PauseMenu, CharacterSelectionMenu}

public class CanvasManager : MonoBehaviour{
    //INSTANCES
    public static CanvasManager instance = null;
    [SerializeField] private List<MenuController> menuControllersList;
    [SerializeField] private List<MenuController> allActiveMenus = new List<MenuController>();
    [SerializeField] private MenuController lastActiveMenu;

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

    public void SwitchMenu(Menu _menu){
        if(lastActiveMenu != null){
            lastActiveMenu.gameObject.SetActive(false);
        }

        MenuController desiredMenu = menuControllersList.Find(x => x.menu == _menu);
        if(desiredMenu != null){
            desiredMenu.gameObject.SetActive(true);
            lastActiveMenu = desiredMenu;

            PauseMenu.instance.Pause();
        }
        else{
            Debug.LogWarning("Desired menu was not found D:");
        }

    }

    public void CloseMenu(){
        if(lastActiveMenu != null){
            allActiveMenus.Remove(lastActiveMenu);
            lastActiveMenu.gameObject.SetActive(false);
        }
        if(allActiveMenus.Count() > 0){
            lastActiveMenu = allActiveMenus[allActiveMenus.Count() - 1];
        }
        if(allActiveMenus.Count() == 0){
            PauseMenu.instance.Resume();
        }
    }

    public void OpenMenu(Menu _menu){
        MenuController desiredMenu = menuControllersList.Find(x => x.menu == _menu);
        if(desiredMenu != null){
            desiredMenu.gameObject.SetActive(true);
            allActiveMenus.Add(desiredMenu);
            lastActiveMenu = desiredMenu;

            PauseMenu.instance.Pause();
            Debug.Log("CanvasManager OpenMenu() desiredMenu != null");
        }
        else{
            Debug.LogWarning("Desired menu was not found D:");
        }
    }
}
