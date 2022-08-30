using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpshooterManager : MonoBehaviour{
    //INSTANCES
    public static SharpshooterManager instance = null;
    
    //public GameObject[] playerPrefabs;
    public GameObject[] spawnersList;

    public GameObject[] playersAlive;

    public enum gameState{
        preparation,
        gameIsRunningSetUp,
        gameIsRunning,
        gameIsOverSetUp,
        gameIsOver
    }
    [SerializeField] private gameState thisGameState = gameState.preparation;

    public float countDown = 10;

    private void Awake(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        GameManager.instance.SetSpawnPoint();

        spawnersList = GameObject.FindGameObjectsWithTag("Spawner");

        //move players to spawn
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().Destroy();
            playerInput.GetComponent<PlayerInputHandler>().Spawn(GameManager.instance.sharpshooterPrefabs, 0);
            playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
        }

        GameManager.instance.joinAction.Disable();
        GameManager.instance.leaveAction.Disable();
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
            thisGameState++;
            Debug.Log("Preparation time ended");
        }
    }

    private void GameIsRunningSetUp(){
        thisGameState++;
        foreach(var spawner in spawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = true;
        }
    }

    private void GameIsRunning(){
        if (playersAlive.Length == 5){
            Debug.Log("Player " + playersAlive[0].transform.GetChild(0).GetComponent<PlayerController>().thisPlayerColor.ToString() + " is the winner");
            thisGameState++;
        }
    }

    private void GameIsOverSetUp(){
        countDown = 10;
        thisGameState++;
        foreach(var spawner in spawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = false;
        }

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
