using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class OldPauseMenu : MonoBehaviour{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button firstSelected;
    [SerializeField] private Button DropOutButton;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private void Start() {
        //playerInputHandler.OnCharacterPressMenuButton += ChangeGameStatus;
    }

    private void OnDestroy() {
        //playerInputHandler.OnCharacterPressMenuButton -= ChangeGameStatus;
    }

    private void ChangeGameStatus(PlayerInput _playerInput){
        if(GameManager.instance.miniGameIsRunning){
            DropOutButton.interactable = false;
        }
        else{
            DropOutButton.interactable = true;
        }


        if(GameManager.instance.gameIsPaused){
            Resume();
        }
        else{
            Pause();
        }
    }

    public void Pause(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.SwitchCurrentActionMap("UI");
        }
        pauseMenuUI.SetActive(true);
        firstSelected.Select();
        Time.timeScale = 0f;
        GameManager.instance.gameIsPaused = true;
    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        ResumeGameFlow();
    }

    public void DropOut(){
        GameManager.instance.UnregisterPlayer(this.transform.parent.GetComponent<PlayerInput>());
        ResumeGameFlow();
    }

    private void ResumeGameFlow(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.SwitchCurrentActionMap("Player");
        }
        Time.timeScale = 1f;
        GameManager.instance.gameIsPaused = false;
    }

    public void QuitToMainMenu(){
        Time.timeScale = 1f;
        GameManager.instance.ReturnToMainMenu();
    }
}
