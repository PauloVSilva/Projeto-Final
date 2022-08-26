using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    private void Start(){
        SetSpawnPoint();
        //PlayerInputManager.instance.JoinPlayer(0, -1, null);
        //Debug.Log(PlayerJoinedGame);
    }

    public void ReturnToMainHub(){
        SceneManager.LoadScene("MainHub");
        StartCoroutine(SetSpawnPointDelayed());
    }

    IEnumerator SetSpawnPointDelayed(){
        yield return new WaitForSeconds(0.01f);
        FindSpawnPoints();
        if (spawnPoints.Count() < 1 || spawnPoints[0] == null){
            CreateSpawnPoint();
        }
        FindSpawnPoints();
        foreach(var player in playerList){
            Debug.Log("teleporting");
            player.transform.parent.position = spawnPoints[0].transform.position;
        }
    }

    public void SetSpawnPoint(){
        FindSpawnPoints();
        if (spawnPoints.Count() < 1 || spawnPoints[0] == null){
            CreateSpawnPoint();
        }
        FindSpawnPoints();
    }

    void FindSpawnPoints(){
        //Debug.Log("Finding SpawnPoint");
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    void CreateSpawnPoint(){
        //Debug.Log("Creating SpawnPoint");
        Instantiate(spawnPointPrefab, GameManager.instance.transform.position, Quaternion.identity); 
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
