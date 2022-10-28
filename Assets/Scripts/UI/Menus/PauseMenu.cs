using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class PauseMenu : MenuBase{
    public static PauseMenu instance = null;
    private string pauseMessage;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    private void Start(){
        ListenToPlayerJoined();
        base.CreateFooterButtons();
    }

    private void ListenToPlayerJoined(){
        GameManager.instance.OnPlayerJoinedGame += SubscribeToPlayerEvent;
    }

    private void SubscribeToPlayerEvent(PlayerInput _playerInput){
        _playerInput.GetComponent<PlayerInputHandler>().OnCharacterPressMenuButton += MenuOpened;
    }

    public void MenuOpened(PlayerInput _playerInput){
        base.AssignPlayerToMenu(_playerInput);
        InitializeMenu();
    }

    private void InitializeMenu(){
        pauseMessage = MessageManager.instance.GetPauseMessage(playerInput.playerIndex + 1);
        playerControllingMenu.text = pauseMessage;

        base.SetUpCanvasButtons();
        CanvasManager.instance.OpenMenu(Menu.PauseMenu);
        StartCoroutine(PauseDelay());
    }

    public IEnumerator PauseDelay(){
        yield return new WaitForSeconds(0.01f);
        Pause();
    }

    public IEnumerator ResumeDelay(){
        yield return new WaitForSeconds(0.01f);
        Resume();
    }

    public void Pause(){
        Time.timeScale = 0f;
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().PlayerOpenedMenu();
        }
        GameManager.instance.joinAction.Disable();
        GameManager.instance.gameIsPaused = true;
    }

    public void Resume(){
        Time.timeScale = 1f;
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().PlayerClosedMenu();
        }
        GameManager.instance.joinAction.Enable();
        GameManager.instance.gameIsPaused = false;
    }

    public void DropOut(){
        GameManager.instance.UnregisterPlayer(playerInput);
        Resume();
    }

    public void QuitToMainMenu(){
        Time.timeScale = 1f;
        CanvasManager.instance.CloseMenu();
        while(GameManager.instance.playerList.Count() > 0){
            GameManager.instance.UnregisterPlayer(GameManager.instance.playerList[0]);
        }
        LevelLoader.instance.LoadLevel("MainMenu");
    }
}
