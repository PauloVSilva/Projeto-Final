using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainHubManager : MonoBehaviour{
    //INSTANCES
    public static MainHubManager instance = null;

    public void Awake(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(gameObject);
        }
    }

    public void Start(){
        GameManager.instance.miniGameIsRunning = false;

        GameManager.instance.SetSpawnPoint();
        if(GameManager.instance.playerList.Count > 0){
            foreach(var playerInput in GameManager.instance.playerList){
                playerInput.transform.GetComponent<CharacterEvents>().RespawnCharacter();
                playerInput.actions.Enable();
            }
        }
        GameManager.instance.joinAction.Enable();
        GameManager.instance.leaveAction.Enable();
    }
}
