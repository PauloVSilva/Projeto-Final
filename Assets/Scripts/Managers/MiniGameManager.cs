using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Threading;

public enum MiniGame{sharpShooter, dimeDrop, rocketRace}
public enum MiniGameGoal{killCount, lastStanding, time, scoreAmount, race}
public enum MiniGameState{none, preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver, returnToHub}

public abstract class MiniGameManager : Singleton<MiniGameManager>
{
    //MINIGAME VARIABLES
    [SerializeField] public MiniGameGoalScriptableObject miniGameSetup;
    [SerializeField] public MiniGame miniGame;
    [SerializeField] public MiniGameGoal gameGoal;
    [SerializeField] public MiniGameState miniGameState;

    [SerializeField] public float timeElapsed;
    [SerializeField] protected int countDown;

    //MINIGAME ACTION EVENTS
    public static event Action<int> OnCountDownTick;
    public static event Action<MiniGameState> OnGameStateAdvances;
    public static event Action<PlayerInput> OnPlayerWins;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitializeLevel();
    }

    private void Update()
    {
        if (miniGameState == MiniGameState.gameIsRunning)
        {
            timeElapsed += Time.deltaTime;
            CheckMiniGameEvents();
        }
    }


    protected void InitializeLevel()
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
        LevelManager.Instance.currentLevel.SetSpawnersEnabled(true);

        foreach (var playerInput in GameManager.Instance.playerList)
        {
            playerInput.GetComponent<CharacterManager>().UnblockActions();
        }

        UpdateMiniGameState(MiniGameState.gameIsRunning);
    }

    protected virtual void GameOverSetUp()
    {
        LevelManager.Instance.currentLevel.SetSpawnersEnabled(false);
    }

    private void ReturnToHub()
    {
        LevelLoader.Instance.LoadLevel("MainHub");
    }

    protected void InvokeOnPlayerWins(PlayerInput playerInput)
    {
        OnPlayerWins?.Invoke(playerInput);
    }
}
