using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI playerKillCount;
    public TextMeshProUGUI playerDeathCount;

    public TextMeshProUGUI pressToJoin;

    //public PlayerController player;
    public PlayerInput player;

    private void Start(){
        //UpdateScore(0);
        playerName.text = null;
        playerScore.text = null;
        playerKillCount.text = null;
        playerDeathCount.text = null;

        pressToJoin.text = "Press X to Join";
    }

    public void AssignPlayer(int index){
        StartCoroutine(AssignPlayerDelay(index));
    }

    IEnumerator AssignPlayerDelay(int index){
        yield return new WaitForSeconds(0.1f);
        //print("Player assigned");
        //player = GameManager.instance.playerList[index].GetComponent<PlayerInputHandler>().playerController;
        player = GameManager.instance.playerList[index];
        SetUpInfoPanel();
    }

    public void UnassignPlayer(){
        player = null;
        SetUpInfoPanel();
    }

    void SetUpInfoPanel(){
        if(player != null){
            player.transform.GetComponent<PlayerStatManager>().OnScoreChanged += UpdateScore;
            player.transform.GetComponent<PlayerStatManager>().OnKillsChanged += UpdateKillCount;
            player.transform.GetComponent<PlayerStatManager>().OnDeathsChanged += UpdateDeathCount;

            playerName.text = player.transform.GetComponent<PlayerStatManager>().thisPlayerColor.ToString();
            playerScore.text = player.transform.GetComponent<PlayerStatManager>().score.ToString();
            playerKillCount.text = player.transform.GetComponent<PlayerStatManager>().kills.ToString();
            playerDeathCount.text = player.transform.GetComponent<PlayerStatManager>().deaths.ToString();
            
            pressToJoin.text = null;
        }
        else{
            playerName.text = null;
            playerScore.text = null;
            playerKillCount.text = null;
            playerDeathCount.text = null;

            pressToJoin.text = "Press X to Join";
        }
    }

    private void UpdateScore(int score){
        playerScore.text = score.ToString();
    }

    private void UpdateKillCount(int killCount){
        playerKillCount.text = killCount.ToString();
    }

    private void UpdateDeathCount(int deathCount){
        playerDeathCount.text = deathCount.ToString();
    }
}
