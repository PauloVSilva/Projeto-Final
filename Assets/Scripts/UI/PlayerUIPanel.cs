using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public PlayerInput player;
    [SerializeField] CharacterStats characterStats;
    [SerializeField] CharacterEvents characterEvents;

    [SerializeField] private GameObject playerInfo;
    [SerializeField] private GameObject joinMessage;

    [SerializeField] private TextMeshProUGUI playerIndex;
    [SerializeField] private TextMeshProUGUI playerTeam;
    [SerializeField] private TextMeshProUGUI _playerTotalLives;
    [SerializeField] private TextMeshProUGUI playerHealth;
    [SerializeField] private TextMeshProUGUI playerStamina;
    [SerializeField] private TextMeshProUGUI playerScore;

    [SerializeField] private Image weaponSprite;
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
        characterEvents.OnPlayerHealthUpdated += UpdateHealth;
        characterEvents.OnPlayerStaminaUpdated += UpdateStamina;
        characterEvents.OnPlayerScoreChanged += UpdateScore;

        characterEvents.OnPlayerPickedUpWeapon += DisplayWeapon;
        characterEvents.OnPlayerDroppedWeapon += HideWeapon;
        characterEvents.OnPlayerShotWeapon += UpdateAmmo;
        characterEvents.OnPlayerReloadedWeapon += UpdateAmmo;

        characterEvents.OnPlayerScoredKill += UpdateKillCount;
        characterEvents.OnPlayerDied += UpdateDeathCount;
    }

    private void UnsubscribeToPlayerEvents(){
        characterEvents.OnPlayerHealthUpdated -= UpdateHealth;
        characterEvents.OnPlayerStaminaUpdated -= UpdateStamina;
        characterEvents.OnPlayerScoreChanged -= UpdateScore;

        characterEvents.OnPlayerPickedUpWeapon -= DisplayWeapon;
        characterEvents.OnPlayerDroppedWeapon -= HideWeapon;
        characterEvents.OnPlayerShotWeapon -= UpdateAmmo;
        characterEvents.OnPlayerReloadedWeapon -= UpdateAmmo;

        characterEvents.OnPlayerScoredKill -= UpdateKillCount;
        characterEvents.OnPlayerDied -= UpdateDeathCount;
    }

    private void InitializeStats(){
        _playerHealthBar.maxValue = characterStats.MaxHealth;
        _playerHealthBar.value = characterStats.MaxHealth;
        
        _playerStaminaBar.maxValue = characterStats.MaxStamina;
        _playerStaminaBar.value = characterStats.MaxStamina;

        _playerTotalLives.text = characterStats.totalLives.ToString();
        playerIndex.text = (player.playerIndex + 1).ToString();

        if(characterStats.teamColor != null){
            playerTeam.text = characterStats.teamColor.ToString();
        }

        playerHealth.text = characterStats.MaxHealth.ToString() + "/" + characterStats.MaxHealth.ToString();
        playerStamina.text = characterStats.MaxStamina.ToString() + "/" + characterStats.MaxStamina.ToString();
        playerScore.text = characterStats.score.ToString();

        playerKillCount.text = characterStats.kills.ToString();
        playerDeathCount.text = characterStats.deaths.ToString();
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
        characterEvents = playerInput.transform.GetComponent<CharacterEvents>();
        characterStats = playerInput.transform.GetComponent<CharacterStats>();
        SetStatsActive();
        SubscribeToPlayerEvents();
        InitializeStats();
        CheckForWeapon();
    }

    private void CheckForWeapon(){
        if(characterEvents.characterObject.GetComponent<CharacterWeaponSystem>().GetWeapon() != null){
            Weapon _weapon = characterEvents.characterObject.GetComponent<CharacterWeaponSystem>().GetWeapon();
            DisplayWeapon(_weapon);
        }
        else{
            HideWeapon();
        }
    }

    public void UnassignPlayer(){
        if(player != null){
            SetStatsInactive();
            UnsubscribeToPlayerEvents();
            player = null;
            characterEvents = null;
            characterStats = null;
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

    private void DisplayWeapon(Weapon _weapon){
        weaponSprite.gameObject.SetActive(true);
        weaponSprite.sprite = _weapon.sprite;
        UpdateAmmo(_weapon);
    }

    private void HideWeapon(){
        weaponSprite.sprite = null;
        weaponSprite.gameObject.SetActive(false);
    }

    private void UpdateAmmo(Weapon _weapon){
        playerAmmo.text = _weapon.ammo.ToString() + "/" + _weapon.ammoCapacity.ToString();
    }

    private void UpdateKillCount(GameObject character){
        playerKillCount.text = characterStats.kills.ToString();
    }

    private void UpdateDeathCount(GameObject character){
        playerDeathCount.text = characterStats.deaths.ToString();
        _playerTotalLives.text = characterStats.totalLives.ToString();
    }
}
