using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour{
    public GameObject[] playerPrefabs;
    public PlayerController playerController;

    Vector3 startPos = new Vector3(0, 0, 0);

    private void Awake(){
        if(playerPrefabs != null){
            playerController = GameObject.Instantiate(playerPrefabs[GetComponent<PlayerInput>().playerIndex], GameManager.instance.spawnPoints[0].transform.position, transform.rotation).GetComponent<PlayerController>();
            transform.parent = playerController.transform;
            transform.position = playerController.transform.position;
        }
    }

    public void OnMove(InputAction.CallbackContext context){
        playerController.OnMove(context);
    }
}
