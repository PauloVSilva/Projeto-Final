using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI pressToJoin;

    //public PlayerController player;
    public PlayerInput player;

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
            playerScore.text = player.transform.GetComponent<PlayerStatManager>().score.ToString();
            playerName.text = player.transform.GetComponent<PlayerStatManager>().thisPlayerColor.ToString();
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
