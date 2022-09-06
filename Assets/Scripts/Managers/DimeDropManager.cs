using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DimeDropManager : MonoBehaviour{
    //INSTANCES
    public static DimeDropManager instance = null;

    //ENUMS
    private enum gameState{preparation, gameIsRunningSetUp, gameIsRunning, gameIsOverSetUp, gameIsOver}
    private enum gameGoal{time, amount}

    //VARIABLES
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject[] spawnersList;
    [SerializeField] private GameObject[] coins;
    [SerializeField] private gameState thisGameState;
    //[SerializeField] private gameGoal thisGameGoal;
    [SerializeField] private float countDown;
    [SerializeField] private int amountGoal;

    //EVENTS
    public static event Action<float> OnCountDownTicks;

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
    }

    private void Start(){
        //move players to spawn
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.GetComponent<PlayerInput>().actions.Disable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();
            mainCamera.GetComponent<CameraController>().AddPlayer(playerInput);
        }
        thisGameState = gameState.preparation;
        //thisGameGoal = gameGoal.amount;
        countDown = 10;
        amountGoal = 10000;
        StartCoroutine(Preparation());
    }

    private void Update(){
        //if((int)thisGameState == 0){
        //    Preparation();
        //}

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

    IEnumerator Preparation(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            //countDown -= 1 * Time.deltaTime;
            countDown--;
            OnCountDownTicks?.Invoke(countDown);
            StartCoroutine(Preparation());
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
            if (player.transform.GetComponent<OldPlayerStatManager>().score >= amountGoal){
                Debug.Log("Player " + player.transform.GetComponent<OldPlayerStatManager>().thisPlayerColor.ToString() + " is the winner");
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
