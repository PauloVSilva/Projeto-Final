using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class GameManager : MonoBehaviour{
    //INSTANCES
    public static GameManager instance = null;
    public bool gameIsPaused;
    public bool miniGameIsRunning;
    [SerializeField] private LevelLoader levelLoader;
    public Camera mainCamera;
    
    public List<PlayerInput> playerList = new List<PlayerInput>();
    public GameObject spawnPointPrefab;
    public GameObject[] spawnPoints;
    public GameObject DeathSpot;

    public InputAction joinAction;
    public InputAction leaveAction;

    //EVENTS
    public event Action<PlayerInput> OnPlayerJoinedGame;
    public event Action<PlayerInput> OnPlayerLeftGame;

    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null){
            Destroy(gameObject);
        }

        gameIsPaused = false;
        miniGameIsRunning = false;

        joinAction.Enable();
        joinAction.performed += context => {JoinAction(context); Debug.Log("Player Joined");};

        //leaveAction.Enable();
        //leaveAction.performed += context => LeaveAction(context);
    }

    private void Start(){
        SetSpawnPoint();
    }

    public void ReturnToMainMenu(){
        LoadMiniGame("MainMenu");
    }

    public void ReturnToMainHub(){
        LoadMiniGame("MainHub");
    }

    public void LoadMiniGame(string _levelName){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.transform.GetComponent<CharacterStats>().ResetStats();
        }
        levelLoader.LoadLevel(_levelName);
    }



    public void SetSpawnPoint(){
        FindSpawnPoints();
        if (spawnPoints.Count() < 1 || spawnPoints[0] == null){
            CreateSpawnPoint();
            Debug.Log("Player spawnpoints array was empty");
        }
        FindSpawnPoints();
    }

    private void FindSpawnPoints(){
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    private void CreateSpawnPoint(){
        Instantiate(spawnPointPrefab, GameManager.instance.transform.position, Quaternion.identity); 
    }



    public void GameManagerCharacterKilled(GameObject character){
        //print("GameManager detected kill" + character.transform.parent.GetComponent<CharacterStats>().teamColor);
    }

    public void GameManagerCharacterDied(GameObject character){
        //print("GameManager detected death" + character.transform.parent.GetComponent<CharacterStats>().teamColor);
        mainCamera.GetComponent<CameraController>().RemoveCharacter(character);
    }

    public void GameManagerCharacterSpawned(GameObject character){
        //print("GameManager detected spawn" + character.transform.parent.GetComponent<CharacterStats>().teamColor);
        mainCamera.GetComponent<CameraController>().AddCharacter(character);
    }



    void OnPlayerJoined(PlayerInput playerInput){ //THIS METHOD COMES FROM UNITY ITSELF
        playerList.Add(playerInput);
        OnPlayerJoinedGame?.Invoke(playerInput);
        
        playerInput.transform.GetComponent<CharacterEvents>().SubscribeToPlayerEvents();
    }

    void OnPlayerLeft(PlayerInput playerInput){ //THIS METHOD COMES FROM UNITY ITSELF
        //Debug.Log("Player left - Goodbye!");
    }

    public void JoinAction(InputAction.CallbackContext context){
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
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

    public void UnregisterPlayer(PlayerInput playerInput){
        playerList.Remove(playerInput);
        mainCamera.GetComponent<CameraController>().RemovePlayer(playerInput);
        OnPlayerLeftGame?.Invoke(playerInput);
        Destroy(playerInput.transform.gameObject);
    }
}
