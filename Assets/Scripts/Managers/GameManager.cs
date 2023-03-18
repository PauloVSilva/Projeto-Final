using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState { None, MainMenu, Hub, MiniGame }

public class GameManager : PersistentSingleton<GameManager>
{
    public bool gameIsPaused;

    [field:SerializeField] public GameState gameState { get; private set; }

    public Camera mainCamera;
    
    public List<PlayerInput> playerList = new List<PlayerInput>();
    public GameObject[] spawnPoints;

    public InputAction joinAction;

    //EVENTS
    public event System.Action<PlayerInput> OnPlayerJoinedGame;
    public event System.Action<PlayerInput> OnPlayerLeftGame;
    public event System.Action<GameState> OnGameStateChanged;

    protected override void Awake(){
        base.Awake();

        gameIsPaused = false;
        
        joinAction.performed += context => {JoinAction(context); Debug.Log("Player Joined");};
    }


    public void UpdateGameState(GameState _gameState)
    {
        gameState = _gameState;

        switch (gameState)
        {
            case GameState.MainMenu:
                joinAction.Disable(); 
                break;
            case GameState.Hub:
                joinAction.Enable();
                break;
            case GameState.MiniGame:
                joinAction.Disable();
                break;
        }

        CanvasManager.instance.playerPanels.SetActive(gameState != GameState.MainMenu);
        CanvasManager.instance.miniGameUI.SetActive(gameState != GameState.MainMenu);

        OnGameStateChanged?.Invoke(gameState);
    }


    private void OnPlayerJoined(PlayerInput playerInput)
    {
        //This callback is called by PlayerInputManager whenever JoinAction method is performed after a player joins
        //So even though is might show up as "0 references" by intellisense, it is called by Unity automatically

        playerList.Add(playerInput);

        OnPlayerJoinedGame?.Invoke(playerInput);

        joinAction.Enable();
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        //This callback is called by PlayerInputManager after a player leaves
        //So even though is might show up as "0 references" by intellisense, it is called by Unity automatically
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
