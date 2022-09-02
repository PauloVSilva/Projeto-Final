using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour{
    private CharacterController controller;
    private Interactor interactor;
    [SerializeField] private GunSystem gunSystem = null;
    [SerializeField] private PlayerStatManager playerStatManager;

    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;

    [SerializeField] private float playerSpeed = 2.0f;
    //[SerializeField] private float sprintSpeed = 4.0f;
    //[SerializeField] private bool isSprinting = false;

    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private int extraJumps = 1;
    [SerializeField] private int extraJumpsRemaining;

    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float currentStamina = 100.0f;
    [SerializeField] private float staminaRegenRate = 5.0f;

    private float gravityValue = -9.81f;
    [SerializeField] private Vector3 move;


    private void Awake(){
        controller = GetComponent<CharacterController>();
        interactor = GetComponent<Interactor>();
        //transform.parent = GameManager.instance.transform;
        foreach (Transform eachChild in transform) {
            if (eachChild.name == "Revolver0") {
                gunSystem = this.transform.Find("Revolver0").GetComponent<GunSystem>();
            }
        }
    }

    private void Start(){
        playerStatManager = this.transform.parent.GetComponent<PlayerStatManager>();
    }

    public void OnTriggerEnter(Collider other){
        print("PlayerController detected collision");
        this.transform.parent.GetComponent<PlayerStatManager>().FilterCollision(gameObject, other.gameObject);
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
        //if(context.started){
        //    if(currentStamina > 0){
        //        isSprinting = true;
        //    }
        //}
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
        if(gunSystem != null){
            gunSystem.OnCockHammer(context);
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(gunSystem != null){
            gunSystem.OnPressTrigger(context);
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(gunSystem != null){
            gunSystem.OnReload(context);
        }
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        if(context.performed){
            interactor.KeyIsPressed(context.ReadValue<float>());
        }
    }
}