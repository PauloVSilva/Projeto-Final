using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum MiniGame{sharpShooter, dimeDrop}
public enum MiniGameGoal{killCount, lastStanding, time, scoreAmount}
public enum MiniGameState{none, preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver}

public abstract class MiniGameManager : LevelManager{
    [SerializeField] protected MiniGameUIManager miniGameUIManager;

    //MINIGAME VARIABLES
    [SerializeField] public MiniGameGoalScriptableObject miniGameSetup;
    [SerializeField] public MiniGame miniGame;
    [SerializeField] public MiniGameGoal gameGoal;
    [SerializeField] public MiniGameState gameState;
    [SerializeField] protected GameObject[] itemSpawnersList;

    [SerializeField] protected int countDown;
    [SerializeField] public int killCountGoal;
    [SerializeField] public int lastStandingLives;
    [SerializeField] public int scoreAmountGoal;
    [SerializeField] public int timeLimitGoal;

    protected override void InitializeLevel(){
        GameManager.instance.joinAction.Disable();
        GameManager.instance.miniGameIsRunning = true;

        countDown = 10;
        gameState = MiniGameState.preparation;

        miniGameUIManager = GameObject.FindWithTag("MiniGameUI").GetComponent<MiniGameUIManager>();
        miniGameUIManager.InitializeVariables();
        itemSpawnersList = GameObject.FindGameObjectsWithTag("Spawner");

        foreach(var playerInput in GameManager.instance.playerList){
            int index = playerInput.playerIndex % GameManager.instance.spawnPoints.Length;
            playerInput.transform.position = GameManager.instance.spawnPoints[index].transform.position;
            playerInput.GetComponent<CharacterEvents>().BlockActions();
        }

        miniGameSetup = MiniGameOptionsMenu.instance.GetMiniGameGoal();
        miniGame = miniGameSetup.parentMiniGame.minigame;
        gameGoal = miniGameSetup.miniGameGoal;
        SetupGame();

        StartCoroutine(Preparation());
        //OnGameGoalIsSet?.Invoke();
        DisplayGoal();
    }

    protected abstract void SetupGame();

    private void DisplayGoal(){
        miniGameUIManager.SetGameGoalText(MiniGameOptionsMenu.instance.GetMiniGameGoalDescription());
    }

    protected IEnumerator Preparation(){
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

    protected abstract void StartGame();

    protected abstract void GameOverSetUp();

    protected IEnumerator GameOver(){
        yield return new WaitForSeconds(1f);
        if(countDown > 0){
            countDown--;
            //OnCountDownTicks?.Invoke(countDown);
            miniGameUIManager.DisplayCountDown(countDown);
            StartCoroutine(GameOver());
        }
        else {
            miniGameUIManager.InitializeVariables();
            gameState = MiniGameState.none;
            GameManager.instance.ReturnToMainHub();
        }
    }
}
