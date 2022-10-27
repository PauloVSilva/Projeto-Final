using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public abstract class MenuBase : MonoBehaviour{
    //ACTION BUTTONS ON SCREEN
    [SerializeField] protected ButtonType[] buttonTypes;
    protected List<CanvasButtonDisplay> canvasButtonsList = new List<CanvasButtonDisplay>();
    protected List<GameObject> footerButtons = new List<GameObject>();
    [SerializeField] protected GameObject footer;


    [SerializeField] protected MenuController menuController;


    protected PlayerInput playerInput;
    [SerializeField] protected TextMeshProUGUI menuName;
    [SerializeField] protected InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] protected TextMeshProUGUI playerControllingMenu;

    protected void CreateFooterButtons(){
        for(int i = 0; i < buttonTypes.Count(); i++){
            for(int j = 0; j < CanvasManager.instance.canvasButtonsList.Count(); j++){
                if(buttonTypes[i] == CanvasManager.instance.canvasButtonsList[j].buttonType){
                    canvasButtonsList.Add(CanvasManager.instance.canvasButtonsList[j]);
                    break;
                }
            }
            GameObject ButtonDisplay = Instantiate(CanvasManager.instance.buttonDisplayPrefab);
            ButtonDisplay.transform.parent = footer.transform;
            footerButtons.Add(ButtonDisplay);
        }
    }

    protected void SetUpCanvasButtons(){
        if(playerInput != null){
            //Debug.Log(playerInput.devices[0].GetType().ToString());
            for(int i = 0; i < footerButtons.Count(); i++){
                footerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = canvasButtonsList[i].buttonString;
                if(playerInput.devices[0].GetType().ToString() == "UnityEngine.InputSystem.DualShock.FastDualShock4GamepadHID"){
                    footerButtons[i].GetComponentInChildren<Image>().sprite = canvasButtonsList[i].buttonSprite[1];
                }
                else{
                    footerButtons[i].GetComponentInChildren<Image>().sprite = canvasButtonsList[i].buttonSprite[0];
                }
            }
        }
    }

    protected virtual void AssignPlayerToMenu(PlayerInput _playerInput){
        playerInput = _playerInput;
        inputSystemUIInputModule.actionsAsset = playerInput.actions;
        //_playerInput.InputSystemUIInputModule = inputSystemUIInputModule;
        playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton += PlayerPressedBackButton;
    }

    protected virtual void PlayerPressedBackButton(InputAction.CallbackContext context){
        if(CanvasManager.instance.lastActiveMenu == menuController){
            playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton -= PlayerPressedBackButton;
            CanvasManager.instance.CloseMenu();
        }
    }
}
