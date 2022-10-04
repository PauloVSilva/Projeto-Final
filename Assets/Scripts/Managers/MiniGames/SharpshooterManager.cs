using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SharpshooterManager : MiniGameManager{
    [SerializeField] private List<PlayerInput> playersAlive = new List<PlayerInput>();
    public int killCountGoal;
    public int lastStandingLives;

    protected override void SetupGame(){
        if(miniGame == MiniGame.sharpShooter){
            if(gameGoal == MiniGameGoal.killCount){
                killCountGoal = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach (var playerInput in GameManager.instance.playerList){
                    playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill += VerifyKillCountWinCondition;
                }
            }
            if(gameGoal == MiniGameGoal.lastStanding){
                lastStandingLives = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach (var playerInput in GameManager.instance.playerList){
                    playerInput.GetComponent<CharacterManager>().OnPlayerDied += VerifyLastStandingWinCondition;
                    playerInput.GetComponent<CharacterManager>().SetLimitedLives(lastStandingLives);
                    playersAlive.Add(playerInput);
                }
            }
            foreach(var playerInput in GameManager.instance.playerList){
                playerInput.GetComponent<CharacterManager>().IncreaseScore(250);
            }
        }
    }

    protected override void StartGame(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterManager>().UnblockActions();
        }
        StartCoroutine(GiveGoldToPlayerDelay());
        GameStateAdvances();
    }

    IEnumerator GiveGoldToPlayerDelay(){
        yield return new WaitForSeconds(2f);
        GiveGoldToPlayers();
        if(gameState == MiniGameState.gameIsRunning){
            StartCoroutine(GiveGoldToPlayerDelay());
        }
    }

    protected void GiveGoldToPlayers(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterManager>().IncreaseScore(10);
        }
    }

    private void VerifyLastStandingWinCondition(GameObject player){
        if(gameGoal == MiniGameGoal.lastStanding){
            if(!player.GetComponent<CharacterManager>().CanRespawn()){
                playersAlive.Remove(player.GetComponent<PlayerInput>());
            }
            if (playersAlive.Count == 1){
                GameStateAdvances();
                PlayerWins(playersAlive[0]);
            }
        }
    }

    private void VerifyKillCountWinCondition(GameObject player){
        if(gameGoal == MiniGameGoal.killCount){
            if (player.GetComponent<CharacterManager>().kills >= killCountGoal){
                GameStateAdvances();
                PlayerWins(player.GetComponent<PlayerInput>());
            }
        }
    }

    protected override void GameOverSetUp(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterManager>().OnPlayerDied -= VerifyLastStandingWinCondition;
        }
        GameStateAdvances();
        StartCoroutine(GameOver());
    }
}
