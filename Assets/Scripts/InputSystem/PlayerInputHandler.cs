using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour{

    private PlayerInput playerInput;
    public event System.Action<PlayerInput> OnCharacterPressMenuButton;

    public event System.Action<InputAction.CallbackContext> OnCharacterMove;
    public event System.Action<InputAction.CallbackContext> OnCharacterJump;
    public event System.Action<InputAction.CallbackContext> OnCharacterDash;
    public event System.Action<InputAction.CallbackContext> OnCharacterThrust;
    public event System.Action<InputAction.CallbackContext> OnCharacterInteractWithObject;
    public event System.Action<InputAction.CallbackContext> OnCharacterPressTrigger;
    public event System.Action<InputAction.CallbackContext> OnCharacterDropItem;

    //UI ACTIONS
    public event System.Action<InputAction.CallbackContext> OnPlayerPressedBackButton;
    public event System.Action<InputAction.CallbackContext> OnPlayerPressedPreviousTabButton;
    public event System.Action<InputAction.CallbackContext> OnPlayerPressedNextTabButton;


    private void Awake()
    {
        InitializeComponents();
    }


    private void InitializeComponents()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void DisableActions(bool _disabled)
    {
        if (playerInput == null) return;

        if(_disabled) playerInput.SwitchCurrentActionMap("UI");
        else playerInput.SwitchCurrentActionMap("Player");
    }


    public void OnPressMenuButton(InputAction.CallbackContext context){
        if(context.performed){
            OnCharacterPressMenuButton?.Invoke(playerInput);
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

    public void OnThrust(InputAction.CallbackContext context)
    {
        OnCharacterThrust?.Invoke(context);
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        OnCharacterInteractWithObject?.Invoke(context);
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        OnCharacterPressTrigger?.Invoke(context);
    }

    public void OnDropItem(InputAction.CallbackContext context){
        OnCharacterDropItem?.Invoke(context);
    }


    public void OnPressBackButton(InputAction.CallbackContext context){
        if(context.performed){
            OnPlayerPressedBackButton?.Invoke(context);
        }
    }
    
}