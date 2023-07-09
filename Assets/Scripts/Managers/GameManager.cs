using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState { None, MainMenu, Hub, MiniGame, Paused }

public class GameManager : Singleton<GameManager>
{
    [field:SerializeField] public GameState GameState { get; private set; }
    [field:SerializeField] public GameState PreviousState { get; private set; }

    public Camera mainCamera;
    
    public List<PlayerInput> playerList = new List<PlayerInput>();

    public InputAction joinAction;

    //EVENTS
    public event System.Action<PlayerInput> OnPlayerJoinedGame;
    public event System.Action<PlayerInput> OnPlayerLeftGame;
    public event System.Action<GameState> OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        
        joinAction.performed += context => {JoinAction(context); Debug.Log("Player Joined");};
    }

    private void Start()
    {
        LevelLoader.Instance.LoadLevel("MainMenu");
    }


    public void UpdateGameState(GameState _gameState)
    {
        GameState = _gameState;

        if (GameState != GameState.Paused) PreviousState = _gameState;

        EnableNewPlayers(GameState == GameState.Hub);

        Time.timeScale = Convert.ToSingle(GameState != GameState.Paused);

        OnGameStateChanged?.Invoke(GameState);
    }

    public void RestorePreviousState()
    {
        UpdateGameState(PreviousState);
    }

    private void EnableNewPlayers(bool _enabled)
    {
        if(_enabled) joinAction.Enable();
        else joinAction.Disable();
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

        StartCoroutine(DestroyDelay());
        IEnumerator DestroyDelay()
        {
            yield return new WaitForEndOfFrame();

            Destroy(playerInput.transform.gameObject);
        }
    }
    
    public void UnregisterAllPlayers()
    {
        while (playerList.Count() > 0)
        {
            UnregisterPlayer(playerList[0]);
        }
    }

    public void BlockAllPlayerActions(bool _block)
    {
        foreach (var playerInput in playerList)
        {
            playerInput.GetComponent<CharacterManager>().BlockActions(_block);
        }
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
