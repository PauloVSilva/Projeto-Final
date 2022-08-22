using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;

    PlayerController player;

    private void Start(){
        UpdateScore(0);
    }

    private void Update(){
        if (player != null){
            playerScore.text = player.score.ToString();
            playerName.text = player.thisPlayerColor.ToString();
        }
    }

    public void AssignPlayer(int index){
        //Debug.Log("AssignPlayer");
        StartCoroutine(AssignPlayerDelay(index));
    }

    IEnumerator AssignPlayerDelay(int index){
        yield return new WaitForSeconds(0.01f);
        player = GameManager.instance.playerList[index].GetComponent<PlayerInputHandler>().playerController;

        SetUpInfoPanel();
    }

    void SetUpInfoPanel(){
        if(player != null){
            player.OnScoreChanged += UpdateScore;
            playerName.text = player.thisPlayerColor.ToString();
        }
    }

    private void UpdateScore(int score){
        playerScore.text = score.ToString();
    }
}
