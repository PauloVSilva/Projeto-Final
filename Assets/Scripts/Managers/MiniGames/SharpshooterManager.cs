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
                    playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill += VerifyKillCountWinCondition;
                }
            }
            if(gameGoal == MiniGameGoal.lastStanding){
                lastStandingLives = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach (var playerInput in GameManager.instance.playerList){
                    playerInput.GetComponent<CharacterEvents>().OnPlayerDied += VerifyLastStandingWinCondition;
                    playerInput.GetComponent<CharacterEvents>().SetLimitedLives(lastStandingLives);
                    playersAlive.Add(playerInput);
                }
            }
        }
    }

    protected override void StartGame(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().UnblockActions();
        }
        GameStateAdvances();
    }

    private void VerifyLastStandingWinCondition(GameObject player){
        if(gameGoal == MiniGameGoal.lastStanding){
            if(!player.GetComponent<CharacterStats>().CanRespawn()){
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
            if (player.GetComponent<CharacterStats>().kills >= killCountGoal){
                GameStateAdvances();
                PlayerWins(player.GetComponent<PlayerInput>());
            }
        }
    }

    protected override void GameOverSetUp(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied -= VerifyLastStandingWinCondition;
        }
        GameStateAdvances();
        StartCoroutine(GameOver());
    }
}
