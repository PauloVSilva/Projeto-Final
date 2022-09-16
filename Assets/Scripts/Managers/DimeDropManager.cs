using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DimeDropManager : MonoBehaviour{
    //INSTANCES
    public static DimeDropManager instance = null;

    [SerializeField] private MiniGameUIManager miniGameUIManager;

    //ENUMS
    public enum GameState{preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameIsOver}
    public enum GameGoal{time, scoreAmount}

    //MINIGAME VARIABLES
    [SerializeField] private GameObject[] spawnersList;
    [SerializeField] private GameObject[] coins;
    [SerializeField] public GameState gameState;
    [SerializeField] public GameGoal gameGoal;
    [SerializeField] private int countDown;
    [SerializeField] public int amountGoal;
    [SerializeField] public int timeLimitGoal;

    //EVENTS
    //public static event Action<int> OnCountDownTicks;
    //public static event Action<PlayerInput> OnPlayerWins;
    //public static event Action OnGameStateAdvances;
    //public static event Action OnGameGoalIsSet;

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
        
        countDown = 10;
        amountGoal = 20000;
        gameState = GameState.preparation;
        gameGoal = GameGoal.scoreAmount;
    }

    private void Start(){
        miniGameUIManager = GameObject.FindWithTag("MiniGameUI").GetComponent<MiniGameUIManager>();
        miniGameUIManager.InitializeVariables();
        //move players to spawn
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.transform.GetComponent<CharacterEvents>().ResetScores();
            playerInput.transform.GetComponent<CharacterSelection>().characterObject.transform.position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.actions.Disable();
            playerInput.actions["Jump"].Enable();

            playerInput.GetComponent<CharacterEvents>().OnPlayerScoreChanged += VerifyScoreAmountWinCondition;
        }
        StartCoroutine(Preparation());
        //OnGameGoalIsSet?.Invoke();
        DisplayGoal();
    }

    private void OnDisable() {
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoreChanged -= VerifyScoreAmountWinCondition;
        }
        miniGameUIManager.InitializeVariables();
    }

    private void DisplayGoal(){
        if(gameGoal == GameGoal.scoreAmount){
            miniGameUIManager.SetGameGoalText("Whoever gets " + amountGoal + " first wins");
        }
        if(gameGoal == GameGoal.time){
            miniGameUIManager.SetGameGoalText("Whoever gets the most coins before " + timeLimitGoal + " wins");
        }
    }

    IEnumerator Preparation(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            //OnCountDownTicks?.Invoke(countDown);
            miniGameUIManager.DisplayCountDown(countDown);
            if(gameState == GameState.preparation){
                StartCoroutine(Preparation());
            }
        }
        else {
            Debug.Log("Preparation time ended");
            gameState++;
            //OnGameStateAdvances?.Invoke();
            GameSetUp();
        }
    }

    private void GameSetUp(){
        foreach(var spawner in spawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = true;
        }
        foreach(var playerInput in GameManager.instance.playerList){
            //playerInput.actions["PauseMenu"].Enable();
            playerInput.actions["Movement"].Enable();
            playerInput.actions["Sprint"].Enable();
            playerInput.actions["Jump"].Enable();
            playerInput.actions["Dash"].Enable();
            playerInput.actions["Interact"].Enable();
        }
        gameState++;
        //OnGameStateAdvances?.Invoke();
    }

    private void VerifyScoreAmountWinCondition(GameObject player){
        if(gameGoal == GameGoal.scoreAmount){
            if (player.transform.GetComponent<CharacterStats>().score >= amountGoal){
                Debug.Log("Player " + player.transform.GetComponent<CharacterStats>().animal.ToString() + " is the winner");
                gameState++;
                //OnGameStateAdvances?.Invoke();
                //OnPlayerWins?.Invoke(player.transform.parent.GetComponent<PlayerInput>());
                miniGameUIManager.AnnounceWinner(player.GetComponent<PlayerInput>());
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
        //OnGameStateAdvances?.Invoke();
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            //OnCountDownTicks?.Invoke(countDown);
            miniGameUIManager.DisplayCountDown(countDown);
            StartCoroutine(GameOver());
        }
        else {
            Debug.Log("Returning to MainHub");
            GameManager.instance.ReturnToMainHub();
        }
    }

}
