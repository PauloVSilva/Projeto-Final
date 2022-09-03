using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStatManager : MonoBehaviour{
    public enum playerColor{blue, red, green, yellow}
    public playerColor thisPlayerColor = playerColor.blue;

    public int score = 0;
    public int kills = 0;
    public int deaths = 0;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnKillsChanged;
    public event Action<int> OnDeathsChanged;

    public void FilterCollision(GameObject player, GameObject gameObject){
        if(gameObject.CompareTag("Coin")){
            if(gameObject.GetComponent<Coin>().canBePickedUp){
                IncreaseScore(gameObject.GetComponent<Coin>().value);
                Destroy(gameObject);
            }
        }
        if(gameObject.CompareTag("Instadeath")){
            player.GetComponent<HealthSystem>().Kill();
        }
    }

    public void IncreaseScore(int value){
        score += value;
        OnScoreChanged?.Invoke(score);
    }

    public void IncreaseKillCount(){
        kills++;
        OnKillsChanged?.Invoke(kills);
    }

    public void IncreaseDeathCount(){
        deaths++;
        OnDeathsChanged?.Invoke(deaths);
    }

    public void ResetScores(){
        score = 0;
        kills = 0;
        deaths = 0;
    }
}
