using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour{
    [SerializeField] private MovementSystem movementSystem;

    private void Awake(){
        Spawn();
    }

    public void Spawn(){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);

        movementSystem = GameObject.Instantiate(GameManager.instance.playerPrefabs[0], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation).GetComponent<MovementSystem>();
        transform.parent = GameManager.instance.transform;
        movementSystem.transform.parent = this.transform;
    }

    public void OnMove(InputAction.CallbackContext context){
        movementSystem.OnMove(context);
    }

    public void OnJump(InputAction.CallbackContext context){
        movementSystem.OnJump(context);
    }

    public void OnDash(InputAction.CallbackContext context){
        movementSystem.OnDash(context);
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        movementSystem.OnCockHammer(context);
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        movementSystem.OnPressTrigger(context);
    }

    public void OnSprint(InputAction.CallbackContext context){
        movementSystem.OnSprint(context);
    }

    public void OnReload(InputAction.CallbackContext context){
        movementSystem.OnReload(context);
    }

    public void OnInteractWithObject(InputAction.CallbackContext context){
        movementSystem.OnInteractWithObject(context);
    }
}