using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementSystem : MonoBehaviour{
    private CharacterManager characterManager;
    private PlayerInputHandler playerInputHandler;
    private CharacterController controller;

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
    public float MoveSpeed {get; protected set;}
    public bool IsSprinting {get; protected set;}

    public bool CanDash {get; protected set;}
    public bool IsDashing {get; protected set;}
    public float DashDuration {get; protected set;}
    public float DashTime {get; protected set;}
    public float DashCooldown {get; protected set;}

    public int JumpsRemaining {get; protected set;}
    public float AirTime {get; protected set;}
    public float AirDamage {get; protected set;}

    public float CurrentStamina {get; protected set;}
    public float StaminaRegenCooldown {get; protected set;}
    public bool CanRegenStamina {get; protected set;}

    private float angleY;
    private int quadrant;
    private float velocityX;
    private float velocityZ;

    private float velocityStopThreshold;
    private float velocityDecelerationMultiplier;

    //OTHER VARIABLES
    [SerializeField] private Vector3 playerVelocity;
    public bool groundedPlayer {get; protected set;}
    public float gravityValue {get; protected set;}
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
        SetScriptableObjectVariables();
        InitializeVariables();
        SubscribeToEvents();
    }

    private void SetScriptableObjectVariables(){
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
    }

    private void InitializeVariables(){
        gravityValue = -9.81f;

        playerVelocity.y = 0f;
        playerVelocity.x = 0f;
        playerVelocity.z = 0f;

        MoveSpeed = WalkSpeed;
        IsSprinting = false;

        CanDash = true;
        IsDashing = false;
        DashDuration = 0.1f;
        DashTime = 0f;
        DashCooldown = 0f;

        JumpsRemaining = TotalJumps;
        AirTime = 0;
        AirDamage = 0;

        CurrentStamina = MaxStamina;

        velocityStopThreshold = 1.5f;
        velocityDecelerationMultiplier = 4f;

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
            AngleCalculator();
            MovementBehaviour();
            SprintBehaviour();
            DashBehaviour();
            AirTimeDamage();
            StaminaRegen();
        }
    }

    public void ResetStats(){
        InitializeVariables();
    }

    private void OnControllerColliderHit(ControllerColliderHit other){
        if(other.transform.GetComponent<CharacterManager>() != null){
            if(AirTime > 0.5){
                other.transform.GetComponent<CharacterHealthSystem>().TakeDamage(this.gameObject, AirDamage);
            }
            if(AirTime > 1){
                characterManager.TakeDamage(AirDamage * 0.5f);
            }
            AirTime = 0;

            if(IsDashing){
                other.transform.GetComponent<MovementSystem>().Push(velocityX * WalkSpeed * velocityDecelerationMultiplier, velocityZ * WalkSpeed * velocityDecelerationMultiplier);
                StopDash();
            }
            else{
                other.transform.GetComponent<MovementSystem>().Push(velocityX * WalkSpeed, velocityZ * WalkSpeed);
            }
        }
    }

    public void Push(float velocityX, float velocityZ){
        playerVelocity.x = velocityX;
        playerVelocity.z = velocityZ;
    }

    private void AirTimeDamage(){
        if(!groundedPlayer && playerVelocity.y < 0){
            AirTime += Time.deltaTime;
            AirDamage = (playerVelocity.y * -1);
        }
        if(groundedPlayer){
            if(AirTime > 1){
                characterManager.TakeDamage(AirDamage);
            }
            AirTime = 0;
        }
    }

    public void StaminaRegen(){
        if(CanRegenStamina){
            CurrentStamina = Math.Min(CurrentStamina += StaminaRegenRate * Time.deltaTime, MaxStamina);
            SendStaminaUpdateEvent();
        }
        if(StaminaRegenCooldown > 0){
            StaminaRegenCooldown = Math.Max(StaminaRegenCooldown -= Time.deltaTime, 0);
        }
        if(StaminaRegenCooldown == 0){
            CanRegenStamina = true;
        }
    }

    private void AngleCalculator(){
        angleY = transform.rotation.eulerAngles.y % 90;
        quadrant = (int)(transform.rotation.eulerAngles.y / 90);
        if(quadrant == 0){
            velocityX = Mathf.Sin(Mathf.PI / 180 * angleY);
            velocityZ = Mathf.Cos(Mathf.PI / 180 * angleY);
        }
        if(quadrant == 1){
            velocityX = Mathf.Cos(Mathf.PI / 180 * angleY);
            velocityZ = Mathf.Sin(Mathf.PI / 180 * angleY);
        }
        if(quadrant == 2){
            velocityX = Mathf.Sin(Mathf.PI / 180 * angleY);
            velocityZ = Mathf.Cos(Mathf.PI / 180 * angleY) * -1;
        }
        if(quadrant == 3){
            velocityX = Mathf.Cos(Mathf.PI / 180 * angleY) * -1;
            velocityZ = Mathf.Sin(Mathf.PI / 180 * angleY);
        }
    }

    private void MovementBehaviour(){
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0){
            playerVelocity.y = 0f;
            JumpsRemaining = TotalJumps;
        }

        if (move != Vector3.zero){
            gameObject.transform.forward = move;
        }
        controller.Move(move * Time.deltaTime * MoveSpeed); //player input - horizontal movement

        playerVelocity.y += (AirTime * -1) + gravityValue * Time.deltaTime;

        if(playerVelocity.x != 0f){
            playerVelocity.x -= (velocityDecelerationMultiplier * playerVelocity.x) * Time.deltaTime;
            if(playerVelocity.x > 0f && playerVelocity.x < velocityStopThreshold){
                playerVelocity.x = 0f;
            }
            if(playerVelocity.x < 0f && playerVelocity.x > -velocityStopThreshold){
                playerVelocity.x = 0f;
            }
        }
        if(playerVelocity.z != 0f){
            playerVelocity.z -= (velocityDecelerationMultiplier * playerVelocity.z) * Time.deltaTime;
            if(playerVelocity.z > 0f && playerVelocity.z < velocityStopThreshold){
                playerVelocity.z = 0f;
            }
            if(playerVelocity.z < 0f && playerVelocity.z > -velocityStopThreshold){
                playerVelocity.z = 0f;
            }
        }

        controller.Move(playerVelocity * Time.deltaTime); //fake physics - vertical movement
    }

    private void SprintBehaviour(){
        if(IsSprinting && CurrentStamina > 0f){
            MoveSpeed = SprintSpeed;
            SpendStamina(SprintStaminaCost * Time.deltaTime);
        }
        else{
            MoveSpeed = WalkSpeed;
            IsSprinting = false;
        }
    }

    private void DashBehaviour(){
        if(IsDashing){
            controller.Move(transform.forward * (MoveSpeed * 5f * Time.deltaTime));
            DashTime = Math.Min(DashTime + Time.deltaTime, DashDuration);
        }
        if(DashTime == DashDuration){
            StopDash();
        }
        if(DashCooldown > 0){
            DashCooldown = Math.Max(DashCooldown -= Time.deltaTime, 0);
        }
        if(DashCooldown == 0){
            CanDash = true;
        }

    }

    private void ToggleSprint(){
        IsSprinting = !IsSprinting;
    }

    private void Jump(){
        playerVelocity.y = Mathf.Sqrt(JumpStrength * -3.0f * gravityValue);
        JumpsRemaining--;
        AirTime = 0;
        SpendStamina(JumpStaminaCost);
    }

    private void Dash(){
        IsDashing = true;
        DashCooldown = 0.25f;
        CanDash = false;
        SpendStamina(DashStaminaCost);
    }

    private void StopDash(){
        IsDashing = false;
        DashTime = 0;
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
        if(!IsDashing){
            Vector2 movement = context.ReadValue<Vector2>();
            move = new Vector3(movement.x, 0, movement.y);
        }
    }

    public void OnSprint(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 1f){
                ToggleSprint();
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 1f && JumpsRemaining > 0){
                Jump();
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        if(context.performed){
            if(CurrentStamina > 1f && CanDash){
                Dash();
            }
        }
    }

}
