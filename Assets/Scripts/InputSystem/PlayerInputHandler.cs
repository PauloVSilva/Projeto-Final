using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour{

    [SerializeField] private CharacterEvents characterEvents;
    public event System.Action OnCharacterPressMenuButton;
    public event System.Action<InputAction.CallbackContext> OnCharacterMove;
    public event System.Action<InputAction.CallbackContext> OnCharacterJump;
    public event System.Action<InputAction.CallbackContext> OnCharacterDash;
    public event System.Action<InputAction.CallbackContext> OnCharacterSprint;
    public event System.Action<InputAction.CallbackContext> OnCharacterInteractWithObject;
    public event System.Action<InputAction.CallbackContext> OnCharacterCockHammer;
    public event System.Action<InputAction.CallbackContext> OnCharacterPressTrigger;
    public event System.Action<InputAction.CallbackContext> OnCharacterReload;
    public event System.Action<InputAction.CallbackContext> OnCharacterDropWeapon;

    private void Start(){
        characterEvents = GetComponent<CharacterEvents>();
        characterEvents.OnPlayerPickedUpWeapon += SetGunActionsActive;
        characterEvents.OnPlayerDroppedWeapon += SetGunActionsInactive;
    }

    private void OnDestroy(){
        characterEvents.OnPlayerPickedUpWeapon -= SetGunActionsActive;
        characterEvents.OnPlayerDroppedWeapon -= SetGunActionsInactive;
    }

    public void EnableMovementActions(){
        GetComponent<PlayerInput>().actions["Movement"].Enable();
        GetComponent<PlayerInput>().actions["Sprint"].Enable();
        GetComponent<PlayerInput>().actions["Jump"].Enable();
        GetComponent<PlayerInput>().actions["Dash"].Enable();
    }

    public void DisableMovementActions(){
        GetComponent<PlayerInput>().actions["Movement"].Disable();
        GetComponent<PlayerInput>().actions["Sprint"].Disable();
        GetComponent<PlayerInput>().actions["Jump"].Disable();
        GetComponent<PlayerInput>().actions["Dash"].Disable();
    }

    public void SetGunActionsActive(Weapon _weapon){
        GetComponent<PlayerInput>().actions["CockHammer"].Enable();
        GetComponent<PlayerInput>().actions["PressTrigger"].Enable();
        GetComponent<PlayerInput>().actions["ReloadWeapon"].Enable();
        GetComponent<PlayerInput>().actions["DropWeapon"].Enable();
    }

    public void SetGunActionsInactive(){
        GetComponent<PlayerInput>().actions["CockHammer"].Disable();
        GetComponent<PlayerInput>().actions["PressTrigger"].Disable();
        GetComponent<PlayerInput>().actions["ReloadWeapon"].Disable();
        GetComponent<PlayerInput>().actions["DropWeapon"].Disable();
    }



    public void OnPressMenuButton(InputAction.CallbackContext context){
        if(context.performed){
            OnCharacterPressMenuButton?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context){
        OnCharacterMove?.Invoke(context);
    }

    public void OnJump(InputAction.CallbackContext context){
        OnCharacterJump?.Invoke(context);
    }

    public void OnDash(InputAction.CallbackContext context){
        OnCharacterDash?.Invoke(context);
    }

    public void OnSprint(InputAction.CallbackContext context){
        OnCharacterSprint?.Invoke(context);
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        OnCharacterInteractWithObject?.Invoke(context);
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        OnCharacterCockHammer?.Invoke(context);
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        OnCharacterPressTrigger?.Invoke(context);
    }

    public void OnReload(InputAction.CallbackContext context){
        OnCharacterReload?.Invoke(context);
    }

    public void OnDropWeapon(InputAction.CallbackContext context){
        OnCharacterDropWeapon?.Invoke(context);
    }
}