using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Threading;

public enum MiniGame{sharpShooter, dimeDrop, rocketRace}
public enum MiniGameGoal{killCount, lastStanding, time, scoreAmount, race}
public enum MiniGameState{none, preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver, returnToHub}

public abstract class MiniGameManager : LevelManager{
    [SerializeField] public static MiniGameManager instance;

    //MINIGAME VARIABLES
    [SerializeField] public MiniGameGoalScriptableObject miniGameSetup;
    [SerializeField] public MiniGame miniGame;
    [SerializeField] public MiniGameGoal gameGoal;
    [SerializeField] public MiniGameState miniGameState;


    [SerializeField] protected GameObject[] itemSpawnersList;
    [SerializeField] public float timeElapsed;
    [SerializeField] protected int countDown;

    //MINIGAME ACTION EVENTS
    public event System.Action<int> OnCountDownTick;
    public event System.Action<MiniGameState> OnGameStateAdvances;
    public event System.Action<PlayerInput> OnPlayerWins;


    protected void Update()
    {
        if (miniGameState == MiniGameState.gameIsRunning)
        {
            timeElapsed += Time.deltaTime;
            CheckMiniGameEvents();
        }
    }

    protected override void InitializeSingletonInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    protected override void InitializeLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.MiniGame);

        foreach (var playerInput in GameManager.Instance.playerList)
        {
            playerInput.GetComponent<CharacterManager>().BlockActions();
        }

        miniGameSetup = MiniGameOptionsMenu.instance.GetMiniGameGoal();

        miniGame = miniGameSetup.parentMiniGame.minigame;
        gameGoal = miniGameSetup.miniGameGoal;

        MiniGameSpecificSetup();

        UpdateMiniGameState(MiniGameState.preparation);
    }

    protected void UpdateMiniGameState(MiniGameState _miniGameState)
    {
        miniGameState = _miniGameState;

        OnGameStateAdvances?.Invoke(miniGameState);

        switch (miniGameState)
        {
            case MiniGameState.preparation:
                InitiateCountDown();
                break;
            case MiniGameState.gameSetUp:
                StartGame();
                break;
            case MiniGameState.gameOverSetUp:
                GameOverSetUp();
                break;
            case MiniGameState.gameOver:
                InitiateCountDown();
                break;
            case MiniGameState.returnToHub:
                ReturnToHub();
                break;
        }
    }

    protected virtual void CheckMiniGameEvents(){}


    protected abstract void MiniGameSpecificSetup();

    private void InitiateCountDown()
    {
        countDown = 8;
        StartCoroutine(CountDownClock());
    }

    private IEnumerator CountDownClock()
    {
        yield return new WaitForSeconds(1f);

        if(countDown == 0)
        {
            miniGameState++;
            UpdateMiniGameState(miniGameState);
        }
        else
        {
            countDown--;
            OnCountDownTick?.Invoke(countDown);
            StartCoroutine(CountDownClock());
        }
    }

    protected void StartGame()
    {
        if(itemSpawnersList.Length > 0)
        {
            foreach (var spawner in itemSpawnersList)
            {
                spawner.GetComponent<Spawner>().spawnerEnabled = true;
            }
        }

        foreach (var playerInput in GameManager.Instance.playerList)
        {
            playerInput.GetComponent<CharacterManager>().UnblockActions();
        }

        UpdateMiniGameState(MiniGameState.gameIsRunning);
    }

    protected virtual void GameOverSetUp()
    {
        if (itemSpawnersList.Length > 0)
        {
            foreach (var spawner in itemSpawnersList)
            {
                spawner.GetComponent<Spawner>().spawnerEnabled = false;
            }
        }
    }

    private void ReturnToHub()
    {
        LevelLoader.instance.LoadLevel("MainHub");
    }

    protected void InvokeOnPlayerWins(PlayerInput playerInput)
    {
        OnPlayerWins?.Invoke(playerInput);
    }
}
