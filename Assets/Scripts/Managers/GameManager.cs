using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState { None, MainMenu, Hub, MiniGame, Paused }

public class GameManager : Singleton<GameManager>
{
    [field:SerializeField] public GameState gameState { get; private set; }
    [field:SerializeField] public GameState previousState { get; private set; }

    public Camera mainCamera;
    
    public List<PlayerInput> playerList = new List<PlayerInput>();

    public InputAction joinAction;

    //EVENTS
    public event System.Action<PlayerInput> OnPlayerJoinedGame;
    public event System.Action<PlayerInput> OnPlayerLeftGame;
    public event System.Action<GameState> OnGameStateChanged;

    protected override void Awake(){
        base.Awake();

        LevelLoader.Instance.LoadLevel("MainMenu");
        
        joinAction.performed += context => {JoinAction(context); Debug.Log("Player Joined");};
    }


    public void UpdateGameState(GameState _gameState)
    {
        gameState = _gameState;

        if (gameState != GameState.Paused) previousState = _gameState;

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
            case GameState.Paused:
                joinAction.Disable();
                break;
        }

        Time.timeScale = Convert.ToSingle(gameState != GameState.Paused);

        OnGameStateChanged?.Invoke(gameState);
    }

    public void RestorePreviousState()
    {
        UpdateGameState(previousState);
    }


    private void OnPlayerJoined(PlayerInput playerInput)
    {
        //This callback is called by PlayerInputManager whenever JoinAction method is performed after a player joins
        //So even though is might show up as "0 references" by intellisense, it is called by Unity automatically

        playerList.Add(playerInput);

        OnPlayerJoinedGame?.Invoke(playerInput);

        LevelManager.Instance.currentLevel.SpawnPlayerRandomly(playerInput);
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        //This callback is called by PlayerInputManager after a player leaves
        //So even though is might show up as "0 references" by intellisense, it is called by Unity automatically
    }

    private void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    public void UnregisterPlayer(PlayerInput playerInput)
    {
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
