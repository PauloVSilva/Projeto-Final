using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public PlayerInput player;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerHealth;
    public TextMeshProUGUI playerStamina;
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI playerKillCount;
    public TextMeshProUGUI playerDeathCount;

    public TextMeshProUGUI pressToJoin;


    private void Awake() {
        pressToJoin.text = "Press X to join";
    }

    private void Start(){
        //UpdateScore(0);
        playerName.gameObject.SetActive(false);
        playerHealth.gameObject.SetActive(false);
        playerStamina.gameObject.SetActive(false);
        playerScore.gameObject.SetActive(false);
        playerKillCount.gameObject.SetActive(false);
        playerDeathCount.gameObject.SetActive(false);
        pressToJoin.gameObject.SetActive(true);
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
            playerName.gameObject.SetActive(true);
            playerHealth.gameObject.SetActive(true);
            playerStamina.gameObject.SetActive(true);
            playerScore.gameObject.SetActive(true);
            playerKillCount.gameObject.SetActive(true);
            playerDeathCount.gameObject.SetActive(true);
            pressToJoin.gameObject.SetActive(false);

            player.transform.GetComponent<CharacterEvents>().OnPlayerHealthUpdated += UpdateHealth;
            player.transform.GetComponent<CharacterEvents>().OnPlayerStaminaUpdated += UpdateStamina;
            player.transform.GetComponent<CharacterEvents>().OnPlayerScoreChanged += UpdateScore;
            player.transform.GetComponent<CharacterEvents>().OnPlayerScoredKill += UpdateKillCount;
            player.transform.GetComponent<CharacterEvents>().OnPlayerDied += UpdateDeathCount;

            playerName.text = player.transform.GetComponent<CharacterStats>().teamColor.ToString();
            playerHealth.text = player.transform.GetChild(0).GetComponent<HealthSystem>().CurrentHealth.ToString() + "/" + player.GetComponent<CharacterStats>().MaxHealth.ToString();
            playerStamina.text = player.transform.GetChild(0).GetComponent<MovementSystem>().CurrentStamina.ToString() + "/" + player.GetComponent<CharacterStats>().MaxStamina.ToString();
            playerScore.text = player.transform.GetComponent<CharacterStats>().score.ToString();
            playerKillCount.text = player.transform.GetComponent<CharacterStats>().kills.ToString();
            playerDeathCount.text = player.transform.GetComponent<CharacterStats>().deaths.ToString();
        }
        else{
            playerName.gameObject.SetActive(false);
            playerHealth.gameObject.SetActive(false);
            playerStamina.gameObject.SetActive(false);
            playerScore.gameObject.SetActive(false);
            playerKillCount.gameObject.SetActive(false);
            playerDeathCount.gameObject.SetActive(false);
            pressToJoin.gameObject.SetActive(true);
        }
    }

    private void UpdateHealth(float health){
        health = (int)health;
        playerHealth.text = health.ToString() + "/" + player.GetComponent<CharacterStats>().MaxHealth.ToString();
    }

    private void UpdateStamina(float stamina){
        stamina = (int)stamina;
        playerStamina.text = stamina.ToString() + "/" + player.GetComponent<CharacterStats>().MaxStamina.ToString();
    }

    private void UpdateScore(GameObject character){
        int score = character.GetComponent<CharacterStats>().score;
        playerScore.text = score.ToString();
    }

    private void UpdateKillCount(GameObject character){
        int killCount = character.transform.parent.GetComponent<CharacterStats>().kills;
        playerKillCount.text = killCount.ToString();
    }

    private void UpdateDeathCount(GameObject character){
        int deathCount = character.transform.parent.GetComponent<CharacterStats>().deaths;
        playerDeathCount.text = deathCount.ToString();
    }
}
