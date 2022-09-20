using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button firstSelected;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private void Start() {
        playerInputHandler.OnCharacterPressMenuButton += ChangeGameStatus;
    }

    private void OnDestroy() {
        playerInputHandler.OnCharacterPressMenuButton -= ChangeGameStatus;
    }

    private void ChangeGameStatus(){
        if(GameManager.instance.GameIsPaused){
            Resume();
        }
        else{
            Pause();
        }
    }

    public void Pause(){
        Debug.Log("Entrou no Pause");
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.SwitchCurrentActionMap("UI");
            Debug.Log("Trocou pra UI");
        }
        pauseMenuUI.SetActive(true);
        firstSelected.Select();
        Time.timeScale = 0f;
        GameManager.instance.GameIsPaused = true;
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
        GameManager.instance.GameIsPaused = false;
    }

    public void QuitToMainMenu(){
        Time.timeScale = 1f;
        GameManager.instance.ReturnToMainMenu();
    }
}
