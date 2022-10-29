using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class PauseMenu : MenuController{
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

    protected override void Start(){
        base.Start();
        ListenToPlayerJoined();
    }

    private void ListenToPlayerJoined(){
        GameManager.instance.OnPlayerJoinedGame += SubscribeToPlayerButtonPress;
    }

    private void SubscribeToPlayerButtonPress(PlayerInput _playerInput){
        _playerInput.GetComponent<PlayerInputHandler>().OnCharacterPressMenuButton += MenuOpened;
    }

    public void MenuOpened(PlayerInput _playerInput){
        base.AssignPlayerToMenu(_playerInput);
        InitializeMenu();
    }

    private void InitializeMenu(){
        pauseMessage = MessageManager.instance.GetPauseMessage(playerInput.playerIndex + 1);
        playerControllingMenu.text = pauseMessage;

        CanvasManager.instance.OpenMenu(this.menu);
        StartCoroutine(FreezeGameDelay());
    }

    public IEnumerator FreezeGameDelay(){
        yield return new WaitForSecondsRealtime(0.01f);
        FreezeGame();
    }

    public IEnumerator UnfreezeGameDelay(){
        yield return new WaitForSecondsRealtime(0.01f);
        UnfreezeGame();
    }

    public void FreezeGame(){
        Time.timeScale = 0f;
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().PlayerOpenedMenu();
        }
        GameManager.instance.joinAction.Disable();
        GameManager.instance.gameIsPaused = true;
    }

    public void UnfreezeGame(){
        Time.timeScale = 1f;
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().PlayerClosedMenu();
        }
        GameManager.instance.joinAction.Enable();
        GameManager.instance.gameIsPaused = false;
    }


    #region BUTTONS
    public void Resume(){
        base.Back();
        UnfreezeGame();
    }

    public void Controls(){
        CanvasManager.instance.OpenMenu(Menu.ControlsMenu);
    }

    public void Settings(){
        CanvasManager.instance.OpenMenu(Menu.SettingsMenu);
    }

    public void DropOut(){
        GameManager.instance.UnregisterPlayer(playerInput);
        Resume();
    }

    public void QuitToMainMenu(){
        Time.timeScale = 1f;
        base.Back();
        while(GameManager.instance.playerList.Count() > 0){
            GameManager.instance.UnregisterPlayer(GameManager.instance.playerList[0]);
        }
        LevelLoader.instance.LoadLevel("MainMenu");
    }
    #endregion BUTTONS
}
