using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MenuController{
    public static MainMenu instance = null;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }


    #region BUTTONS
    public void Play(){
        CanvasManager.instance.CloseMenu();
        LevelLoader.instance.LoadLevel("MainHub");
    }

    public void Controls(){
        CanvasManager.instance.OpenMenu(Menu.ControlsMenu);
    }

    public void Settings(){
        CanvasManager.instance.OpenMenu(Menu.SettingsMenu);
    }

    public void Credits(){
        Debug.Log("Credits");
    }

    public void Quit(){
        LevelLoader.instance.QuitGame();
    }
    #endregion BUTTONS
}
