using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour{
    public static bool GameIsPaused;

    public GameObject pauseMenuUI;

    private void Awake() {
        GameIsPaused = false;
    }

    private void Start() {
        PlayerInputHandler.OnCharacterPressMenuButton += ChangeGameStatus;
    }

    private void OnDisable() {
        PlayerInputHandler.OnCharacterPressMenuButton -= ChangeGameStatus;
    }

    private void ChangeGameStatus(){
        if(GameIsPaused){
            Resume();
        }
        else{
            Pause();
        }
    }

    public void Pause(){
        GameManager.instance.joinAction.Disable();
        GameManager.instance.leaveAction.Disable();
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.SwitchCurrentActionMap("Menu");
        }
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume(){
        GameManager.instance.joinAction.Enable();
        GameManager.instance.leaveAction.Enable();
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.SwitchCurrentActionMap("Player");
        }
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void QuitToMainMenu(){
        Time.timeScale = 1f;
        GameManager.instance.ReturnToMainMenu();
    }
}
