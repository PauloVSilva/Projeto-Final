using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class GameManager : PersistentSingleton<GameManager>
{

    public bool gameIsPaused;
    public bool miniGameIsRunning;

    public ItemsDatabank itemsDatabank;

    public Camera mainCamera;
    
    public List<PlayerInput> playerList = new List<PlayerInput>();
    public GameObject[] spawnPoints;
    public GameObject DeathSpot;

    public InputAction joinAction;

    //EVENTS
    public event System.Action<PlayerInput> OnPlayerJoinedGame;
    public event System.Action<PlayerInput> OnPlayerLeftGame;

    protected override void Awake(){
        base.Awake();

        gameIsPaused = false;
        miniGameIsRunning = false;
        
        joinAction.performed += context => {JoinAction(context); /*joinAction.Disable();*/ Debug.Log("Player Joined");};
    }


    public void FullyResetPlayers(){
        if(GameManager.Instance.playerList.Count > 0){
            mainCamera.GetComponent<CameraController>().ClearList();
            foreach(var playerInput in GameManager.Instance.playerList){
                playerInput.GetComponent<CharacterManager>().FullReset();
            }
        }
    }



    public void GameManagerCharacterKilled(GameObject character){
        //print("GameManager detected kill" + character.GetComponent<PlayerInput>().playerIndex);
    }

    public void GameManagerCharacterDied(GameObject character){
        //print("GameManager detected death" + character.GetComponent<PlayerInput>().playerIndex);
        mainCamera.GetComponent<CameraController>().RemoveCharacter(character);
    }

    public void GameManagerCharacterSpawned(GameObject character){
        //print("GameManager detected spawn" + character.GetComponent<PlayerInput>().playerIndex);
        mainCamera.GetComponent<CameraController>().AddCharacter(character);
    }



    void OnPlayerJoined(PlayerInput playerInput){ //THIS METHOD COMES FROM UNITY ITSELF
        playerList.Add(playerInput);
        OnPlayerJoinedGame?.Invoke(playerInput);
        
        playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill += GameManagerCharacterKilled;
        playerInput.GetComponent<CharacterManager>().OnPlayerDied += GameManagerCharacterDied;
        playerInput.GetComponent<CharacterManager>().OnPlayerBorn += GameManagerCharacterSpawned;

        joinAction.Enable();
    }

    void OnPlayerLeft(PlayerInput playerInput){ //THIS METHOD COMES FROM UNITY ITSELF
        playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill -= GameManagerCharacterKilled;
        playerInput.GetComponent<CharacterManager>().OnPlayerDied -= GameManagerCharacterDied;
        playerInput.GetComponent<CharacterManager>().OnPlayerBorn -= GameManagerCharacterSpawned;
    }

    public void JoinAction(InputAction.CallbackContext context){
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    public void UnregisterPlayer(PlayerInput playerInput){
        playerList.Remove(playerInput);
        mainCamera.GetComponent<CameraController>().RemovePlayer(playerInput);
        OnPlayerLeftGame?.Invoke(playerInput);
        Destroy(playerInput.transform.gameObject);
    }

    public void LeaveAction(InputAction.CallbackContext context){
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
    }
}
