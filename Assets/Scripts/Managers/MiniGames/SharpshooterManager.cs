using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SharpshooterManager : MiniGameManager{
    public static SharpshooterManager instance = null;
    [SerializeField] private List<PlayerInput> playersAlive = new List<PlayerInput>();
    public int killCountGoal;
    public int lastStandingLives;

    protected override void InitializeSingletonInstance(){
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

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
        gameState++;
        //OnGameStateAdvances?.Invoke();
    }

    private void VerifyLastStandingWinCondition(GameObject player){
        if(gameGoal == MiniGameGoal.lastStanding){
            if(!player.GetComponent<CharacterStats>().CanRespawn()){
                playersAlive.Remove(player.GetComponent<PlayerInput>());
            }
            if (playersAlive.Count == 1){
                gameState++;
                //OnGameStateAdvances?.Invoke();
                //OnPlayerWins?.Invoke(playersAlive[0]);
                miniGameUIManager.AnnounceWinner(playersAlive[0]);
                GameOverSetUp();
            }
        }
    }

    private void VerifyKillCountWinCondition(GameObject player){
        if(gameGoal == MiniGameGoal.killCount){
            if (player.GetComponent<CharacterStats>().kills >= killCountGoal){
                gameState++;
                //OnGameStateAdvances?.Invoke();
                //OnPlayerWins?.Invoke(player.transform.parent.GetComponent<PlayerInput>());
                miniGameUIManager.AnnounceWinner(player.GetComponent<PlayerInput>());
                GameOverSetUp();
            }
        }
    }

    protected override void GameOverSetUp(){
        foreach(var playerInput in GameManager.instance.playerList){
            playerInput.GetComponent<CharacterEvents>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterEvents>().OnPlayerDied -= VerifyLastStandingWinCondition;
        }

        countDown = 10;
        gameState++;
        //OnGameStateAdvances?.Invoke();
        StartCoroutine(GameOver());
    }
}
