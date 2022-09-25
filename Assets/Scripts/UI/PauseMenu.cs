using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class PauseMenu : MonoBehaviour{
    public static PauseMenu instance = null;
    //COMMON TO ALL MENUS
    [SerializeField] private Button firstSelected;
    [SerializeField] private TextMeshProUGUI menuName;

    //COMMON TO PLAYER-SPECIFIC MENU
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] private TextMeshProUGUI playerControllingMenu;

    //SPECIFIC TO THIS MENU
    [SerializeField] private Button DropOutButton;
    [SerializeField] private string pauseMessage;

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
        pauseMessage = MessageManager.instance.GetPauseMessage(_playerInput.playerIndex + 1);
        playerControllingMenu.text = pauseMessage;
        //Pause();
        CanvasManager.instance.SwitchMenu(Menu.PauseMenu);
    }

    public IEnumerator PauseDelay(){
        yield return new WaitForSeconds(0.01f);
        Pause();
    }

    public void Pause(){
        Time.timeScale = 0f;
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().PlayerOpenedMenu();
        }
        GameManager.instance.joinAction.Disable();
        GameManager.instance.gameIsPaused = true;
        DropOutButton.interactable = !GameManager.instance.miniGameIsRunning;
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
        GameManager.instance.ReturnToMainMenu();
    }
}
