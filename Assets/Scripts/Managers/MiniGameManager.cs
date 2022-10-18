using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum MiniGame{sharpShooter, dimeDrop}
public enum MiniGameGoal{killCount, lastStanding, time, scoreAmount}
public enum MiniGameState{none, preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver}

public abstract class MiniGameManager : LevelManager{
    [SerializeField] public static MiniGameManager instance;

    //MINIGAME VARIABLES
    [SerializeField] public MiniGameGoalScriptableObject miniGameSetup;
    [SerializeField] public MiniGame miniGame;
    [SerializeField] public MiniGameGoal gameGoal;
    [SerializeField] public MiniGameState gameState;
    [SerializeField] protected GameObject[] itemSpawnersList;
    [SerializeField] public float timeElapsed;
    [SerializeField] protected int countDown;

    //MINIGAME ACTION EVENTS
    public event System.Action<int> OnCountDownTicks;
    public event System.Action<string> OnGameGoalIsSet;
    public event System.Action OnGameStateAdvances;
    public event System.Action OnGameEnds;
    public event System.Action<PlayerInput> OnPlayerWins;

    protected void CountDownTicks(){
        countDown--;
        OnCountDownTicks?.Invoke(countDown);
    }

    protected void GameGoalIsSet(){
        OnGameGoalIsSet?.Invoke(MiniGameOptionsMenu.instance.GetMiniGameGoalDescription());
    }

    protected void GameStateAdvances(){
        gameState++;
        if(gameState == MiniGameState.preparation) {countDown = 10; StartCoroutine(Preparation());} 
        if(gameState == MiniGameState.gameSetUp) {StartGame();}
        if(gameState == MiniGameState.gameIsRunning) {}
        if(gameState == MiniGameState.gameOverSetUp) {GameOverSetUp();}
        if(gameState == MiniGameState.gameOver) {countDown = 10; /*StartCoroutine(GameOver()); Debug.Log("called gameover");*/}
        //for some fucked up reason, the last verification is called twice and I have no fucking clue why

        OnGameStateAdvances?.Invoke();
    }

    protected void PlayerWins(PlayerInput playerInput){
        OnPlayerWins?.Invoke(playerInput);
    }

    protected void Update(){
        if(gameState == MiniGameState.gameIsRunning){
            timeElapsed += Time.deltaTime;
            CheckMiniGameEvents();
        }
    }

    protected override void InitializeSingletonInstance(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    protected virtual void CheckMiniGameEvents(){}

    protected override void InitializeLevel(){
        GameManager.instance.joinAction.Disable();
        GameManager.instance.miniGameIsRunning = true;

        gameState = MiniGameState.none;

        itemSpawnersList = GameObject.FindGameObjectsWithTag("Spawner");

        foreach(var playerInput in GameManager.instance.playerList){
            int index = playerInput.playerIndex % GameManager.instance.spawnPoints.Length;
            playerInput.transform.position = GameManager.instance.spawnPoints[index].transform.position;
            playerInput.GetComponent<CharacterManager>().BlockActions();
        }

        miniGameSetup = MiniGameOptionsMenu.instance.GetMiniGameGoal();
        miniGame = miniGameSetup.parentMiniGame.minigame;
        gameGoal = miniGameSetup.miniGameGoal;
        SetupGame();
        GameStateAdvances();
    }

    protected abstract void SetupGame();

    protected IEnumerator Preparation(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            CountDownTicks();
            if(gameState == MiniGameState.preparation){
                StartCoroutine(Preparation());
            }
        }
        else {
            GameGoalIsSet();
            GameStateAdvances();
        }
    }

    protected abstract void StartGame();

    protected abstract void GameOverSetUp();

    protected IEnumerator GameOver(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            CountDownTicks();
            if(gameState == MiniGameState.gameOver){
                StartCoroutine(GameOver());
            }
        }
        else {
            OnGameEnds?.Invoke();
            GameManager.instance.ReturnToMainHub();
        }
    }
}
