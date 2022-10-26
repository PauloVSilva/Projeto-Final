using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainHubManager : LevelManager{
    public static MainHubManager instance = null;

    protected override void InitializeSingletonInstance(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    protected override void InitializeLevel(){
        GameManager.instance.joinAction.Enable();
        GameManager.instance.miniGameIsRunning = false;

        CanvasManager.instance.playerPanels.SetActive(true);
        CanvasManager.instance.miniGameUI.SetActive(true);
    }
}
