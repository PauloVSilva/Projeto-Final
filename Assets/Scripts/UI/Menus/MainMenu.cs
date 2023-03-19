using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MenuController
{
    #region BUTTONS
    public void Play()
    {
        CanvasManager.Instance.CloseMenu();
        LevelLoader.Instance.LoadLevel("MainHub");
    }

    public void Controls()
    {
        CanvasManager.Instance.OpenMenu(Menu.ControlsMenu);
    }

    public void Settings()
    {
        CanvasManager.Instance.OpenMenu(Menu.SettingsMenu);
    }

    public void Credits()
    {
        Debug.Log("Credits");
    }

    public void Quit()
    {
        LevelLoader.Instance.QuitGame();
    }
    #endregion BUTTONS
}
