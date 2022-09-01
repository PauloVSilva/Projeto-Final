using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainHubManager : MonoBehaviour{
    //INSTANCES
    public static MainHubManager instance = null;

    public Camera mainCamera;

    public void Start(){
        GameManager.instance.SetSpawnPoint();
        foreach(var playerInput in GameManager.instance.playerList){
            //Debug.Log("teleporting");
            playerInput.GetComponent<PlayerInputHandler>().Destroy();
            playerInput.GetComponent<PlayerInputHandler>().Spawn();
            playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
        }
        GameManager.instance.joinAction.Enable();
        GameManager.instance.leaveAction.Enable();
    }

    private void OnEnable(){
        HealthSystem.OnPlayerDied += RespawnPlayer;
    }

    private void OnDisable(){
        HealthSystem.OnPlayerDied -= RespawnPlayer;
    }

    private void RespawnPlayer(GameObject gameObject){
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().RespawnPlayer(gameObject);
    }
}
