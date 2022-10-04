using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementSystem : MonoBehaviour{
    private CharacterManager characterManager;
    private PlayerInputHandler playerInputHandler;
    [SerializeField] private CharacterController controller;

    //VARIABLES THAT WILL COME FROM CHARACTER SCRIPTABLE OBJECT
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
    public float airTime;

    //OTHER VARIABLES
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private Vector3 move;

    private void Awake(){
        InitializeComponents();
    }

    private void InitializeComponents(){
        playerInputHandler = GetComponent<PlayerInputHandler>();
        characterManager = GetComponent<CharacterManager>();
        controller = GetComponent<CharacterController>();
    }

    public void Initialize(){
        InitializeVariables();
        SubscribeToEvents();
    }

    private void InitializeVariables(){
        controller.radius = characterManager.Character.characterControllerRadius;
        controller.height = characterManager.Character.characterControllerHeight;
        controller.center = characterManager.Character.characterControllerCenter;
        
        WalkSpeed = characterManager.Character.walkSpeed;
        SprintSpeed = characterManager.Character.sprintSpeed;
        MaxStamina = characterManager.Character.maxStamina;
        StaminaRegenRate = characterManager.Character.staminaRegenRate;
        JumpStrength = characterManager.Character.jumpStrength;
        TotalJumps = characterManager.Character.totalJumps;
        JumpStaminaCost = characterManager.Character.jumpStaminaCost;
        DashStaminaCost = characterManager.Character.dashStaminaCost;
        SprintStaminaCost = characterManager.Character.sprintStaminaCost;

        moveSpeed = WalkSpeed;
        isSprinting = false;
        CurrentStamina = MaxStamina;
        JumpsRemaining = TotalJumps;
        SendStaminaUpdateEvent();
    }

    private void SubscribeToEvents(){
        playerInputHandler.OnCharacterMove += OnMove;
        playerInputHandler.OnCharacterSprint += OnSprint;
        playerInputHandler.OnCharacterJump += OnJump;
        playerInputHandler.OnCharacterDash += OnDash;
    }

    private void Update(){
        if(characterManager.Character != null){
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
            AirTimeDamage();
            StaminaRegen();
        }
    }

    public void ResetStats(){
        InitializeVariables();
    }

    private void AirTimeDamage(){
        if(!groundedPlayer && playerVelocity.y < 0){
            airTime += Time.deltaTime;
        }
        //if(groundedPlayer){
        //    if(airTime > 1){
        //        characterManager.TakeDamage(airTime * 10);
        //    }
        //    airTime = 0;
        //}
    }

    private void OnControllerColliderHit(ControllerColliderHit other){
        if(other.transform.GetComponent<CharacterManager>() != null){
            if(airTime > 0.5){
                other.transform.GetComponent<CharacterManager>().TakeDamage(this.gameObject, airTime * 20);
            }
            if(airTime > 1){
                characterManager.TakeDamage(airTime * 10);
            }
            airTime = 0;
        }
        if(groundedPlayer){
            if(airTime > 1){
                characterManager.TakeDamage(airTime * 10);
            }
            airTime = 0;
        }
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
        characterManager.PlayerStaminaUpdated(CurrentStamina, MaxStamina);
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
                    airTime = 0;
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
