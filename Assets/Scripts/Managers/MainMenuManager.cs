using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour{
    private void OnEnable(){
        InitializeMainMenu();
    }

    private void InitializeMainMenu(){
        GameManager.Instance.joinAction.Disable();
        GameManager.Instance.miniGameIsRunning = false;

        CanvasManager.instance.playerPanels.SetActive(false);
        CanvasManager.instance.miniGameUI.SetActive(false);
        CanvasManager.instance.OpenMenu(Menu.MainMenu);
    }
}