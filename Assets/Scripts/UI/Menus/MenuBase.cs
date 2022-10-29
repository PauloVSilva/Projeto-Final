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

    [SerializeField] protected TabGroup tabGroup;


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
            ButtonDisplay.transform.SetParent(footer.transform, false);
            footerButtons.Add(ButtonDisplay);
        }
    }

    protected void SetUpCanvasButtons(){
        if(playerInput != null){
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
        //playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton += PlayerPressedBackButton;
        playerInput.actions["Back"].performed += PlayerPressedBackButton;
        playerInput.actions["PreviousTab"].performed += PlayerPressedPreviousTabButton;
        playerInput.actions["NextTab"].performed += PlayerPressedNextTabButton;
    }

    protected virtual void PlayerPressedBackButton(InputAction.CallbackContext context){
        CanvasManager.instance.CloseMenu();
        if(CanvasManager.instance.currentMenu == null){
            //playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton -= PlayerPressedBackButton;
            playerInput.actions["Back"].performed -= PlayerPressedBackButton;
            playerInput.actions["PreviousTab"].performed -= PlayerPressedPreviousTabButton;
            playerInput.actions["NextTab"].performed -= PlayerPressedNextTabButton;
        }
    }

    protected virtual void PlayerPressedPreviousTabButton(InputAction.CallbackContext context){
        Debug.Log(CanvasManager.instance.currentMenu);
        Debug.Log(this.menuController);
        if(CanvasManager.instance.currentMenu == this.menuController){
            tabGroup?.SelectPreviousTab();
        }
    }

    protected virtual void PlayerPressedNextTabButton(InputAction.CallbackContext context){
        Debug.Log(CanvasManager.instance.currentMenu);
        Debug.Log(this.menuController);
        if(CanvasManager.instance.currentMenu == this.menuController){
            tabGroup?.SelectNextTab();
        }
    }
}
