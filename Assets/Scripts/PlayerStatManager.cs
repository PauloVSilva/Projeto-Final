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

    

    public void FilterCollision(GameObject player, GameObject gameObject){
        if(gameObject.CompareTag("Coin")){
            if(gameObject.GetComponent<Coin>().canBePickedUp){
                IncreaseScore(gameObject.GetComponent<Coin>().value);
                Destroy(gameObject);
            }
        }
    }

    public void IncreaseScore(int value){
        score += value;
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScores(){
        score = 0;
        kills = 0;
        deaths = 0;
    }
}
