using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Menu{
    MiniGameSetupMenu
}

public class CanvasManager : MonoBehaviour{
    //INSTANCES
    public static CanvasManager instance = null;
    List<MenuController> menuControllersList;
    MenuController lastActiveMenu;

    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null){
            Destroy(gameObject);
        }

        menuControllersList = GetComponentsInChildren<MenuController>().ToList();
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
        }
        else{
            Debug.LogWarning("Desired menu was not found D:");
        }

    }

    public void CloseMenu(){
        if(lastActiveMenu != null){
            lastActiveMenu.gameObject.SetActive(false);
        }
    }
}
