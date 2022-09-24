using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class MovementSystem : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterEvents characterEvents;

    //VARIABLES THAT WILL COME FROM CHARACTERSTATS
    public float WalkSpeed {get; protected set;}
    public float SprintSpeed {get; protected set;}
    public float MaxStamina {get; protected set;}
    public float StaminaRegenRate {get; protected set;}
    public float JumpStrength {get; protected set;}
    public int TotalJumps {get; protected set;}
    public int JumpStaminaCost {get; protected set;}
    public int DashStaminaCost {get; protected set;}
    public int SprintStaminaCost {get; protected set;}
    
    //VARIABLES FOR INTERNAL USE
    public float moveSpeed {get; protected set;}
    public bool isSprinting {get; protected set;}
    public float CurrentStamina {get; protected set;}
    public float StaminaRegenCooldown {get; protected set;}
    public bool CanRegenStamina {get; protected set;}
    public int JumpsRemaining {get; protected set;}

    //OTHER VARIABLES
    [SerializeField] private CharacterController controller;
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private Vector3 move;

    private void Start(){
        InitializeVariables();
        SubscribeToEvents();
    }

    private void InitializeVariables(){
        characterStats = gameObject.transform.parent.GetComponent<CharacterStats>();
        characterEvents = gameObject.transform.parent.GetComponent<CharacterEvents>();
        controller = GetComponent<CharacterController>();
        
        WalkSpeed = characterStats.WalkSpeed;
        SprintSpeed = characterStats.SprintSpeed;
        MaxStamina = characterStats.MaxStamina;
        StaminaRegenRate = characterStats.StaminaRegenRate;
        JumpStrength = characterStats.JumpStrength;
        TotalJumps = characterStats.TotalJumps;
        JumpStaminaCost = characterStats.JumpStaminaCost;
        DashStaminaCost = characterStats.DashStaminaCost;
        SprintStaminaCost = characterStats.SprintStaminaCost;

        moveSpeed = WalkSpeed;
        isSprinting = false;
        CurrentStamina = MaxStamina;
        JumpsRemaining = TotalJumps;
        SendStaminaUpdateEvent();
    }

    private void SubscribeToEvents(){
        //INPUT EVENTS
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterMove += OnMove;
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterSprint += OnSprint;
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterJump += OnJump;
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterDash += OnDash;
    }

    private void Update(){
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0){
            playerVelocity.y = 0f;
            JumpsRemaining = TotalJumps;
        }

        controller.Move(move * Time.deltaTime * moveSpeed);

        if (move != Vector3.zero){
            gameObject.transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        SprintBehavior();
        StaminaRegen();

    }

    public void ResetStats(){
        InitializeVariables();
    }

    public void OnTriggerEnter(Collider other){
        characterEvents.FilterCollision(gameObject, other.gameObject);
    }

    public void StaminaRegen(){
        if(CanRegenStamina){
            CurrentStamina = Math.Min(CurrentStamina += StaminaRegenRate * Time.deltaTime, MaxStamina);
            SendStaminaUpdateEvent();
        }
        if(StaminaRegenCooldown > 0){
            StaminaRegenCooldown -= Time.deltaTime;
        }
        if(StaminaRegenCooldown <= 0){
            StaminaRegenCooldown = 0;
            CanRegenStamina = true;
        }
    }

    private void SprintBehavior(){
        if(isSprinting && CurrentStamina > 0f){
            SpendStamina(SprintStaminaCost * Time.deltaTime);
        }
        if(CurrentStamina <= 1f){
            isSprinting = false;
            moveSpeed = WalkSpeed;
        }
    }

    private void SpendStamina(float _value){
        CurrentStamina = Math.Max(CurrentStamina -= _value, 0);
        SendStaminaUpdateEvent();
        StaminaRegenCooldown = 1f;
        CanRegenStamina = false;
    }

    private void SendStaminaUpdateEvent(){
        characterEvents.PlayerStaminaUpdated(CurrentStamina, MaxStamina);
    }

    public void OnMove(InputAction.CallbackContext context){
        Vector2 movement = context.ReadValue<Vector2>();
        move = new Vector3(movement.x, 0, movement.y);
    }

    public void OnSprint(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 1f){
                isSprinting = true;
                moveSpeed = SprintSpeed;
                SendStaminaUpdateEvent();
            }
        }
        if(context.canceled){
            isSprinting = false;
            moveSpeed = WalkSpeed;
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 1f){
                if(JumpsRemaining > 0){
                    playerVelocity.y = Mathf.Sqrt(JumpStrength * -3.0f * gravityValue);
                    JumpsRemaining--;
                    SpendStamina(JumpStaminaCost);
                }
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 1f){
                controller.Move(transform.forward * moveSpeed);
                SpendStamina(DashStaminaCost);
            }
        }
    }

}
