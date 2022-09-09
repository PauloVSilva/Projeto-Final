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

    private string GO_SCREEN;
    private string PLAYER;
    private string WINNER_ANNOUNCEMENT;
    private string GAME_GOAL_REMINDER;

    private void Awake(){
        InitializeVariables();
    }

    public void InitializeVariables(){
        GO_SCREEN = "Go!";
        PLAYER = "Player ";
        WINNER_ANNOUNCEMENT = " wins!";
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
            countDownBoard.text = GO_SCREEN;
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
        winnerBoard.text = PLAYER + (playerInput.playerIndex + 1).ToString() + WINNER_ANNOUNCEMENT;
    }
}