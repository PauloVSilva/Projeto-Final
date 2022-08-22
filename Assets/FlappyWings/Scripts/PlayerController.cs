using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 2.0f;
    //[SerializeField] private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private Vector3 move;


    public int score = 0;
    public enum playerColor{
        blue, red, green, yellow
    }
    public playerColor thisPlayerColor = playerColor.blue;

    public event System.Action<int> OnScoreChanged;

    private void Awake(){
        controller = GetComponent<CharacterController>();
    }

    void Update(){
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0){
            playerVelocity.y = 0f;
        }

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero){
            gameObject.transform.forward = move;
        }

        // // Changes the height position of the player..
        // if (Input.GetButtonDown("Jump") && groundedPlayer){
        //     playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        // }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void OnMove(InputAction.CallbackContext context){
        Vector2 movement = context.ReadValue<Vector2>();
        move = new Vector3(movement.x, 0, movement.y);
    }

    public void IncreaseScore(int value){
        score += value;
        if(OnScoreChanged != null){
            OnScoreChanged(score);
        }
        Debug.Log("Player score: " + score);
    }
}