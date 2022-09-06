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

    public List<PlayerInput> playerList = new List<PlayerInput>();
    public GameObject spawnPointPrefab;
    public GameObject[] spawnPoints;
    public GameObject[] playerPrefabs;
    public GameObject DeathSpot;

    public InputAction joinAction;
    public InputAction leaveAction;

    //EVENTS
    public event Action<PlayerInput> OnPlayerJoinedGame;
    public event Action<PlayerInput> OnPlayerLeftGame;
    //public static event Action MiniGameStarted;

    private void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    private void Start(){
        SetSpawnPoint();
    }

    public void ReturnToMainHub(){
        SceneManager.LoadScene("MainHub");
    }

    public void SetSpawnPoint(){
        FindSpawnPoints();
        if (spawnPoints.Count() < 1 || spawnPoints[0] == null){
            CreateSpawnPoint();
        }
        FindSpawnPoints();
    }

    void FindSpawnPoints(){
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    void CreateSpawnPoint(){
        Instantiate(spawnPointPrefab, GameManager.instance.transform.position, Quaternion.identity); 
    }






    void OnPlayerJoined(PlayerInput playerInput){ //THIS METHOD COMES FROM UNITY ITSELF
        playerList.Add(playerInput);
        OnPlayerJoinedGame?.Invoke(playerInput);

        //hope this works
        //playerInput.transform.GetComponent<CharacterEvents>().SubscribeToEvents(gameObject);
    }

    void OnPlayerLeft(PlayerInput playerInput){ //THIS METHOD COMES FROM UNITY ITSELF
        //Debug.Log("Player left - Goodbye!");
    }

    void JoinAction(InputAction.CallbackContext context){
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    void LeaveAction(InputAction.CallbackContext context){
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

    void UnregisterPlayer(PlayerInput playerInput){
        playerList.Remove(playerInput);
        OnPlayerLeftGame?.Invoke(playerInput);
        Destroy(playerInput.transform.gameObject);
    }
}
