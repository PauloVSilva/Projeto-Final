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
        GameManager.Instance.OnPlayerJoinedGame += SubscribeToPlayerButtonPress;
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

    public IEnumerator FreezeGameDelay()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        //yield return new WaitForEndOfFrame();
        FreezeGame();
    }

    public IEnumerator UnfreezeGameDelay()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        //yield return new WaitForEndOfFrame();
        UnfreezeGame();
    }

    public void FreezeGame(){
        Time.timeScale = 0f;
        foreach(var playerInput in GameManager.Instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().PlayerOpenedMenu();
        }
        GameManager.Instance.joinAction.Disable();
        GameManager.Instance.gameIsPaused = true;
    }

    public void UnfreezeGame(){
        Time.timeScale = 1f;
        foreach(var playerInput in GameManager.Instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().PlayerClosedMenu();
        }
        GameManager.Instance.joinAction.Enable();
        GameManager.Instance.gameIsPaused = false;
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
        GameManager.Instance.UnregisterPlayer(playerInput);
        Resume();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        base.Back();

        while(GameManager.Instance.playerList.Count() > 0)
        {
            GameManager.Instance.UnregisterPlayer(GameManager.Instance.playerList[0]);
        }

        LevelLoader.instance.LoadLevel("MainMenu");
    }
    #endregion BUTTONS
}
