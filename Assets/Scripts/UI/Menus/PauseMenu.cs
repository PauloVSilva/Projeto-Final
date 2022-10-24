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

    [SerializeField] private CanvasButtonDisplay[] canvasButtonDisplays; //0 back
    [SerializeField] private List<GameObject> footerButtons = new List<GameObject>();
    [SerializeField] private GameObject footer;
    [SerializeField] private GameObject buttonDisplayPrefab;

    //COMMON TO ALL MENUS
    [SerializeField] private Button firstSelected;
    [SerializeField] private TextMeshProUGUI menuName;

    //COMMON TO PLAYER-SPECIFIC MENU
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] private TextMeshProUGUI playerControllingMenu;

    //SPECIFIC TO THIS MENU
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
        CreateFooterButtons();
    }

    private void ListenToPlayerJoined(){
        GameManager.instance.OnPlayerJoinedGame += SubscribeToPlayerEvent;
    }

    private void CreateFooterButtons(){
        for(int i = 0; i < canvasButtonDisplays.Count(); i++){
            GameObject ButtonDisplay = Instantiate(buttonDisplayPrefab);
            ButtonDisplay.transform.parent = footer.transform;
            footerButtons.Add(ButtonDisplay);
        }
    }

    private void SubscribeToPlayerEvent(PlayerInput _playerInput){
        _playerInput.GetComponent<PlayerInputHandler>().OnCharacterPressMenuButton += MenuOpened;
    }

    public void MenuOpened(PlayerInput _playerInput){
        AssignPlayerToMenu(_playerInput);
        InitializeMenu();
    }

    private void AssignPlayerToMenu(PlayerInput _playerInput){
        playerInput = _playerInput;
        inputSystemUIInputModule.actionsAsset = playerInput.actions;
        //_playerInput.InputSystemUIInputModule = inputSystemUIInputModule;
        playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton += PlayerPressedBackButton;
    }

    private void InitializeMenu(){
        pauseMessage = MessageManager.instance.GetPauseMessage(playerInput.playerIndex + 1);
        playerControllingMenu.text = pauseMessage;

        for(int i = 0; i < footerButtons.Count(); i++){
            footerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = canvasButtonDisplays[i].buttonString;
            if(playerInput.devices[0].name.ToString() == "DualShock4GamepadHID"){
                footerButtons[i].GetComponentInChildren<Image>().sprite = canvasButtonDisplays[i].buttonSprite[1];
            }
            else{
                footerButtons[i].GetComponentInChildren<Image>().sprite = canvasButtonDisplays[i].buttonSprite[0];
            }
        }

        CanvasManager.instance.SwitchMenu(Menu.PauseMenu);
        firstSelected.Select();
    }

    private void PlayerPressedBackButton(InputAction.CallbackContext context){
        playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton -= PlayerPressedBackButton;
        CanvasManager.instance.CloseMenu();
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
