using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Threading;
using Unity.VisualScripting;

public enum MiniGame{none = -1, sharpShooter, dimeDrop, rocketRace}
public enum MiniGameGoal{none = -1, killCount, lastStanding, time, scoreAmount, race}
public enum MiniGameState{none, preparation, gameSetUp, gameIsRunning, gameOverSetUp, gameOver, returnToHub}

public class MiniGameManager : Singleton<MiniGameManager>
{
    [SerializeField] private GameObject victoryVFX;

    [field: Header("Mini Game properties")]
    [field: Space(5)]
    [field: SerializeField] public MiniGameGoalScriptableObject miniGameSetup { get; private set; }
    [field: SerializeField] public MiniGame miniGame { get; private set; }
    [field: SerializeField] public MiniGameGoal gameGoal { get; private set; }
    [field: SerializeField] public MiniGameState miniGameState { get; private set; }

    [field: SerializeField] public int GoalAmount { get; private set; }
    [field: SerializeField] public float TimeElapsed { get; private set; }
    [field: SerializeField] public string GoalDescription { get; private set; }
    [field: SerializeField] public int CountDown { get; private set; }

    //MINIGAME ACTION EVENTS
    public static event Action<int> OnCountDownTick;
    public static event Action<MiniGameState> OnGameStateAdvances;
    public static event Action<PlayerInput> OnPlayerWins;
    public static event Action OnMiniGameEnds;

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
            TimeElapsed += Time.deltaTime;
            CheckMiniGameEvents();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }


    public void InitializeVariables()
    {
        miniGameSetup = null;
        miniGame = MiniGame.none;
        gameGoal = MiniGameGoal.none;
        miniGameState = MiniGameState.none;

        GoalAmount = -1;
        TimeElapsed = 0;
        GoalDescription = string.Empty;
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
        GoalAmount = _amount;
    }

    public void SetGoalDescription(string _description)
    {
        GoalDescription = _description;
    }

    private void InitializeLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.MiniGame);
        GameManager.Instance.BlockAllPlayerActions(true);

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

            WeaponScriptableObject initialWeapon = (WeaponScriptableObject)ItemsDatabank.Instance.GetItem("blaster_c");

            foreach (PlayerInput playerInput in GameManager.Instance.playerList)
            {
                GameObject _weaponSO = Instantiate(initialWeapon.itemModel, playerInput.transform.position, playerInput.transform.rotation);

                //that's just so fucking cursed
                _weaponSO.GetComponent<Weapon>().canSpin = false;

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
            if (player.GetComponent<CharacterManager>().kills >= GoalAmount)
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
            if (_score >= GoalAmount)
            {
                UpdateMiniGameState(MiniGameState.gameOverSetUp);
                InvokeOnPlayerWins(player.GetComponent<PlayerInput>());
            }
        }
    }

    private void InitiateCountDown()
    {
        CountDown = 8;
        StartCoroutine(CountDownClock());
    }

    private IEnumerator CountDownClock()
    {
        yield return new WaitForSeconds(1f);

        if(CountDown == 0)
        {
            UpdateMiniGameState(++miniGameState);
        }
        else
        {
            CountDown--;
            OnCountDownTick?.Invoke(CountDown);
            StartCoroutine(CountDownClock());
        }
    }

    private void StartGame()
    {
        LevelManager.Instance.currentLevel.SetSpawnersEnabled(true);
        GameManager.Instance.BlockAllPlayerActions(false);

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

    public void ReturnToHub()
    {
        InitializeVariables();

        OnMiniGameEnds?.Invoke();

        LevelLoader.Instance.LoadLevel("MainHub");
    }

    public void QuitToMenu()
    {
        InitializeVariables();

        OnMiniGameEnds?.Invoke();

        LevelLoader.Instance.LoadLevel("MainMenu");
    }

    private void InvokeOnPlayerWins(PlayerInput playerInput)
    {
        OnPlayerWins?.Invoke(playerInput);

        StartCoroutine(VictoryVFX(6)); 
        IEnumerator VictoryVFX(int times)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject _victoryVFX = Instantiate(victoryVFX, playerInput.transform.position, playerInput.transform.rotation);
            Destroy(_victoryVFX, 2f);

            if(times > 0)
            {
                StartCoroutine(VictoryVFX(--times));
            }
        }
    }


}
