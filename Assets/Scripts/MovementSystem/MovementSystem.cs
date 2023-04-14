using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementSystem : MonoBehaviour{
    private CharacterManager characterManager;
    private CharacterHealthSystem characterHealthSystem;
    private PlayerInputHandler playerInputHandler;
    private CharacterController controller;

    //VARIABLES THAT WILL COME FROM CHARACTER SCRIPTABLE OBJECT
    public float WalkSpeed {get; protected set;}
    public float JumpStrength {get; protected set;}
    public int TotalJumps {get; protected set;}
    
    //VARIABLES FOR INTERNAL USE
    public float MoveSpeed {get; protected set;}

    public bool CanDash {get; protected set;}
    public bool IsDashing {get; protected set;}
    public float DashDuration {get; protected set;}
    public float DashTime {get; protected set;}
    public float DashCooldown {get; protected set;}

    public int JumpsRemaining {get; protected set;}
    public float AirTime {get; protected set;}
    public float AirDamage {get; protected set;}

    [SerializeField] private float angleY;
    [SerializeField] private int quadrant;
    [SerializeField] private float velocityX;
    [SerializeField] private float velocityZ;

    [SerializeField] private float velocityStopThreshold;
    [SerializeField] private float velocityDecelerationMultiplier;

    //OTHER VARIABLES
    [SerializeField] private Vector3 playerVelocity;
    public bool GroundedPlayer {get; protected set;}
    public float GravityValue {get; protected set;}
    [SerializeField] private Vector3 move;

    private void Awake(){
        InitializeComponents();
        SubscribeToEvents();
    }

    private void FixedUpdate()
    {
        if (characterManager.Character == null) return;

        AngleCalculator();
        FakePhysics();
        MovementBehaviour();
        DashBehaviour();
        AirTimeDamage();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.transform.GetComponent<CharacterManager>() == null)
        {
            //Debug.Log("CharacterManager not found on character");
            return;
        }

        other.transform.TryGetComponent(out CharacterManager otherCharacter);

        if (AirTime > 0.25)
        {
            otherCharacter.characterHealthSystem.TakeDamage(this.gameObject, 10 + AirDamage * 2);
        }

        if (AirTime > 1.5)
        {
            characterManager.characterHealthSystem.TakeDamage(AirDamage * 0.5f);
        }

        AirTime = 0;

        if (IsDashing)
        {
            otherCharacter.characterMovementSystem.Push(velocityX * WalkSpeed * velocityDecelerationMultiplier, velocityZ * WalkSpeed * velocityDecelerationMultiplier);
            otherCharacter.characterHealthSystem.TakeDamage(this.gameObject, 0);

            StopDash();
        }

        else if (move != Vector3.zero)
        {
            otherCharacter.characterMovementSystem.Push(velocityX * WalkSpeed, velocityZ * WalkSpeed);
        }
    }


    private void InitializeComponents(){
        playerInputHandler = GetComponent<PlayerInputHandler>();
        characterManager = GetComponent<CharacterManager>();
        characterHealthSystem = GetComponent<CharacterHealthSystem>();
        controller = GetComponent<CharacterController>();
    }


    private void SubscribeToEvents()
    {
        playerInputHandler.OnCharacterMove += OnMove;
        playerInputHandler.OnCharacterJump += OnJump;
        playerInputHandler.OnCharacterDash += OnDash;
    }

    private void UnsubscribeFromEvents()
    {
        playerInputHandler.OnCharacterMove -= OnMove;
        playerInputHandler.OnCharacterJump -= OnJump;
        playerInputHandler.OnCharacterDash -= OnDash;
    }


    public void Initialize()
    {
        GetScriptableObjectVariables();
        InitializeVariables();
    }

    private void GetScriptableObjectVariables()
    {
        controller.radius = characterManager.Character.characterControllerRadius;
        controller.height = characterManager.Character.characterControllerHeight;
        controller.center = characterManager.Character.characterControllerCenter;
        
        WalkSpeed = characterManager.Character.walkSpeed;
        JumpStrength = characterManager.Character.jumpStrength;
        TotalJumps = characterManager.Character.totalJumps;
    }

    private void InitializeVariables()
    {
        GravityValue = -9.81f;

        playerVelocity = new Vector3(0, 0, 0);
        move = new Vector3(0, 0, 0);

        MoveSpeed = WalkSpeed;

        CanDash = true;
        IsDashing = false;
        DashDuration = 0.1f;
        DashTime = 0f;
        DashCooldown = 0f;

        JumpsRemaining = TotalJumps;
        AirTime = 0;
        AirDamage = 0;

        velocityStopThreshold = 1.5f;
        velocityDecelerationMultiplier = 4f;
    }

    public void Push(float velocityX, float velocityZ)
    {
        playerVelocity.x = velocityX;
        playerVelocity.z = velocityZ;
    }

    private void AirTimeDamage()
    {
        if(!GroundedPlayer && playerVelocity.y < 0)
        {
            AirTime += Time.deltaTime;
            AirDamage = (playerVelocity.y * -1);
        }
        if(GroundedPlayer)
        {
            if(AirTime > 1)
            {
                characterManager.characterHealthSystem.TakeDamage(AirDamage);
            }
            AirTime = 0;
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
            velocityZ = Mathf.Sin(Mathf.PI / 180 * angleY) * -1;
        }
        if(quadrant == 2){
            velocityX = Mathf.Sin(Mathf.PI / 180 * angleY) * -1;
            velocityZ = Mathf.Cos(Mathf.PI / 180 * angleY) * -1;
        }
        if(quadrant == 3){
            velocityX = Mathf.Cos(Mathf.PI / 180 * angleY) * -1;
            velocityZ = Mathf.Sin(Mathf.PI / 180 * angleY);
        }
    }

    private void MovementBehaviour()
    {
        GroundedPlayer = controller.isGrounded;

        if (GroundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            JumpsRemaining = TotalJumps;
        }

        if (move == Vector3.zero) return;

        //float x = Mathf.LerpAngle(gameObject.transform.forward.x, move.x, Time.deltaTime * 25f);
        //float y = Mathf.LerpAngle(gameObject.transform.forward.y, move.y, Time.deltaTime * 25f);
        //float z = Mathf.LerpAngle(gameObject.transform.forward.z, move.z, Time.deltaTime * 25f);
        //
        //Debug.Log("transform " + gameObject.transform.forward);
        //Debug.Log("move " + move);
        //
        //gameObject.transform.forward = new Vector3(x, y, z);

        gameObject.transform.forward = move;

        controller.Move(MoveSpeed * Time.deltaTime * move); //player input - horizontal movement
    }

    private void FakePhysics()
    {
        playerVelocity.y += /*(AirTime * -1f) + */ 3 * GravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime); //fake physics - vertical movement

        if (playerVelocity.x != 0f)
        {
            playerVelocity.x -= (velocityDecelerationMultiplier * playerVelocity.x) * Time.deltaTime;

            if (playerVelocity.x > 0f && playerVelocity.x < velocityStopThreshold)
            {
                playerVelocity.x = 0f;
            }
            if (playerVelocity.x < 0f && playerVelocity.x > -velocityStopThreshold)
            {
                playerVelocity.x = 0f;
            }
        }

        if (playerVelocity.z != 0f)
        {
            playerVelocity.z -= (velocityDecelerationMultiplier * playerVelocity.z) * Time.deltaTime;

            if (playerVelocity.z > 0f && playerVelocity.z < velocityStopThreshold)
            {
                playerVelocity.z = 0f;
            }
            if (playerVelocity.z < 0f && playerVelocity.z > -velocityStopThreshold)
            {
                playerVelocity.z = 0f;
            }
        }
    }

    private void DashBehaviour(){
        if(IsDashing)
        {
            controller.Move(transform.forward * (MoveSpeed * 3f * Time.deltaTime));
            DashTime = Math.Min(DashTime + Time.deltaTime, DashDuration);
        }
        if(DashTime == DashDuration)
        {
            StopDash();
        }
        if(DashCooldown > 0)
        {
            DashCooldown = Math.Max(DashCooldown -= Time.deltaTime, 0);
        }
        if(DashCooldown == 0)
        {
            CanDash = true;
        }

    }

    private void Jump()
    {
        playerVelocity.y = Mathf.Sqrt(JumpStrength * -3.0f * GravityValue);
        JumpsRemaining--;
        AirTime = 0;
    }

    private void Dash()
    {
        IsDashing = true;
        DashCooldown = 0.5f;
        CanDash = false;
    }

    private void StopDash()
    {
        IsDashing = false;
        DashTime = 0;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!characterManager.CanMove()) return;
        if (IsDashing) return;

        Vector2 input = context.ReadValue<Vector2>();
        Vector3 movement = new(input.x, 0, input.y);

        move = movement;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!characterManager.CanMove()) return;
        if (JumpsRemaining < 1) return;

        if (context.performed)
        {
            Jump();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!characterManager.CanMove()) return;
        if (!CanDash) return;

        if (context.performed)
        {
            Dash();
        }
    }

}
