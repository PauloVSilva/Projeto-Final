using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainHubManager : MonoBehaviour{
    //INSTANCES
    public static MainHubManager instance = null;

    private void Awake(){
        InitializeSingletonInstance();
    }

    private void Start(){
        InitializeMainHub();
    }

    private void InitializeSingletonInstance(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(this);
        }
    }

    private void InitializeMainHub(){
        GameManager.instance.SetSpawnPoint();
        GameManager.instance.joinAction.Enable();
        if(GameManager.instance.playerList.Count > 0){
            foreach(var playerInput in GameManager.instance.playerList){
                playerInput.GetComponent<CharacterEvents>().FullReset();
            }
        }
    }
}
