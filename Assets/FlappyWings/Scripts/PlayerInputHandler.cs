using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour{
    //public GameObject[] playerPrefabs;
    public PlayerController playerController;

    Vector3 startPos = new Vector3(0, 0, 0);

    private void Awake(){
        Spawn();
    }

    public void Spawn(){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = Random.Range(0, index);

        playerController = GameObject.Instantiate(GameManager.instance.playerPrefabs[GetComponent<PlayerInput>().playerIndex], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation).GetComponent<PlayerController>();
        //transform.parent = playerController.transform;
        //transform.position = playerController.transform.position;
        transform.parent = GameManager.instance.transform;
        playerController.transform.parent = this.transform;
        //Debug.Log("spawning player character");
    }

    public void Spawn(GameObject[] prefabsList, int characterIndex){
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = Random.Range(0, index);

        playerController = GameObject.Instantiate(prefabsList[characterIndex], GameManager.instance.spawnPoints[randomIndex].transform.position, transform.rotation).GetComponent<PlayerController>();
        //transform.parent = playerController.transform;
        //transform.position = playerController.transform.position;
        transform.parent = GameManager.instance.transform;
        playerController.transform.parent = this.transform;
        //Debug.Log("spawning player character");
    }

    public void Destroy(){
        GameObject.Destroy(transform.GetChild(0).gameObject);
        //Debug.Log("destroying player character");
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

    public void OnInteractWithObject(InputAction.CallbackContext context){
        playerController.OnInteractWithObject(context);
    }
}