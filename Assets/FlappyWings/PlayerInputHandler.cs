using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour{
    public GameObject playerPrefab;
    PlayerController playerController;

    Vector3 startPos = new Vector3(0, 0, 0);

    private void Awake(){
        if(playerPrefab != null){
            playerController = GameObject.Instantiate(playerPrefab, startPos, transform.rotation).GetComponent<PlayerController>();
            transform.parent = playerController.transform;
        }
    }

    public void OnMove(InputAction.CallbackContext context){
        playerController.OnMove(context);
    }
}
