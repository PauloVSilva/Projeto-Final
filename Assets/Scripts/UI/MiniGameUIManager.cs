using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

public class MiniGameUIManager : MonoBehaviour{

    [SerializeField] private TextMeshProUGUI countDownMessage;
    [SerializeField] private TextMeshProUGUI countDownBoard;
    [SerializeField] private TextMeshProUGUI winnerBoard;
    [SerializeField] private TextMeshProUGUI gameGoalReminder;

    private bool isSubscribed;


    private string go;
    private string returningToLobbyInSeconds;
    private string gameBeginsInSeconds;


    //Just to be clear, events here mean the event actions baked in the language
    //These are NOT mini game events such as reaching X spot or stuff

    private void Start(){
        LevelLoader.instance.OnSceneLoaded += CheckForMiniGame;
        isSubscribed = false;
    }

    private void OnDestroy(){
        UnsubscribeFromMiniGameEvents();
    }

    private void CheckForMiniGame(){
        if(MiniGameManager.instance != null){
            SubscribeToMiniGameEvents();
            InitializeVariables();
        }
    }

    private void SubscribeToMiniGameEvents(){
        MiniGameManager.instance.OnGameStateAdvances += AdaptToMiniGameState;

        MiniGameManager.instance.OnCountDownTick += DisplayCountDown;
        MiniGameManager.instance.OnPlayerWins += AnnounceWinner;

        isSubscribed = true;
    }

    private void UnsubscribeFromMiniGameEvents(){
        if (!isSubscribed) return;

        MiniGameManager.instance.OnGameStateAdvances -= AdaptToMiniGameState;

        MiniGameManager.instance.OnCountDownTick -= DisplayCountDown;
        MiniGameManager.instance.OnPlayerWins -= AnnounceWinner;
        
        isSubscribed = false;
    }

    public void InitializeVariables(){
        go = "Go!";
        returningToLobbyInSeconds = "Returning to lobby in ";
        gameBeginsInSeconds = "Game begins in ";

        countDownMessage.text = "_COUNTDOWN_MESSAGE";
        countDownBoard.text = "_COUNTDOWN_TIME";
        winnerBoard.text = "_WINNER_MESSAGE";
        //gameGoalReminder.text = "_GOAL_REMINDER";
        SetGameGoalText();
    }


    private void AdaptToMiniGameState(MiniGameState miniGameState)
    {
        countDownMessage.gameObject.SetActive(miniGameState == MiniGameState.preparation);
        countDownBoard.gameObject.SetActive(miniGameState == MiniGameState.preparation);

        gameGoalReminder.gameObject.SetActive(miniGameState == MiniGameState.gameIsRunning);

        winnerBoard.gameObject.SetActive(miniGameState == MiniGameState.gameOver);
    }

    private void DisplayCountDown(int seconds)
    {
        if(seconds > 0 && seconds < 6)
        {
            countDownMessage.gameObject.SetActive(true);
            countDownBoard.gameObject.SetActive(true);

            if (MiniGameManager.instance.miniGameState == MiniGameState.preparation) countDownMessage.text = gameBeginsInSeconds;
            if (MiniGameManager.instance.miniGameState == MiniGameState.gameOver) countDownMessage.text = returningToLobbyInSeconds;

            countDownBoard.text = seconds.ToString();
        }

        if(seconds == 0)
        {
            if(MiniGameManager.instance.miniGameState == MiniGameState.preparation) countDownBoard.text = go;
            StartCoroutine(CleanCountDownBoard());
            IEnumerator CleanCountDownBoard()
            {
                yield return new WaitForSeconds(0.5f);
                countDownMessage.transform.DOScale(Vector3.zero, 0.5f);
                countDownBoard.transform.DOScale(Vector3.zero, 0.5f);

                yield return new WaitForSeconds(0.5f);
                countDownMessage.gameObject.SetActive(false);
                countDownBoard.gameObject.SetActive(false);
                countDownMessage.transform.DOScale(Vector3.one, 0.0f);
                countDownBoard.transform.DOScale(Vector3.one, 0.0f);
            }
        }
    }

    private void SetGameGoalText(){
        gameGoalReminder.text = MiniGameOptionsMenu.instance.GetMiniGameGoalDescription();
    }

    private void AnnounceWinner(PlayerInput playerInput){
        winnerBoard.text = MessageManager.instance.GetPlayerVictoryMessage(playerInput.playerIndex + 1);
    }
}