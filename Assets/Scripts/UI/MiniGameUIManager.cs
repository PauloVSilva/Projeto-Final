using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

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
        MiniGameManager.instance.OnCountDownTicks += DisplayCountDown;
        MiniGameManager.instance.OnGameGoalIsSet += SetGameGoalText;
        MiniGameManager.instance.OnPlayerWins += AnnounceWinner;
        MiniGameManager.instance.OnGameEnds += DisablePanels;

        isSubscribed = true;
    }

    private void UnsubscribeFromMiniGameEvents(){
        if(isSubscribed){
            MiniGameManager.instance.OnCountDownTicks -= DisplayCountDown;
            MiniGameManager.instance.OnGameGoalIsSet -= SetGameGoalText;
            MiniGameManager.instance.OnPlayerWins -= AnnounceWinner;
            MiniGameManager.instance.OnGameEnds -= DisablePanels;

            isSubscribed = false;
        }
    }

    public void InitializeVariables(){
        go = "Go!";
        returningToLobbyInSeconds = "Returning to lobby in ";
        gameBeginsInSeconds = "Game begins in ";

        countDownMessage.text = "_COUNTDOWN_MESSAGE";
        countDownBoard.text = "_COUNTDOWN_TIME";
        winnerBoard.text = "_WINNER_MESSAGE";
        gameGoalReminder.text = "_GOAL_REMINDER";
    }

    private void DisablePanels(){
        countDownMessage.gameObject.SetActive(false);
        countDownBoard.gameObject.SetActive(false);
        winnerBoard.gameObject.SetActive(false);
        gameGoalReminder.gameObject.SetActive(false);
    }

    public void DisplayCountDown(int seconds){
        if(seconds > 0 && seconds < 6){
            if(MiniGameManager.instance.gameState == MiniGameState.preparation){
                countDownMessage.gameObject.SetActive(true);
                countDownMessage.text = gameBeginsInSeconds;
            }
            if(MiniGameManager.instance.gameState == MiniGameState.gameOver){
                countDownMessage.gameObject.SetActive(true);
                countDownMessage.text = returningToLobbyInSeconds;
            }
            countDownBoard.gameObject.SetActive(true);
            countDownBoard.text = seconds.ToString();
        }
        if(seconds == 0){
            if(MiniGameManager.instance.gameState == MiniGameState.preparation){
                countDownBoard.text = go;
            }
            StartCoroutine(CleanCountDownBoard());
        }
    }

    IEnumerator CleanCountDownBoard(){
        yield return new WaitForSeconds(1f);
        countDownMessage.gameObject.SetActive(false);
        countDownBoard.gameObject.SetActive(false);
    }

    public void SetGameGoalText(string text){
        gameGoalReminder.gameObject.SetActive(true);
        gameGoalReminder.text = text;
    }

    public void AnnounceWinner(PlayerInput playerInput){
        winnerBoard.gameObject.SetActive(true);
        winnerBoard.text = MessageManager.instance.GetPlayerVictoryMessage(playerInput.playerIndex + 1);
    }
}