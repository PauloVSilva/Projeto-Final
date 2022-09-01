using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI pressToJoin;

    public PlayerController player;

    private void Start(){
        //UpdateScore(0);
        playerScore.text = null;
        playerName.text = null;
        pressToJoin.text = "Press X to Join";
    }

    public void AssignPlayer(int index){
        StartCoroutine(AssignPlayerDelay(index));
    }

    IEnumerator AssignPlayerDelay(int index){
        yield return new WaitForSeconds(0.1f);
        //print("Player assigned");
        player = GameManager.instance.playerList[index].GetComponent<PlayerInputHandler>().playerController;
        SetUpInfoPanel();
    }

    public void UnassignPlayer(){
        player = null;
        SetUpInfoPanel();
    }

    void SetUpInfoPanel(){
        if(player != null){
            player.OnScoreChanged += UpdateScore;

            playerScore.text = player.GetComponent<PlayerController>().score.ToString();
            playerName.text = player.thisPlayerColor.ToString();
            pressToJoin.text = null;
        }
        else{
            playerScore.text = null;
            playerName.text = null;
            pressToJoin.text = "Press X to Join";
        }
    }

    private void UpdateScore(int score){
        playerScore.text = score.ToString();
    }
}
