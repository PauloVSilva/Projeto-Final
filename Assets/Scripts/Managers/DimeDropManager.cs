using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DimeDropManager : MonoBehaviour{
    //INSTANCES
    public static DimeDropManager instance = null;

    //ENUMS
    public enum GameState{preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameIsOver}
    public enum GameGoal{time, scoreAmount}

    //MINIGAME VARIABLES
    [SerializeField] private GameObject[] spawnersList;
    [SerializeField] private GameObject[] coins;
    [SerializeField] private GameState gameState;
    [SerializeField] private GameGoal gameGoal;
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
        gameState = GameState.preparation;
        gameGoal = GameGoal.scoreAmount;
        countDown = 10;
        amountGoal = 20000;
        //move players to spawn
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.transform.GetComponent<CharacterEvents>().ResetScores();
            playerInput.transform.GetChild(0).position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.GetComponent<PlayerInput>().actions.Disable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();

            playerInput.GetComponent<CharacterEvents>().OnPlayerScoreChanged += VerifyWinCondition;
        }
        StartCoroutine(Preparation());
    }

    private void OnDisable() {
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoreChanged -= VerifyWinCondition;
        }
    }

    IEnumerator Preparation(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            OnCountDownTicks?.Invoke(countDown);
            StartCoroutine(Preparation());
        }
        else {
            Debug.Log("Preparation time ended");
            gameState++;
            GameSetUp();
        }
    }

    private void GameSetUp(){
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
        gameState++;
    }

    private void VerifyWinCondition(GameObject player){
        if(gameGoal == GameGoal.scoreAmount){
            if (player.transform.GetComponent<CharacterStats>().score >= amountGoal){
                Debug.Log("Player " + player.transform.GetComponent<CharacterStats>().animal.ToString() + " is the winner");
                gameState++;
                GameOverSetUp();
            }
        }
    }

    private void GameOverSetUp(){
        foreach(var spawner in spawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = false;
        }
        coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach(var coin in coins){
            coin.GetComponent<Coin>().canBePickedUp = false;
        }
        countDown = 10;
        gameState++;
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            OnCountDownTicks?.Invoke(countDown);
            StartCoroutine(GameOver());
        }
        else {
            Debug.Log("Returning to MainHub");
            GameManager.instance.ReturnToMainHub();
        }
    }

}
