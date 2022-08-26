using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour{
    public GameObject[] playerPrefabs;
    public PlayerController playerController;

    Vector3 startPos = new Vector3(0, 0, 0);

    private void Awake(){
        Spawn();
    }

    public void Spawn(){
        if(playerPrefabs != null){

            int index = GameManager.instance.spawnPoints.Length;
            int randomIndex = Random.Range(0, index);

            playerController = GameObject.Instantiate(playerPrefabs[GetComponent<PlayerInput>().playerIndex], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation).GetComponent<PlayerController>();
            transform.parent = playerController.transform;
            transform.position = playerController.transform.position;
        }

    }

    public void OnMove(InputAction.CallbackContext context){
        if(playerPrefabs != null){
            playerController.OnMove(context);
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if(playerPrefabs != null){
            playerController.OnJump(context);
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        if(playerPrefabs != null){
            playerController.OnDash(context);
        }
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        if(playerPrefabs != null){
            playerController.OnInteractWithObject(context);
        }
    }
}