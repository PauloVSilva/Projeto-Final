using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DimeDropManager : MiniGameManager{
    [SerializeField] protected GameObject[] coins;
    public int scoreAmountGoal;
    public int timeLimitGoal;

    protected override void SetupGame(){
        if(miniGame == MiniGame.dimeDrop){
            if(gameGoal == MiniGameGoal.scoreAmount){
                scoreAmountGoal = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach(var playerInput in GameManager.Instance.playerList){
                    playerInput.GetComponent<CharacterManager>().OnPlayerScoreChanged += VerifyScoreAmountWinCondition;
                }
            }
        }
    }

    protected override void StartGame(){
        foreach(var spawner in itemSpawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = true;
        }
        foreach(var playerInput in GameManager.Instance.playerList){
            playerInput.GetComponent<CharacterManager>().UnblockActions();
        }
        GameStateAdvances();
    }

    private void VerifyScoreAmountWinCondition(GameObject player, int _score){
        if(gameGoal == MiniGameGoal.scoreAmount){
            if (_score >= scoreAmountGoal){
                GameStateAdvances();
                PlayerWins(player.GetComponent<PlayerInput>());
            }
        }
    }

    protected override void GameOverSetUp(){
        foreach(var playerInput in GameManager.Instance.playerList){
            playerInput.GetComponent<CharacterManager>().OnPlayerScoreChanged -= VerifyScoreAmountWinCondition;
        }

        foreach(var spawner in itemSpawnersList){
            spawner.GetComponent<Spawner>().spawnerEnabled = false;
        }
        //coins = GameObject.FindGameObjectsWithTag("Coin");
        //foreach(var coin in coins){
        //    coin.GetComponent<Coin>().canBePickedUp = false;
        //}
        GameStateAdvances();
        StartCoroutine(GameOver());
    }
}
