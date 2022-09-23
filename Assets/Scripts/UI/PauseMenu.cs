using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class PauseMenu : MonoBehaviour{
    //COMMON TO ALL MENUS
    [SerializeField] private Button firstSelected;
    [SerializeField] private TextMeshProUGUI menuName;

    //COMMON TO PLAYER-SPECIFIC MENU
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] private TextMeshProUGUI playerControllingMenu;

    //SPECIFIC TO THIS MENU
    [SerializeField] private Button DropOutButton;

    private void Start(){
        ListenToPlayerJoined();
    }

    private void ListenToPlayerJoined(){
        GameManager.instance.OnPlayerJoinedGame += SubscribeToPlayerEvent;
    }

    private void SubscribeToPlayerEvent(PlayerInput _playerInput){
        _playerInput.GetComponent<PlayerInputHandler>().OnCharacterPressMenuButton += PlayerPressedPause;
    }

    private void PlayerPressedPause(PlayerInput _playerInput){
        firstSelected.Select();
        playerInput = _playerInput;
        inputSystemUIInputModule.actionsAsset = playerInput.actions;
        //_playerInput.InputSystemUIInputModule = inputSystemUIInputModule;
        playerControllingMenu.text = "Paused by player " + (_playerInput.playerIndex + 1);
        Pause();
        CanvasManager.instance.SwitchMenu(Menu.PauseMenu);
    }

    public void Pause(){
        Time.timeScale = 0f;
        GameManager.instance.gameIsPaused = true;
    }

    public void Resume(){
        Time.timeScale = 1f;
        GameManager.instance.gameIsPaused = false;
    }

    public void DropOut(){
        GameManager.instance.UnregisterPlayer(playerInput);
        Resume();
    }

    public void QuitToMainMenu(){
        Time.timeScale = 1f;
        GameManager.instance.ReturnToMainMenu();
    }
}
