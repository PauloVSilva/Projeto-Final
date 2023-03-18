using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DimeDropManager : MiniGameManager{
    [SerializeField] protected GameObject[] coins;
    public int scoreAmountGoal;
    public int timeLimitGoal;

    protected override void MiniGameSpecificSetup()
    {
        if(miniGame == MiniGame.dimeDrop)
        {
            if(gameGoal == MiniGameGoal.scoreAmount)
            {
                scoreAmountGoal = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach(var playerInput in GameManager.Instance.playerList)
                {
                    playerInput.GetComponent<CharacterManager>().OnPlayerScoreChanged += VerifyScoreAmountWinCondition;
                }
            }
        }
    }

    private void VerifyScoreAmountWinCondition(GameObject player, int _score)
    {
        if(gameGoal == MiniGameGoal.scoreAmount)
        {
            if (_score >= scoreAmountGoal)
            {
                UpdateMiniGameState(MiniGameState.gameOverSetUp);
                InvokeOnPlayerWins(player.GetComponent<PlayerInput>());
            }
        }
    }

    protected override void GameOverSetUp()
    {
        base.GameOverSetUp();

        foreach(var playerInput in GameManager.Instance.playerList)
        {
            playerInput.GetComponent<CharacterManager>().OnPlayerScoreChanged -= VerifyScoreAmountWinCondition;
        }

        //coins = GameObject.FindGameObjectsWithTag("Coin");
        //foreach(var coin in coins){
        //    coin.GetComponent<Coin>().canBePickedUp = false;
        //}

        UpdateMiniGameState(MiniGameState.gameOver);
    }
}
