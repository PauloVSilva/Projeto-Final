using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public PlayerInput player;

    [SerializeField] private GameObject playerInfo;
    [SerializeField] private GameObject joinMessage;

    [SerializeField] private TextMeshProUGUI playerIndex;
    [SerializeField] private TextMeshProUGUI playerTeam;
    [SerializeField] private TextMeshProUGUI _playerTotalLives;
    [SerializeField] private TextMeshProUGUI playerHealth;
    [SerializeField] private TextMeshProUGUI playerStamina;
    [SerializeField] private TextMeshProUGUI playerScore;
    [SerializeField] private TextMeshProUGUI playerKillCount;
    [SerializeField] private TextMeshProUGUI playerDeathCount;

    [SerializeField] private Slider _playerHealthBar;
    [SerializeField] private Slider _playerStaminaBar;

    [SerializeField] private TextMeshProUGUI pressToJoin;

    private void Start(){
        SetStatsInactive();
        InitializeBars();
    }

    private void OnDisable(){
        UnassignPlayer();
    }

    private void SetStatsActive(){
        playerInfo.gameObject.SetActive(true);
        joinMessage.gameObject.SetActive(false);
    }

    private void SetStatsInactive(){
        playerInfo.gameObject.SetActive(false);
        joinMessage.gameObject.SetActive(true);
    }

    private void SubscribeToPlayerEvents(){
        player.transform.GetComponent<CharacterEvents>().OnPlayerHealthUpdated += UpdateHealth;
        player.transform.GetComponent<CharacterEvents>().OnPlayerStaminaUpdated += UpdateStamina;
        player.transform.GetComponent<CharacterEvents>().OnPlayerScoreChanged += UpdateScore;
        player.transform.GetComponent<CharacterEvents>().OnPlayerScoredKill += UpdateKillCount;
        player.transform.GetComponent<CharacterEvents>().OnPlayerDied += UpdateDeathCount;
    }

    private void UnsubscribeToPlayerEvents(){
        player.transform.GetComponent<CharacterEvents>().OnPlayerHealthUpdated -= UpdateHealth;
        player.transform.GetComponent<CharacterEvents>().OnPlayerStaminaUpdated -= UpdateStamina;
        player.transform.GetComponent<CharacterEvents>().OnPlayerScoreChanged -= UpdateScore;
        player.transform.GetComponent<CharacterEvents>().OnPlayerScoredKill -= UpdateKillCount;
        player.transform.GetComponent<CharacterEvents>().OnPlayerDied -= UpdateDeathCount;
    }

    private void InitializeStats(){
        _playerHealthBar.maxValue = player.GetComponent<CharacterStats>().MaxHealth;
        _playerHealthBar.value = player.GetComponent<CharacterStats>().MaxHealth;
        
        _playerStaminaBar.maxValue = player.GetComponent<CharacterStats>().MaxStamina;
        _playerStaminaBar.value = player.GetComponent<CharacterStats>().MaxStamina;

        _playerTotalLives.text = player.GetComponent<CharacterStats>().totalLives.ToString();

        playerIndex.text = (player.playerIndex + 1).ToString();
        if(player.transform.GetComponent<CharacterStats>().teamColor != null){
            playerTeam.text = player.transform.GetComponent<CharacterStats>().teamColor.ToString();
        }
        playerHealth.text = player.transform.GetComponent<CharacterSelection>().characterObject.GetComponent<HealthSystem>().CurrentHealth.ToString() + "/" + player.GetComponent<CharacterStats>().MaxHealth.ToString();
        playerStamina.text = player.transform.GetComponent<CharacterSelection>().characterObject.GetComponent<MovementSystem>().CurrentStamina.ToString() + "/" + player.GetComponent<CharacterStats>().MaxStamina.ToString();
        playerScore.text = player.transform.GetComponent<CharacterStats>().score.ToString();
        playerKillCount.text = player.transform.GetComponent<CharacterStats>().kills.ToString();
        playerDeathCount.text = player.transform.GetComponent<CharacterStats>().deaths.ToString();
    }

    private void InitializeBars(){
        _playerHealthBar.minValue = 0;
        _playerStaminaBar.minValue = 0;
    }

    public void AssignPlayer(PlayerInput playerInput){
        StartCoroutine(AssignPlayerDelay(playerInput));
    }

    IEnumerator AssignPlayerDelay(PlayerInput playerInput){
        yield return new WaitForSeconds(0.01f);
        player = playerInput;
        SetStatsActive();
        SubscribeToPlayerEvents();
        InitializeStats();
    }

    public void UnassignPlayer(){
        if(player != null){
            SetStatsInactive();
            UnsubscribeToPlayerEvents();
            player = null;
        }
    }

    private void UpdateHealth(float health){
        health = (int)health;
        playerHealth.text = health.ToString() + "/" + player.GetComponent<CharacterStats>().MaxHealth.ToString();
        _playerHealthBar.value = health;
    }

    private void UpdateStamina(float stamina){
        stamina = (int)stamina;
        playerStamina.text = stamina.ToString() + "/" + player.GetComponent<CharacterStats>().MaxStamina.ToString();
        _playerStaminaBar.value = stamina;
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
        _playerTotalLives.text = player.GetComponent<CharacterStats>().totalLives.ToString();
    }
}
