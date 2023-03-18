using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : PersistentSingleton<GameManager>
{
    public bool gameIsPaused;
    public bool miniGameIsRunning;

    public ItemsDatabank itemsDatabank;

    public Camera mainCamera;
    
    public List<PlayerInput> playerList = new List<PlayerInput>();
    public GameObject[] spawnPoints;

    public InputAction joinAction;

    //EVENTS
    public event System.Action<PlayerInput> OnPlayerJoinedGame;
    public event System.Action<PlayerInput> OnPlayerLeftGame;

    protected override void Awake(){
        base.Awake();

        gameIsPaused = false;
        miniGameIsRunning = false;
        
        joinAction.performed += context => {JoinAction(context); Debug.Log("Player Joined");};
    }


    private void OnPlayerJoined(PlayerInput playerInput)
    {
        //This callback is called by PlayerInputManager whenever JoinAction method is performed
        //So even though is might show up as "0 references" by intellisense, it is called by Unity automatically

        playerList.Add(playerInput);

        OnPlayerJoinedGame?.Invoke(playerInput);

        joinAction.Enable();
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    { 
        //THIS METHOD COMES FROM UNITY ITSELF
    }

    public void JoinAction(InputAction.CallbackContext context){
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    public void UnregisterPlayer(PlayerInput playerInput){
        playerList.Remove(playerInput);

        OnPlayerLeftGame?.Invoke(playerInput);

        Destroy(playerInput.transform.gameObject);
    }

    /*public void LeaveAction(InputAction.CallbackContext context){
        if (playerList.Count > 0){
            foreach(var player in playerList){
                foreach (var device in player.devices){
                    if (device != null && context.control.device == device){
                        UnregisterPlayer(player);
                        return;
                    }
                }
            }
        }
    }*/
}
