using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum MiniGame{sharpShooter, dimeDrop}
public enum MiniGameGoal{killCount, lastStanding, time, scoreAmount}
public enum MiniGameState{preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver}

public class MiniGameManager : MonoBehaviour{
    //INSTANCES
    public static MiniGameManager instance = null;

    [SerializeField] private MiniGameUIManager miniGameUIManager;

    //ENUMS

    //MINIGAME VARIABLES
    [SerializeField] private List<PlayerInput> playersAlive = new List<PlayerInput>();
    [SerializeField] public MiniGameGoalScriptableObject miniGameSetup;
    [SerializeField] public MiniGame miniGame;
    [SerializeField] public MiniGameGoal gameGoal;
    [SerializeField] public MiniGameState gameState;

    [SerializeField] private int countDown;
    [SerializeField] public int killCountGoal;
    [SerializeField] public int lastStandingLives;
    [SerializeField] public int scoreAmountGoal;
    [SerializeField] public int timeLimitGoal;
    [SerializeField] private GameObject[] itemSpawnersList;
    [SerializeField] private GameObject[] coins;

    private void Awake(){
        if (instance == null){
            instance = this;
        }
        else if (instance != null){
            Destroy(gameObject);
        }

        GameManager.instance.joinAction.Disable();
        //GameManager.instance.leaveAction.Disable();
        GameManager.instance.SetSpawnPoint();

        countDown = 10;
        gameState = MiniGameState.preparation;
    }

    private void Start(){
        GameManager.instance.miniGameIsRunning = true;

        miniGameUIManager = GameObject.FindWithTag("MiniGameUI").GetComponent<MiniGameUIManager>();
        miniGameUIManager.InitializeVariables();
        itemSpawnersList = GameObject.FindGameObjectsWithTag("Spawner");

        SetupGame();

        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterSelection>().characterObject.transform.position = GameManager.instance.spawnPoints[0].transform.position;
            playerInput.GetComponent<PlayerInputHandler>().DisableMovementActions();
        }

        StartCoroutine(Preparation());
        //OnGameGoalIsSet?.Invoke();
        DisplayGoal();
    }

    private void OnDisable() {
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
        if(miniGame == MiniGame.dimeDrop){
            if(gameGoal == MiniGameGoal.scoreAmount){
                scoreAmountGoal = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach(var playerInput in GameManager.instance.playerList){
                    playerInput.GetComponent<CharacterEvents>().OnPlayerScoreChanged += VerifyScoreAmountWinCondition;
                }
            }
        }
    }

    private void DisplayGoal(){
        miniGameUIManager.SetGameGoalText(MiniGameOptionsMenu.instance.GetMiniGameGoalDescription());
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
            gameState++;
            //OnGameStateAdvances?.Invoke();
            StartGame();
        }
    }

    private void StartGame(){
        if(miniGame == MiniGame.dimeDrop){
            foreach(var spawner in itemSpawnersList){
                spawner.GetComponent<Spawner>().spawnerEnabled = true;
            }
        }
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<PlayerInputHandler>().EnableMovementActions();
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

    private void VerifyScoreAmountWinCondition(GameObject player, int _score){
        if(gameGoal == MiniGameGoal.scoreAmount){
            if (_score >= scoreAmountGoal){
                Debug.Log("Player " + (player.GetComponent<PlayerInput>().playerIndex + 1).ToString() + " is the winner");
                gameState++;
                //OnGameStateAdvances?.Invoke();
                //OnPlayerWins?.Invoke(player.transform.parent.GetComponent<PlayerInput>());
                miniGameUIManager.AnnounceWinner(player.GetComponent<PlayerInput>());
                GameOverSetUp();
            }
        }
    }

    private void GameOverSetUp(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied -= VerifyLastStandingWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoreChanged -= VerifyScoreAmountWinCondition;
        }

        if(miniGame == MiniGame.dimeDrop){
            foreach(var spawner in itemSpawnersList){
                spawner.GetComponent<Spawner>().spawnerEnabled = false;
            }
            coins = GameObject.FindGameObjectsWithTag("Coin");
            foreach(var coin in coins){
                coin.GetComponent<Coin>().canBePickedUp = false;
            }
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
            GameManager.instance.ReturnToMainHub();
        }
    }
}
