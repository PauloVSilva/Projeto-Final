using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MiniGameUIManager : MonoBehaviour{

    [SerializeField] private TextMeshProUGUI countDownBoard;
    [SerializeField] private TextMeshProUGUI winnerBoard;
    [SerializeField] private TextMeshProUGUI gameGoalReminder;

    //Just to be clear, events here mean the event actions baked in the language
    //These are NOT mini game events such as reaching X spot or stuff

    private void Start(){
        GameManager.instance.levelLoader.OnSceneLoaded += CheckForMiniGame;
    }

    private void CheckForMiniGame(){
        if(MiniGameManager.instance != null){
            SubscribeToMiniGameEvents();
            InitializeVariables();
        }
        else{
            UnsubscribeFromMiniGameEvents();
            InitializeVariables();
        }
    }

    public void SubscribeToMiniGameEvents(){
        MiniGameManager.instance.OnCountDownTicks += DisplayCountDown;
        MiniGameManager.instance.OnGameGoalIsSet += SetGameGoalText;
        MiniGameManager.instance.OnPlayerWins += AnnounceWinner;
    }

    public void UnsubscribeFromMiniGameEvents(){
        MiniGameManager.instance.OnCountDownTicks -= DisplayCountDown;
        MiniGameManager.instance.OnGameGoalIsSet -= SetGameGoalText;
        MiniGameManager.instance.OnPlayerWins -= AnnounceWinner;
    }

    private string GO_SCREEN;
    private string GAME_GOAL_REMINDER;

    private void Awake(){
        InitializeVariables();
    }

    public void InitializeVariables(){
        GO_SCREEN = "Go!";
        countDownBoard.text = null;
        winnerBoard.text = null;
        gameGoalReminder.text = null;
    }

    private void OnDisable(){
        InitializeVariables();
    }

    public void DisplayCountDown(int seconds){
        if(seconds > 0 && seconds < 6){
            countDownBoard.text = seconds.ToString();
        }
        if(seconds == 0){
            if(MiniGameManager.instance.gameState == MiniGameState.preparation){
                countDownBoard.text = GO_SCREEN;
            }
            StartCoroutine(CleanCountDownBoard());
        }
    }

    IEnumerator CleanCountDownBoard(){
        yield return new WaitForSeconds(1f);
        countDownBoard.text = null;
    }

    public void SetGameGoalText(string text){
        gameGoalReminder.text = text; 
    }

    public void AnnounceWinner(PlayerInput playerInput){
        winnerBoard.text = MessageManager.instance.GetPlayerVictoryMessage(playerInput.playerIndex + 1);
    }
}