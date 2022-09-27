using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour{

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private CharacterStats characterStats;
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

    private void Start(){
        playerInput = GetComponent<PlayerInput>();
        characterEvents = GetComponent<CharacterEvents>();
        characterStats = GetComponent<CharacterStats>();
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
        //if(characterStats.isMountedOnTurret){
        //    playerInput.SwitchCurrentActionMap("Turret");
        //    return;
        //}
        //else{
            playerInput.SwitchCurrentActionMap("Player");
        //}
    }



    public void OnPressMenuButton(InputAction.CallbackContext context){
        if(context.performed){
            OnCharacterPressMenuButton?.Invoke(GetComponent<PlayerInput>());
        }
    }

    public void OnMove(InputAction.CallbackContext context){
        if(characterStats.CanMove()){
            OnCharacterMove?.Invoke(context);
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if(characterStats.CanMove()){
            OnCharacterJump?.Invoke(context);
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        if(characterStats.CanMove()){
            OnCharacterDash?.Invoke(context);
        }
    }

    public void OnSprint(InputAction.CallbackContext context){
        if(characterStats.CanMove()){
            OnCharacterSprint?.Invoke(context);
        }
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        OnCharacterInteractWithObject?.Invoke(context);
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        if(characterStats.CanUseFireGun()){
            OnCharacterCockHammer?.Invoke(context);
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(characterStats.CanUseFireGun()){
            OnCharacterPressTrigger?.Invoke(context);
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(characterStats.CanUseFireGun()){
            OnCharacterReload?.Invoke(context);
        }
    }

    public void OnDropItem(InputAction.CallbackContext context){
        if(!characterStats.IsBlocked()){
            OnCharacterDropItem?.Invoke(context);
        }
    }
}