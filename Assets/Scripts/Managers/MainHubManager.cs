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
        if(GameManager.instance.playerList.Count > 0){
            foreach(var playerInput in GameManager.instance.playerList){
                playerInput.GetComponent<PlayerInputHandler>().Destroy();
                playerInput.GetComponent<PlayerInputHandler>().Spawn();
                playerInput.GetComponent<PlayerInput>().actions["Movement"].Enable();
                playerInput.GetComponent<PlayerStatManager>().ResetScores();
            }
        }
        GameManager.instance.joinAction.Enable();
        GameManager.instance.leaveAction.Enable();
    }

    private void OnEnable(){
        HealthSystem.OnPlayerDied += PlayerKilled;
        HealthSystem.OnPlayerReborn += PlayerReborn;
    }

    private void OnDisable(){
        HealthSystem.OnPlayerDied -= PlayerKilled;
        HealthSystem.OnPlayerReborn -= PlayerReborn;
    }

    private void PlayerKilled(GameObject gameObject){
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().RespawnPlayer(gameObject);
        gameObject.transform.parent.GetComponent<PlayerInput>().actions.Disable();
    }

    private void PlayerReborn(GameObject gameObject){
        gameObject.transform.parent.GetComponent<PlayerInput>().actions.Enable();
    }
}
