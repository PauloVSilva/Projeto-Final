using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class SharpshooterManager : MonoBehaviour{
    //INSTANCES
    public static SharpshooterManager instance = null;

    public Camera mainCamera;
    public GameObject[] spawnersList;
    public List<PlayerInput> playersAlive = new List<PlayerInput>();
    public float countDown = 10;
    public enum gameState{
        preparation,
        gameIsRunningSetUp,
        gameIsRunning,
        gameIsOverSetUp,
        gameIsOver
    }

    [SerializeField] private gameState thisGameState = gameState.preparation;

    private void Awake(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        GameManager.instance.joinAction.Disable();
        GameManager.instance.leaveAction.Disable();
        GameManager.instance.SetSpawnPoint();
        spawnersList = GameObject.FindGameObjectsWithTag("Spawner");

        //move players to spawn
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().Destroy();
            playerInput.GetComponent<PlayerInputHandler>().Spawn(GameManager.instance.sharpshooterPrefabs, 0);
            playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.GetComponent<PlayerInput>().actions.Disable();
        }
    }

    private void OnEnable(){
        HealthSystem.OnPlayerDied += PlayerKilled;
        HealthSystem.OnPlayerReborn += PlayerReborn;
    }

    private void OnDisable(){
        HealthSystem.OnPlayerDied -= PlayerKilled;
        HealthSystem.OnPlayerReborn -= PlayerReborn;
    }

    private void PlayerKilled(GameObject gameObject){
        //gameObject.transform.parent.GetComponent<PlayerInputHandler>().RespawnPlayer(gameObject);
        playersAlive.Remove(gameObject.transform.parent.GetComponent<PlayerInput>());
        gameObject.transform.parent.GetComponent<PlayerInput>().actions.Disable();
    }

    private void PlayerReborn(GameObject gameObject){
    }

    private void Update(){
        if((int)thisGameState == 0){
            Preparation();
        }

        if((int)thisGameState == 1){
            GameIsRunningSetUp();
        }

        if((int)thisGameState == 2){
            GameIsRunning();
        }

        if((int)thisGameState == 3){
            GameIsOverSetUp();
        }

        if((int)thisGameState == 4){
            GameIsOver();
        }
    }

    private void Preparation(){
        if(countDown > 0){
            countDown -= 1 * Time.deltaTime;
        }
        else {
            Debug.Log("Preparation time ended");
            thisGameState++;
        }
    }

    private void GameIsRunningSetUp(){
        foreach(var player in GameManager.instance.playerList){
            playersAlive.Add(player);
        }
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInput>().actions["Movement"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Sprint"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Dash"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Interact"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["CockHammer"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["PressTrigger"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["ReloadWeapon"].Enable();
        }
        thisGameState++;
    }

    private void GameIsRunning(){
        if (playersAlive.Count == 1){
            Debug.Log("Player " + playersAlive[0].transform.GetChild(0).GetComponent<PlayerController>().thisPlayerColor.ToString() + " is the winner");
            thisGameState++;
        }
    }

    private void GameIsOverSetUp(){
        countDown = 10;
        thisGameState++;
    }

    private void GameIsOver(){
        if(countDown > 0){
            countDown -= 1 * Time.deltaTime;
        }
        else {
            GameManager.instance.ReturnToMainHub();
            Debug.Log("Returning to MainHub");
        }
    }

}
