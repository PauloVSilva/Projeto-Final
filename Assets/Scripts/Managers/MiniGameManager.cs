using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Threading;

public enum MiniGame{none = -1, sharpShooter, dimeDrop, rocketRace}
public enum MiniGameGoal{none = -1, killCount, lastStanding, time, scoreAmount, race}
public enum MiniGameState{none, preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver, returnToHub}

public class MiniGameManager : Singleton<MiniGameManager>
{
    //MINIGAME VARIABLES
    [SerializeField] public MiniGameGoalScriptableObject miniGameSetup;
    [SerializeField] public MiniGame miniGame;
    [SerializeField] public MiniGameGoal gameGoal;
    [SerializeField] public MiniGameState miniGameState;

    [SerializeField] private int goalAmount;
    [SerializeField] public float timeElapsed;
    [SerializeField] public string goalDescription;
    [SerializeField] private int countDown;

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
        InitializeVariables();
        SubscribeToEvents();
    }

    private void Update()
    {
        if (miniGameState == MiniGameState.gameIsRunning)
        {
            timeElapsed += Time.deltaTime;
            CheckMiniGameEvents();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }


    private void InitializeVariables()
    {
        miniGameSetup = null;
        miniGame = MiniGame.none;
        gameGoal = MiniGameGoal.none;
        miniGameState = MiniGameState.none;

        goalAmount = -1;
        timeElapsed = 0;
        goalDescription = string.Empty;
    }

    private void SubscribeToEvents()
    {
        LevelLoader.Instance.OnSceneLoaded += CheckForMiniGame;
    }

    private void UnsubscribeFromEvents()
    {
        LevelLoader.Instance.OnSceneLoaded += CheckForMiniGame;
    }


    private void CheckForMiniGame()
    {
        if(miniGameSetup != null)
        {
            InitializeLevel();
        }
    }


    public void SetMiniGame(MiniGameGoalScriptableObject _miniGameSetup)
    {
        miniGameSetup = _miniGameSetup;

        miniGame = miniGameSetup.parentMiniGame.minigame;
        gameGoal = miniGameSetup.miniGameGoal;
    }

    public void SetGoalAmount(int _amount)
    {
        goalAmount = _amount;
    }

    public void SetGoalDescription(string _description)
    {
        goalDescription = _description;
    }

    private void InitializeLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.MiniGame);

        foreach (var playerInput in GameManager.Instance.playerList)
        {
            playerInput.GetComponent<CharacterManager>().BlockActions();
        }

        MiniGameSpecificSetup();

        UpdateMiniGameState(MiniGameState.preparation);
    }

    private void UpdateMiniGameState(MiniGameState _miniGameState)
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

    private void CheckMiniGameEvents()
    {

    }

    private void MiniGameSpecificSetup()
    {
        if(miniGame == MiniGame.sharpShooter)
        {
            if (gameGoal == MiniGameGoal.killCount)
            {
                foreach (var playerInput in GameManager.Instance.playerList)
                {
                    playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill += VerifyKillCountWinCondition;
                }
            }

            WeaponScriptableObject initialWeapon = (WeaponScriptableObject)ItemsDatabank.Instance.GetItem("double_action_revolver");
            foreach (PlayerInput playerInput in GameManager.Instance.playerList)
            {
                GameObject _weaponSO = Instantiate(initialWeapon.itemModel, playerInput.transform.position, playerInput.transform.rotation);

                playerInput.transform.TryGetComponent(out CharacterManager characterManager);

                characterManager.characterInventory.PickWeapon(_weaponSO);
            }
        }
        if(miniGame == MiniGame.dimeDrop)
        {
            if (gameGoal == MiniGameGoal.scoreAmount)
            {
                foreach (var playerInput in GameManager.Instance.playerList)
                {
                    playerInput.GetComponent<CharacterManager>().OnPlayerScoreChanged += VerifyScoreAmountWinCondition;
                }
            }
        }
    }

    private void VerifyKillCountWinCondition(GameObject player)
    {
        if (gameGoal == MiniGameGoal.killCount)
        {
            if (player.GetComponent<CharacterManager>().kills >= goalAmount)
            {
                UpdateMiniGameState(MiniGameState.gameOverSetUp);
                InvokeOnPlayerWins(player.GetComponent<PlayerInput>());
            }
        }
    }

    private void VerifyScoreAmountWinCondition(GameObject player, int _score)
    {
        if (gameGoal == MiniGameGoal.scoreAmount)
        {
            if (_score >= goalAmount)
            {
                UpdateMiniGameState(MiniGameState.gameOverSetUp);
                InvokeOnPlayerWins(player.GetComponent<PlayerInput>());
            }
        }
    }

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
            UpdateMiniGameState(++miniGameState);
        }
        else
        {
            countDown--;
            OnCountDownTick?.Invoke(countDown);
            StartCoroutine(CountDownClock());
        }
    }

    private void StartGame()
    {
        LevelManager.Instance.currentLevel.SetSpawnersEnabled(true);

        foreach (var playerInput in GameManager.Instance.playerList)
        {
            playerInput.GetComponent<CharacterManager>().UnblockActions();
        }

        UpdateMiniGameState(MiniGameState.gameIsRunning);
    }

    private void GameOverSetUp()
    {
        LevelManager.Instance.currentLevel.SetSpawnersEnabled(false);

        if(miniGame == MiniGame.sharpShooter)
        {
            foreach (var playerInput in GameManager.Instance.playerList)
            {
                playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            }
        }
        if(miniGame == MiniGame.dimeDrop)
        {
            foreach (var playerInput in GameManager.Instance.playerList)
            {
                playerInput.GetComponent<CharacterManager>().OnPlayerScoreChanged -= VerifyScoreAmountWinCondition;
            }
        }

        UpdateMiniGameState(MiniGameState.gameOver);
    }

    private void ReturnToHub()
    {
        InitializeVariables();

        LevelLoader.Instance.LoadLevel("MainHub");
    }

    private void InvokeOnPlayerWins(PlayerInput playerInput)
    {
        OnPlayerWins?.Invoke(playerInput);
    }
}
