using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class DimeDropManager : MonoBehaviour{
    //INSTANCES
    public static DimeDropManager instance = null;

    public Camera mainCamera;
    public GameObject[] spawnersList;
    public GameObject[] coins;
    public float countDown = 10;
    public int goal = 100000;
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
            playerInput.GetComponent<PlayerInputHandler>().Spawn(GameManager.instance.dimeDropPrefabs, 0);
            playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.GetComponent<PlayerInput>().actions.Disable();
        }
    }

    private void OnEnable(){
        HealthSystem.OnPlayerDied += PlayerDied;
        HealthSystem.OnPlayerReborn += PlayerReborn;
    }

    private void OnDisable(){
        HealthSystem.OnPlayerDied -= PlayerDied;
        HealthSystem.OnPlayerReborn -= PlayerReborn;
    }

    private void PlayerDied(GameObject gameObject){
        //float delay = gameObject.GetComponent<HealthSystem>().timeToRespawn; 
        //gameObject.transform.parent.GetComponent<PlayerInputHandler>().RespawnPlayer(delay);
        //gameObject.transform.parent.GetComponent<PlayerInput>().actions.Disable();
        gameObject.transform.parent.GetComponent<PlayerStatManager>().IncreaseDeathCount();
    }

    private void PlayerReborn(GameObject gameObject){
        //gameObject.transform.parent.GetComponent<PlayerInput>().actions.Enable();
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
        foreach(var spawner in spawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = true;
        }
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInput>().actions["Movement"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Sprint"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Dash"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Interact"].Enable();
        }
        thisGameState++;
    }

    private void GameIsRunning(){
        foreach(var player in GameManager.instance.playerList){
            if (player.transform.GetComponent<PlayerStatManager>().score >= goal){
                Debug.Log("Player " + player.transform.GetComponent<PlayerStatManager>().thisPlayerColor.ToString() + " is the winner");
                thisGameState++;
            }
        }
    }

    private void GameIsOverSetUp(){
        countDown = 10;
        foreach(var spawner in spawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = false;
        }

        coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach(var coin in coins){
            coin.GetComponent<Coin>().canBePickedUp = false;
        }
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
