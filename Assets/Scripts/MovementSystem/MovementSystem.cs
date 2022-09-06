using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class MovementSystem : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;

    //VARIABLES THAT WILL COME FROM CHARACTERSTATS
    public float MovSpeed {get; protected set;}
    public float SprintSpeed {get; protected set;}
    public float MaxStamina {get; protected set;}
    public float StaminaRegenRate {get; protected set;}
    public float JumpStrength {get; protected set;}
    public int ExtraJumps {get; protected set;}
    
    //VARIABLES FOR INTERNAL USE
    public bool IsSprinting {get; protected set;}
    public float CurrentStamina {get; protected set;}
    public int ExtraJumpsRemaining {get; protected set;}

    //OTHER VARIABLES
    [SerializeField] private CharacterController controller;
    [SerializeField] private Interactor interactor;
    [SerializeField] private Weapon weapon = null;
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private Vector3 move;

    //EVENTS
    public event System.Action<float> OnEntityStaminaUpdated;

    private void Awake(){
        characterStats = gameObject.transform.parent.GetComponent<CharacterStats>();
        controller = GetComponent<CharacterController>();
        interactor = GetComponent<Interactor>();
        foreach (Transform eachChild in transform) {
            if (eachChild.CompareTag("Weapon")) {
                weapon = eachChild.GetComponent<Weapon>();
            }
        }
        InitializeVariables();
    }

    void Update(){
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0){
            playerVelocity.y = 0f;
        }

        if (groundedPlayer){
            ExtraJumpsRemaining = ExtraJumps;
        }

        controller.Move(move * Time.deltaTime * MovSpeed);

        if (move != Vector3.zero){
            gameObject.transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        StaminaRegen();

    }

    private void InitializeVariables(){
        MovSpeed = characterStats.MovSpeed;
        SprintSpeed = characterStats.SprintSpeed;
        MaxStamina = characterStats.MaxStamina;
        StaminaRegenRate = characterStats.StaminaRegenRate;
        JumpStrength = characterStats.JumpStrength;
        ExtraJumps = characterStats.ExtraJumps;

        IsSprinting = false;
        CurrentStamina = MaxStamina;
        ExtraJumpsRemaining = ExtraJumps;
    }

    public void ResetStats(){
        InitializeVariables();
    }

    public void OnTriggerEnter(Collider other){
        this.transform.parent.GetComponent<CharacterEvents>().FilterCollision(gameObject, other.gameObject);
    }

    public void StaminaRegen(){
        if(CurrentStamina < MaxStamina){
            CurrentStamina += StaminaRegenRate * Time.deltaTime;
            OnEntityStaminaUpdated?.Invoke(CurrentStamina);
        }
        if (CurrentStamina > MaxStamina){
            CurrentStamina = MaxStamina;
        }
    }

    public void OnMove(InputAction.CallbackContext context){
        Vector2 movement = context.ReadValue<Vector2>();
        move = new Vector3(movement.x, 0, movement.y);
    }

    public void OnSprint(InputAction.CallbackContext context){
        //if(context.started){
        //    if(CurrentStamina > 0){
        //        IsSprinting = true;
        //    }
        //}
    }

    public void OnJump(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 15f){
                if(ExtraJumpsRemaining > 0){
                    playerVelocity.y = Mathf.Sqrt(JumpStrength * -3.0f * gravityValue);
                    if(!groundedPlayer){
                        ExtraJumpsRemaining--;
                    }
                }
                CurrentStamina -= 15f;
                OnEntityStaminaUpdated?.Invoke(CurrentStamina);
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 10f){
                controller.Move(transform.forward * MovSpeed);
                CurrentStamina -= 10f;
                OnEntityStaminaUpdated?.Invoke(CurrentStamina);
            }
        }
    }


    //MOVE THESE METHODS TO OTHER CLASSES IN THE FUTURE

    public void OnCockHammer(InputAction.CallbackContext context){
        if(weapon != null){
            weapon.OnCockHammer(context);
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(weapon != null){
            weapon.OnPressTrigger(context);
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(weapon != null){
            weapon.OnReload(context);
        }
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        if(context.performed){
            interactor.KeyIsPressed(context.ReadValue<float>());
        }
    }
}
