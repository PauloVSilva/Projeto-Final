using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SharpshooterManager : MonoBehaviour{
    //INSTANCES
    public static SharpshooterManager instance = null;

    [SerializeField] private MiniGameUIManager miniGameUIManager;

    //ENUMS
    public enum GameState{preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver}
    public enum GameGoal{killCount, lastStanding}

    //MINIGAME VARIABLES
    [SerializeField] private GameObject[] spawnersList;
    [SerializeField] private List<PlayerInput> playersAlive = new List<PlayerInput>();
    [SerializeField] public GameState gameState;
    [SerializeField] public GameGoal gameGoal;
    [SerializeField] private int countDown;
    [SerializeField] public int killCountGoal;

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
        killCountGoal = 1;
        gameState = GameState.preparation;
        gameGoal = GameGoal.killCount;
    }

    private void Start(){
        miniGameUIManager = GameObject.FindWithTag("MiniGameUI").GetComponent<MiniGameUIManager>();
        miniGameUIManager.InitializeVariables();
        //move players to spawn
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.transform.GetComponent<CharacterEvents>().ResetScores();
            playerInput.transform.GetComponent<CharacterSelection>().characterObject.transform.position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.GetComponent<PlayerInput>().actions.Disable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();
        
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill += VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied += VerifyLastStandingWinCondition;
            //playerInput.GetComponent<CharacterEvents>().OnPlayerBorn += VerifyWinCondition;

            if(gameGoal == GameGoal.lastStanding){
                playerInput.transform.GetComponent<CharacterStats>().totalLives = 1;
                playerInput.transform.GetComponent<CharacterStats>().unlimitedLives = false;
            }
        }
        StartCoroutine(Preparation());
        //OnGameGoalIsSet?.Invoke();
        DisplayGoal();
    }

    private void OnDisable() {
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied -= VerifyLastStandingWinCondition;
            //playerInput.GetComponent<CharacterEvents>().OnPlayerBorn -= VerifyWinCondition;
        }
        miniGameUIManager.InitializeVariables();
    }

    private void DisplayGoal(){
        if(gameGoal == GameGoal.killCount){
            miniGameUIManager.SetGameGoalText("Whoever gets " + SharpshooterManager.instance.killCountGoal + " kills wins");
        }
        if(gameGoal == GameGoal.lastStanding){
            miniGameUIManager.SetGameGoalText("Last player standing wins");
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
        foreach(var playerInput in GameManager.instance.playerList){
            playersAlive.Add(playerInput);
            playerInput.GetComponent<PlayerInput>().actions["Movement"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Sprint"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Jump"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Dash"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["Interact"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["CockHammer"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["PressTrigger"].Enable();
            playerInput.GetComponent<PlayerInput>().actions["ReloadWeapon"].Enable();
        }
        gameState++;
        //OnGameStateAdvances?.Invoke();
    }

    private void VerifyLastStandingWinCondition(GameObject player){
        if(gameGoal == GameGoal.lastStanding){
            if(!player.transform.parent.GetComponent<CharacterStats>().CanRespawn()){
                playersAlive.Remove(player.transform.parent.GetComponent<PlayerInput>());
            }
            if (playersAlive.Count == 1){
                //Debug.Log("Player " + playersAlive[0].transform.GetComponent<CharacterStats>().animal.ToString() + " is the winner");
                Debug.Log("Player " + (playersAlive[0].playerIndex + 1).ToString() + " is the winner");
                gameState++;
                //OnGameStateAdvances?.Invoke();
                //OnPlayerWins?.Invoke(playersAlive[0]);
                miniGameUIManager.AnnounceWinner(playersAlive[0]);
                GameOverSetUp();
            }
        }
    }

    private void VerifyKillCountWinCondition(GameObject player){
        if(gameGoal == GameGoal.killCount){
            if (player.transform.parent.GetComponent<CharacterStats>().kills >= killCountGoal){
                Debug.Log("Player " + player.transform.parent.GetComponent<CharacterStats>().animal.ToString() + " is the winner");
                gameState++;
                //OnGameStateAdvances?.Invoke();
                //OnPlayerWins?.Invoke(player.transform.parent.GetComponent<PlayerInput>());
                miniGameUIManager.AnnounceWinner(player.transform.parent.GetComponent<PlayerInput>());
                GameOverSetUp();
            }
        }
    }

    private void GameOverSetUp(){
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
