using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour{
    //public GameObject[] playerPrefabs;
    public PlayerController playerController;

    private void Awake(){
        Spawn();
    }

    public void Spawn(){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);

        playerController = GameObject.Instantiate(GameManager.instance.playerPrefabs[GetComponent<PlayerInput>().playerIndex], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation).GetComponent<PlayerController>();
        //playerController = GameObject.Instantiate(GameManager.instance.playerPrefabs[0], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation).GetComponent<PlayerController>();
        transform.parent = GameManager.instance.transform;
        playerController.transform.parent = this.transform;
    }

    public void Spawn(GameObject[] prefabsList, int characterIndex){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);

        playerController = GameObject.Instantiate(prefabsList[characterIndex], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation).GetComponent<PlayerController>();
        transform.parent = GameManager.instance.transform;
        playerController.transform.parent = this.transform;
    }

    public void Destroy(){
        GameObject.Destroy(this.transform.GetChild(0).gameObject);
    }

    public void OnMove(InputAction.CallbackContext context){
        playerController.OnMove(context);
    }

    public void OnJump(InputAction.CallbackContext context){
        playerController.OnJump(context);
    }

    public void OnDash(InputAction.CallbackContext context){
        playerController.OnDash(context);
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        playerController.OnCockHammer(context);
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        playerController.OnPressTrigger(context);
    }

    public void OnSprint(InputAction.CallbackContext context){
        playerController.OnSprint(context);
    }

    public void OnReload(InputAction.CallbackContext context){
        playerController.OnReload(context);
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        playerController.OnInteractWithObject(context);
    }
}