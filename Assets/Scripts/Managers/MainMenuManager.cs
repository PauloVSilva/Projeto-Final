using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour{
    private void OnEnable(){
        InitializeMainMenu();
    }

    private void InitializeMainMenu()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        CanvasManager.Instance.OpenMenu(Menu.MainMenu);
    }
}