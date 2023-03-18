using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour{

    private PlayerInput playerInput;
    private CharacterManager characterManager;
    public event System.Action<PlayerInput> OnCharacterPressMenuButton;
    public event System.Action<InputAction.CallbackContext> OnCharacterMove;
    public event System.Action<InputAction.CallbackContext> OnCharacterJump;
    public event System.Action<InputAction.CallbackContext> OnCharacterDash;
    public event System.Action<InputAction.CallbackContext> OnCharacterSprint;
    public event System.Action<InputAction.CallbackContext> OnCharacterInteractWithObject;
    public event System.Action<InputAction.CallbackContext> OnCharacterCockHammer;
    public event System.Action<InputAction.CallbackContext> OnCharacterPressTrigger;
    public event System.Action<InputAction.CallbackContext> OnCharacterReload;
    public event System.Action<InputAction.CallbackContext> OnCharacterDropItem;

    //UI ACTIONS
    public event System.Action<InputAction.CallbackContext> OnPlayerPressedBackButton;
    public event System.Action<InputAction.CallbackContext> OnPlayerPressedPreviousTabButton;
    public event System.Action<InputAction.CallbackContext> OnPlayerPressedNextTabButton;


    private void Start(){
        playerInput = GetComponent<PlayerInput>();
        characterManager = GetComponent<CharacterManager>();
    }

    public void PlayerOpenedMenu(){
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void DisableActions(){
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void PlayerClosedMenu(){
        RestoreActions();
    }

    public void RestoreActions(){
        //if(characterManager.isMountedOnTurret){
        //    playerInput.SwitchCurrentActionMap("Turret");
        //    return;
        //}
        //else{
            playerInput.SwitchCurrentActionMap("Player");
        //}
    }



    public void OnPressMenuButton(InputAction.CallbackContext context){
        if(context.performed){
            OnCharacterPressMenuButton?.Invoke(playerInput);
        }
    }

    public void OnMove(InputAction.CallbackContext context){
        if(characterManager.CanMove()){
            OnCharacterMove?.Invoke(context);
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if(characterManager.CanMove()){
            OnCharacterJump?.Invoke(context);
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        if(characterManager.CanMove()){
            OnCharacterDash?.Invoke(context);
        }
    }

    public void OnSprint(InputAction.CallbackContext context){
        if(characterManager.CanMove()){
            //OnCharacterSprint?.Invoke(context);
        }
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        OnCharacterInteractWithObject?.Invoke(context);
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        if(characterManager.CanUseFireGun()){
            //OnCharacterCockHammer?.Invoke(context);
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(characterManager.CanUseFireGun()){
            OnCharacterPressTrigger?.Invoke(context);
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(characterManager.CanUseFireGun()){
            //OnCharacterReload?.Invoke(context);
        }
    }

    public void OnDropItem(InputAction.CallbackContext context){
        if(!characterManager.IsBlocked()){
            OnCharacterDropItem?.Invoke(context);
        }
    }


    public void OnPressBackButton(InputAction.CallbackContext context){
        if(context.performed){
            OnPlayerPressedBackButton?.Invoke(context);
        }
    }
    
}