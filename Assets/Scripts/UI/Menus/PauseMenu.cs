using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class PauseMenu : MenuController
{
    protected override void Start()
    {
        base.Start();

        ListenToPlayerJoined();
    }

    private void ListenToPlayerJoined()
    {
        GameManager.Instance.OnPlayerJoinedGame += SubscribeToPlayerButtonPress;
    }

    private void SubscribeToPlayerButtonPress(PlayerInput _playerInput)
    {
        _playerInput.GetComponent<PlayerInputHandler>().OnCharacterPressMenuButton += ButtonPressed;
    }

    private void ButtonPressed(PlayerInput _playerInput)
    {
        CanvasManager.Instance.OpenMenu(menu, _playerInput);

        InitializeMenuVariables();

        GameManager.Instance.UpdateGameState(GameState.Paused);
    }

    private void InitializeMenuVariables()
    {
        string pauseMessage = MessageManager.instance.GetPauseMessage(playerInput.playerIndex + 1);
        playerControllingMenu.text = pauseMessage;
    }


    #region BUTTONS
    public void Resume()
    {
        Back();
    }

    public void Controls()
    {
        CanvasManager.Instance.OpenMenu(Menu.ControlsMenu);
    }

    public void Settings()
    {
        CanvasManager.Instance.OpenMenu(Menu.SettingsMenu);
    }

    public void DropOut()
    {
        playerInput.GetComponent<PlayerInputHandler>().OnCharacterPressMenuButton -= ButtonPressed;
        GameManager.Instance.UnregisterPlayer(playerInput);
        Resume();
    }

    public void QuitToMainMenu()
    {
        Back();

        while(GameManager.Instance.playerList.Count() > 0)
        {
            GameManager.Instance.UnregisterPlayer(GameManager.Instance.playerList[0]);
        }

        LevelLoader.Instance.LoadLevel("MainMenu");
    }
    #endregion BUTTONS
}
