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
    [SerializeField] private TextMeshProUGUI playerAmmo;
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
        player.transform.GetComponent<CharacterEvents>().OnPlayerShotWeapon += UpdateAmmo;
        player.transform.GetComponent<CharacterEvents>().OnPlayerReloadedWeapon += UpdateAmmo;
        player.transform.GetComponent<CharacterEvents>().OnPlayerScoredKill += UpdateKillCount;
        player.transform.GetComponent<CharacterEvents>().OnPlayerDied += UpdateDeathCount;
    }

    private void UnsubscribeToPlayerEvents(){
        player.transform.GetComponent<CharacterEvents>().OnPlayerHealthUpdated -= UpdateHealth;
        player.transform.GetComponent<CharacterEvents>().OnPlayerStaminaUpdated -= UpdateStamina;
        player.transform.GetComponent<CharacterEvents>().OnPlayerScoreChanged -= UpdateScore;
        player.transform.GetComponent<CharacterEvents>().OnPlayerShotWeapon -= UpdateAmmo;
        player.transform.GetComponent<CharacterEvents>().OnPlayerReloadedWeapon -= UpdateAmmo;
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

        playerAmmo.text = "0";

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

    private void UpdateHealth(float _health, float _maxHealth){
        _health = (int)_health;
        playerHealth.text = _health.ToString() + "/" + _maxHealth.ToString();
        _playerHealthBar.value = _health;
    }

    private void UpdateStamina(float _stamina, float _maxStamina){
        _stamina = (int)_stamina;
        playerStamina.text = _stamina.ToString() + "/" + _maxStamina.ToString();
        _playerStaminaBar.value = _stamina;
    }

    private void UpdateScore(GameObject _character, int _score){
        playerScore.text = _score.ToString();
    }

    private void UpdateAmmo(Weapon _weapon){
        playerAmmo.text = _weapon.ammo.ToString() + "/" + _weapon.ammoCapacity.ToString();
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
