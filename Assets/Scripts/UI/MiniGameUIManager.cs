using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

public class MiniGameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject miniGameUIContainer;

    [SerializeField] private TextMeshProUGUI countDownMessage;
    [SerializeField] private TextMeshProUGUI countDownBoard;
    [SerializeField] private TextMeshProUGUI winnerBoard;
    [SerializeField] private TextMeshProUGUI gameGoalReminder;


    private string go;
    private string returningToLobbyInSeconds;
    private string gameBeginsInSeconds;


    //Just to be clear, events here mean the event actions baked in the language
    //These are NOT mini game events such as reaching X spot or stuff

    private void Start()
    {
        SubscribeToEvents();
        InitializeVariables();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        InitializeVariables();
    }


    private void AdaptToGameState(GameState gameState)
    {
        miniGameUIContainer.SetActive(gameState != GameState.MainMenu);
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnGameStateChanged += AdaptToGameState;
        MiniGameManager.OnGameStateAdvances += AdaptToMiniGameState;
        MiniGameManager.OnCountDownTick += DisplayCountDown;
        MiniGameManager.OnPlayerWins += AnnounceWinner;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.OnGameStateChanged -= AdaptToGameState;
        MiniGameManager.OnGameStateAdvances -= AdaptToMiniGameState;
        MiniGameManager.OnCountDownTick -= DisplayCountDown;
        MiniGameManager.OnPlayerWins -= AnnounceWinner;
    }

    private void InitializeVariables(){
        go = "Go!";
        returningToLobbyInSeconds = "Returning to lobby in ";
        gameBeginsInSeconds = "Game begins in ";

        //countDownMessage.text = "_COUNTDOWN_MESSAGE";
        //countDownBoard.text = "_COUNTDOWN_TIME";
        //winnerBoard.text = "_WINNER_MESSAGE";
        //gameGoalReminder.text = "_GOAL_REMINDER";

        countDownMessage.text = "";
        countDownBoard.text = "";
        winnerBoard.text = "";
        gameGoalReminder.text = "";

        //SetGameGoalText();
    }


    private void AdaptToMiniGameState(MiniGameState miniGameState)
    {
        switch (miniGameState)
        {
            case MiniGameState.gameIsRunning:
                SetGameGoalText();
                break;
        }

        countDownMessage.gameObject.SetActive(miniGameState == MiniGameState.preparation || miniGameState == MiniGameState.gameOver);
        countDownBoard.gameObject.SetActive(miniGameState == MiniGameState.preparation || miniGameState == MiniGameState.gameOver);
        gameGoalReminder.gameObject.SetActive(miniGameState == MiniGameState.gameIsRunning);
        winnerBoard.gameObject.SetActive(miniGameState == MiniGameState.gameOver);
    }

    private void DisplayCountDown(int seconds)
    {
        countDownBoard.transform.DOScale(Vector3.one, 0f);

        if(seconds > 0 && seconds < 6)
        {
            if (MiniGameManager.Instance.miniGameState == MiniGameState.preparation) countDownMessage.text = gameBeginsInSeconds;
            if (MiniGameManager.Instance.miniGameState == MiniGameState.gameOver) countDownMessage.text = returningToLobbyInSeconds;

            countDownBoard.text = seconds.ToString();

            StartCoroutine(DecreaseEffect());
            IEnumerator DecreaseEffect()
            {
                yield return new WaitForSeconds(0.5f);
                countDownBoard.transform.DOScale(Vector3.zero, 0.5f);
            }
        }

        if(seconds == 0)
        {
            if (MiniGameManager.Instance.miniGameState == MiniGameState.preparation) countDownBoard.text = go;
            if (MiniGameManager.Instance.miniGameState == MiniGameState.gameOver) countDownBoard.text = "Returning to lobby yay ^-^";

            StartCoroutine(CleanCountDownBoard());
            IEnumerator CleanCountDownBoard()
            {
                yield return new WaitForSeconds(1.5f);
                countDownMessage.transform.DOScale(Vector3.zero, 0.5f);
                countDownBoard.transform.DOScale(Vector3.zero, 0.5f);

                yield return new WaitForSeconds(0.5f);
                countDownMessage.text = "";
                countDownBoard.text = "";
                countDownMessage.transform.DOScale(Vector3.one, 0.0f);
                countDownBoard.transform.DOScale(Vector3.one, 0.0f);
            }
        }
    }

    private void SetGameGoalText()
    {
        gameGoalReminder.text = MiniGameManager.Instance.goalDescription;
    }

    private void AnnounceWinner(PlayerInput playerInput){
        winnerBoard.text = MessageManager.Instance.GetPlayerVictoryMessage(playerInput.playerIndex + 1);
    }
}