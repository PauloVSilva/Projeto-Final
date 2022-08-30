using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour{
    private CharacterController controller;
    private Interactor interactor;
    [SerializeField] private GunSystem gunSystem;

    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;

    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 4.0f;
    [SerializeField] private bool isSprinting = false;

    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private int extraJumps = 1;
    [SerializeField] private int extraJumpsRemaining;

    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float currentStamina = 100.0f;
    [SerializeField] private float staminaRegenRate = 5.0f;

    private float gravityValue = -9.81f;
    [SerializeField] private Vector3 move;

    public int score = 0;
    public enum playerColor{blue, red, green, yellow}
    public playerColor thisPlayerColor = playerColor.blue;

    public event System.Action<int> OnScoreChanged;


    private void Awake(){
        controller = GetComponent<CharacterController>();
        interactor = GetComponent<Interactor>();
        //transform.parent = GameManager.instance.transform;
        gunSystem = this.transform.Find("Revolver0").GetComponent<GunSystem>();
    }

    void Update(){
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0){
            playerVelocity.y = 0f;
        }

        if (groundedPlayer){
            extraJumpsRemaining = extraJumps;
        }

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero){
            gameObject.transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //LerpRotation();
        StaminaRegen();
    }

    public void LerpRotation(){
        //transform.rotation = Quaternion.Euler(0, 180, 0);
        if(transform.rotation.y > 0 && transform.rotation.y < 180){
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, 90f, transform.rotation.z), 5 * Time.deltaTime);
        }
        else {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, -90f, transform.rotation.z), 5 * Time.deltaTime);
        }
    }

    public void StaminaRegen(){
        if(currentStamina < maxStamina){
            currentStamina = currentStamina + staminaRegenRate * Time.deltaTime;
        }
        if (currentStamina > maxStamina){
            currentStamina = maxStamina;
        }
    }

    public void OnMove(InputAction.CallbackContext context){
        Vector2 movement = context.ReadValue<Vector2>();
        move = new Vector3(movement.x, 0, movement.y);
    }

    public void OnSprint(InputAction.CallbackContext context){
        if(context.started){
            if(currentStamina > 0){
                isSprinting = true;
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if(context.performed){
            if(currentStamina > 15f){
                if(extraJumpsRemaining > 0){
                    playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                    if(!groundedPlayer){
                        extraJumpsRemaining--;
                    }
                }
                currentStamina -= 15f;
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        if(context.performed){
            if(currentStamina > 10f){
                controller.Move(transform.forward * playerSpeed);
                currentStamina -= 10f;
            }
        }
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        gunSystem.OnCockHammer(context);
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        gunSystem.OnPressTrigger(context);
    }

    public void OnReload(InputAction.CallbackContext context){
        gunSystem.OnReload(context);
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        if(context.performed){
            interactor.KeyIsPressed(context.ReadValue<float>());
        }
    }

    public void IncreaseScore(int value){
        score += value;
        if(OnScoreChanged != null){
            OnScoreChanged(score);
        }
        //Debug.Log("Player score: " + score);
    }
}