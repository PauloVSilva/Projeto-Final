using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SharpshooterManager : MiniGameManager
{
    public int killCountGoal;

    protected override void MiniGameSpecificSetup()
    {
        //Called as soon as the level starts
        if (miniGame != MiniGame.sharpShooter) return;

        if(gameGoal == MiniGameGoal.killCount)
        {
            killCountGoal = MiniGameOptionsMenu.instance.GetMiniGameGoalAmount();

            foreach (var playerInput in GameManager.Instance.playerList)
            {
                playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill += VerifyKillCountWinCondition;
            }
        }
        
        WeaponScriptableObject initialWeapon = (WeaponScriptableObject)ItemsDatabank.Instance.GetItem("double_action_revolver");
        foreach(PlayerInput playerInput in GameManager.Instance.playerList)
        {
            GameObject _weaponSO = Instantiate(initialWeapon.itemModel, playerInput.transform.position, playerInput.transform.rotation);
        
            playerInput.transform.TryGetComponent(out CharacterManager characterManager);
        
            characterManager.characterInventory.PickWeapon(_weaponSO);
        }
    }

    private void VerifyKillCountWinCondition(GameObject player)
    {
        if(gameGoal == MiniGameGoal.killCount)
        {
            if (player.GetComponent<CharacterManager>().kills >= killCountGoal)
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
            playerInput.GetComponent<CharacterManager>().OnPlayerScoredKill -= VerifyKillCountWinCondition;
        }

        UpdateMiniGameState(MiniGameState.gameOver);
    }
}
