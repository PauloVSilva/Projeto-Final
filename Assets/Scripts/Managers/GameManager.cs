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
    public bool GameIsPaused;
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

        GameIsPaused = false;

        joinAction.Enable();
        joinAction.performed += context => {JoinAction(context); Debug.Log("Player Joined");};

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    private void Start(){
        SetSpawnPoint();
    }

    public void ReturnToMainHub(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.transform.GetComponent<CharacterStats>().ResetStats();
            //playerInput.transform.GetComponent<CharacterSelection>().characterObject.GetComponent<HealthSystem>().Kill();
        }
        levelLoader.LoadLevel("MainHub");
    }

    public void ReturnToMainMenu(){
        levelLoader.LoadLevel("MainMenu");
    }

    public void GoToLevel(string levelName){
        levelLoader.LoadLevel(levelName);
    }

    public void SetSpawnPoint(){
        FindSpawnPoints();
        if (spawnPoints.Count() < 1 || spawnPoints[0] == null){
            CreateSpawnPoint();
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

        if(character.transform.parent.GetComponent<CharacterStats>().CanRespawn()){
            StartCoroutine(RespawnCharacter(character));
        }
        int index = GameManager.instance.spawnPoints.Length;
        int randomIndex = UnityEngine.Random.Range(0, index);
        character.transform.position = GameManager.instance.spawnPoints[randomIndex].transform.position;
        character.SetActive(false);
    }

    IEnumerator RespawnCharacter(GameObject character){
        yield return new WaitForSeconds(character.transform.parent.GetComponent<CharacterStats>().timeToRespawn);
        character.SetActive(true);
        character.transform.parent.GetComponent<CharacterEvents>().RefreshStatsUponRespawning();
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
