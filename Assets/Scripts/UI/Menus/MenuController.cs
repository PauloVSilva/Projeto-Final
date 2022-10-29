using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class MenuController : MonoBehaviour{
    public Menu menu;
    [SerializeField] private GameObject menuContainer;
    [SerializeField] private Button firstSelected;


    private List<CanvasButtonDisplay> canvasButtonsList = new List<CanvasButtonDisplay>();
    private List<GameObject> footerButtons = new List<GameObject>();
    protected PlayerInput playerInput;

    [SerializeField] protected ButtonType[] buttonTypes;
    [SerializeField] protected TextMeshProUGUI menuName;
    [SerializeField] protected TextMeshProUGUI playerControllingMenu;
    [SerializeField] protected TabGroup tabGroup;
    [SerializeField] protected GameObject footer;
    [SerializeField] protected InputSystemUIInputModule inputSystemUIInputModule;

    protected virtual void Start(){
        CreateFooterButtons();
    }


    public void Open(){
        menuContainer.SetActive(true);
        GainControl();
        SetUpCanvasButtons();
        SubscribeToInputActions();
    }

    public void Close(){
        menuContainer.SetActive(false);
        UnsubscribeFromInputActions();
    }

    public void GainControl(){
        firstSelected.Select();
    }
    
    public void Back(){
        CanvasManager.instance.CloseMenu();
    }

    private void CreateFooterButtons(){
        if(buttonTypes.Count() < 1)
            return;

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

    private void SetUpCanvasButtons(){
        if(playerInput == null)
            return;

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

    private void SubscribeToInputActions(){
        if(playerInput == null)
            return;

        //InputActionAsset playerActions = playerInput.actions;

        for(int i = 0; i < buttonTypes.Count(); i++){
            if(buttonTypes[i].ToString() == "Back")
                playerInput.actions["Back"].performed += PlayerPressedBackButton;

            if(tabGroup != null){
                playerInput.actions["PreviousTab"].performed += PlayerPressedPreviousTabButton;
                playerInput.actions["NextTab"].performed += PlayerPressedNextTabButton;
            }
        }
    }

    private void UnsubscribeFromInputActions(){
        if(playerInput == null)
            return;
            
        playerInput.actions["Back"].performed -= PlayerPressedBackButton;
        playerInput.actions["PreviousTab"].performed -= PlayerPressedPreviousTabButton;
        playerInput.actions["NextTab"].performed -= PlayerPressedNextTabButton;
    }

    protected virtual void AssignPlayerToMenu(PlayerInput _playerInput){
        playerInput = _playerInput;
        inputSystemUIInputModule.actionsAsset = playerInput.actions;
        //_playerInput.InputSystemUIInputModule = inputSystemUIInputModule;
        //playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton += PlayerPressedBackButton;
    }

    protected virtual void PlayerPressedBackButton(InputAction.CallbackContext context){
        //CanvasManager.instance.CloseMenu();
        //if(CanvasManager.instance.currentMenu == null){
        //    //playerInput.GetComponent<PlayerInputHandler>().OnPlayerPressedBackButton -= PlayerPressedBackButton;
        //    playerInput.actions["Back"].performed -= PlayerPressedBackButton;
        //    playerInput.actions["PreviousTab"].performed -= PlayerPressedPreviousTabButton;
        //    playerInput.actions["NextTab"].performed -= PlayerPressedNextTabButton;
        //}
        if(CanvasManager.instance.currentMenu == this)
            CanvasManager.instance.CloseMenu();
    }

    protected virtual void PlayerPressedPreviousTabButton(InputAction.CallbackContext context){
        if(CanvasManager.instance.currentMenu == this)
            tabGroup?.SelectPreviousTab();
    }

    protected virtual void PlayerPressedNextTabButton(InputAction.CallbackContext context){
        if(CanvasManager.instance.currentMenu == this)
            tabGroup?.SelectNextTab();
    }
}
