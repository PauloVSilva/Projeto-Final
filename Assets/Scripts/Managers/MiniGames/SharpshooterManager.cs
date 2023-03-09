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
                foreach (var playerInput in GameManager.Instance.playerList){
                    playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill += VerifyKillCountWinCondition;
                }
            }
            if(gameGoal == MiniGameGoal.lastStanding){
                lastStandingLives = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();
                foreach (var playerInput in GameManager.Instance.playerList){
                    playerInput.GetComponent<CharacterManager>().OnPlayerDied += VerifyLastStandingWinCondition;
                    playerInput.GetComponent<CharacterManager>().SetLimitedLives(lastStandingLives);
                    playersAlive.Add(playerInput);
                }
            }
            foreach(var playerInput in GameManager.Instance.playerList){
                CoinScriptableObject itemToAdd = (CoinScriptableObject)GameManager.Instance.itemsDatabank.GetItem("gold_coin");
                for(int i = 0; i < 3; i++){
                    if(playerInput.GetComponent<CharacterManager>().characterInventory.AddToInventory(itemToAdd)){
                        playerInput.GetComponent<CharacterManager>().IncreaseScore(itemToAdd.value);
                    }
                }
            }
        }
    }

    protected override void StartGame(){
        foreach(var playerInput in GameManager.Instance.playerList){
            playerInput.GetComponent<CharacterManager>().UnblockActions();
        }
        StartCoroutine(GiveCoinToPlayersDelay());
        GameStateAdvances();
    }

    IEnumerator GiveCoinToPlayersDelay(){
        yield return new WaitForSeconds(2f);
        GiveCoinToPlayers();
        if(gameState == MiniGameState.gameIsRunning){
            StartCoroutine(GiveCoinToPlayersDelay());
        }
    }

    protected void GiveCoinToPlayers(){
        foreach(var playerInput in GameManager.Instance.playerList){
            CoinScriptableObject itemToAdd = (CoinScriptableObject)GameManager.Instance.itemsDatabank.GetItem("silver_coin");
            if(playerInput.GetComponent<CharacterManager>().characterInventory.AddToInventory(itemToAdd)){
                playerInput.GetComponent<CharacterManager>().IncreaseScore(itemToAdd.value);
            }
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
        foreach(var playerInput in GameManager.Instance.playerList){
            playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
            playerInput.GetComponent<CharacterManager>().OnPlayerDied -= VerifyLastStandingWinCondition;
        }
        GameStateAdvances();
        StartCoroutine(GameOver());
    }
}
