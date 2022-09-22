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

    //MINIGAME VARIABLES
    [SerializeField] private GameObject[] spawnersList;
    [SerializeField] private List<PlayerInput> playersAlive = new List<PlayerInput>();
    [SerializeField] public MiniGameGoalScriptableObject miniGameSetup;
    [SerializeField] public MiniGame miniGame;
    [SerializeField] public MiniGameGoal gameGoal;
    [SerializeField] public MiniGameState gameState;
    [SerializeField] private int countDown;
    [SerializeField] public int killCountGoal;
    [SerializeField] public int lastStandingLives;

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

        countDown = 10;
        gameState = MiniGameState.preparation;
    }

    private void Start(){
        miniGameUIManager = GameObject.FindWithTag("MiniGameUI").GetComponent<MiniGameUIManager>();
        miniGameUIManager.InitializeVariables();

        SetupGame();

        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterSelection>().characterObject.transform.position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.actions.Disable();
            playerInput.actions["Jump"].Enable();
        }

        StartCoroutine(Preparation());
        //OnGameGoalIsSet?.Invoke();
        DisplayGoal();
    }

    private void OnDisable() {
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied -= VerifyLastStandingWinCondition;
        }
        miniGameUIManager.InitializeVariables();
    }

    private void SetupGame(){
        miniGameSetup = MiniGameOptionsMenu.instance.GetMiniGameGoal();
        miniGame = miniGameSetup.parentMiniGame.minigame;
        gameGoal = miniGameSetup.miniGameGoal;

        if(miniGame == MiniGame.sharpShooter){
            if(gameGoal == MiniGameGoal.killCount){
                killCountGoal = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach (var playerInput in GameManager.instance.playerList){
                    playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill += VerifyKillCountWinCondition;
                }
            }
            if(gameGoal == MiniGameGoal.lastStanding){
                lastStandingLives = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach (var playerInput in GameManager.instance.playerList){
                    playerInput.GetComponent<CharacterEvents>().OnPlayerDied += VerifyLastStandingWinCondition;
                    playerInput.GetComponent<CharacterEvents>().SetLimitedLives(lastStandingLives);
                    playersAlive.Add(playerInput);
                }
            }
        }
    }

    private void DisplayGoal(){
        miniGameUIManager.SetGameGoalText(miniGameSetup.goalDescription);
    }

    IEnumerator Preparation(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            //OnCountDownTicks?.Invoke(countDown);
            miniGameUIManager.DisplayCountDown(countDown);
            if(gameState == MiniGameState.preparation){
                StartCoroutine(Preparation());
            }
        }
        else {
            Debug.Log("Preparation time ended");
            gameState++;
            //OnGameStateAdvances?.Invoke();
            StartGame();
        }
    }

    private void StartGame(){
        foreach(var playerInput in GameManager.instance.playerList){
            //playerInput.actions["PauseMenu"].Enable();
            playerInput.actions["Movement"].Enable();
            playerInput.actions["Sprint"].Enable();
            playerInput.actions["Jump"].Enable();
            playerInput.actions["Dash"].Enable();
            playerInput.actions["Interact"].Enable();
            playerInput.actions["CockHammer"].Enable();
            playerInput.actions["PressTrigger"].Enable();
            playerInput.actions["ReloadWeapon"].Enable();
        }
        gameState++;
        //OnGameStateAdvances?.Invoke();
    }

    private void VerifyLastStandingWinCondition(GameObject player){
        if(gameGoal == MiniGameGoal.lastStanding){
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
        if(gameGoal == MiniGameGoal.killCount){
            if (player.transform.parent.GetComponent<CharacterStats>().kills >= killCountGoal){
                Debug.Log("Player " + (player.transform.parent.GetComponent<PlayerInput>().playerIndex + 1).ToString() + " is the winner");
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
