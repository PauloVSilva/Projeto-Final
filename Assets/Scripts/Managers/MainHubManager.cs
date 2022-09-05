using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainHubManager : MonoBehaviour{
    //INSTANCES
    public static MainHubManager instance = null;

    public Camera mainCamera;

    public void Start(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        GameManager.instance.SetSpawnPoint();
        if(GameManager.instance.playerList.Count > 0){
            foreach(var playerInput in GameManager.instance.playerList){
                playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
                playerInput.GetComponent<PlayerInput>().actions.Enable();
                playerInput.GetComponent<PlayerStatManager>().ResetScores();
                mainCamera.GetComponent<CameraController>().AddPlayer(playerInput);
            }
        }
        GameManager.instance.joinAction.Enable();
        GameManager.instance.leaveAction.Enable();
    }
}
