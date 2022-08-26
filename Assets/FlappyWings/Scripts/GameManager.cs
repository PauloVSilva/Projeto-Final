using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class GameManager : MonoBehaviour{
    //INSTANCES
    public static GameManager instance = null;

    public GameObject spawnPointPrefab;
    public GameObject[] spawnPoints;
    public List<PlayerInput> playerList = new List<PlayerInput>();

    [SerializeField] public InputAction joinAction;
    [SerializeField] public InputAction leaveAction;

    //EVENTS
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    private void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        SetSpawnPoint();

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    void FindSpawnPoints(){
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    void CreateSpawnPoint(){
        Instantiate(spawnPointPrefab, GameManager.instance.transform.position, Quaternion.identity); 
    }

    public void SetSpawnPoint(){
        FindSpawnPoints();
        if (spawnPoints.Length < 1){
            CreateSpawnPoint();
        }
        FindSpawnPoints();
    }

    private void Start(){
        //PlayerInputManager.instance.JoinPlayer(0, -1, null);
        //Debug.Log(PlayerJoinedGame);
    }

    void OnPlayerJoined(PlayerInput playerInput){
        playerList.Add(playerInput);
        //if (PlayerJoinedGame != null){
            PlayerJoinedGame(playerInput);
        //}
    }

    void OnPlayerLeft(PlayerInput playerInput){
        //Debug.Log("Player left - Goodbye!");
    }

    void JoinAction(InputAction.CallbackContext context){
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    void LeaveAction(InputAction.CallbackContext context){
        if (playerList.Count > 1){
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
        if(PlayerLeftGame != null){
            PlayerLeftGame(playerInput);
        }
        Destroy(playerInput.transform.parent.gameObject);
    }
}
