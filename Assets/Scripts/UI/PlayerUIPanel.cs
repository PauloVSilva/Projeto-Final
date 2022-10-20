using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerUIPanel : MonoBehaviour{
    public PlayerInput player;
    [SerializeField] CharacterManager characterManager;
    [SerializeField] CharacterWeaponSystem characterWeaponSystem;

    [SerializeField] private GameObject playerInfo;
    [SerializeField] private GameObject joinMessage;
    [SerializeField] private TextMeshProUGUI pressToJoin;

    
    [Header("MainInfo")]
    [SerializeField] private GameObject mainInfoPanel;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI playerIndex;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI playerTeam;
    [SerializeField] private TextMeshProUGUI playerHealth;
    [SerializeField] private TextMeshProUGUI playerStamina;
    [SerializeField] private Slider _playerHealthBar;
    [SerializeField] private Slider _playerStaminaBar;

    [Space(5)]
    [Header("Stats")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI _playerTotalLives;
    [SerializeField] private TextMeshProUGUI playerKillCount;
    [SerializeField] private TextMeshProUGUI playerDeathCount;
    [SerializeField] private TextMeshProUGUI playerScore;

    [Space(5)]
    [Header("Weapon")]
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private Image weaponSprite;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponAmmo;




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
        characterManager.OnPlayerHealthUpdated += UpdateHealth;
        characterManager.OnPlayerStaminaUpdated += UpdateStamina;
        characterManager.OnPlayerScoreChanged += UpdateScore;

        characterManager.OnPlayerPickedUpWeapon += DisplayWeapon;
        characterManager.OnPlayerDroppedWeapon += HideWeapon;
        characterManager.OnPlayerShotWeapon += UpdateAmmo;
        characterManager.OnPlayerReloadedWeapon += UpdateAmmo;

        characterManager.OnPlayerScoredKill += UpdateKillCount;
        characterManager.OnPlayerDied += UpdateDeathCount;

        characterManager.OnPlayerStatsReset += UpdatePanel;
    }

    private void UnsubscribeToPlayerEvents(){
        characterManager.OnPlayerHealthUpdated -= UpdateHealth;
        characterManager.OnPlayerStaminaUpdated -= UpdateStamina;
        characterManager.OnPlayerScoreChanged -= UpdateScore;

        characterManager.OnPlayerPickedUpWeapon -= DisplayWeapon;
        characterManager.OnPlayerDroppedWeapon -= HideWeapon;
        characterManager.OnPlayerShotWeapon -= UpdateAmmo;
        characterManager.OnPlayerReloadedWeapon -= UpdateAmmo;

        characterManager.OnPlayerScoredKill -= UpdateKillCount;
        characterManager.OnPlayerDied -= UpdateDeathCount;

        characterManager.OnPlayerStatsReset -= UpdatePanel;
    }

    private void UpdatePanel(){
        InitializeStats();
        CheckForWeapon();
    }

    private void InitializeStats(){
        _playerHealthBar.maxValue = characterManager.characterHealthSystem.MaxHealth;
        _playerHealthBar.value = characterManager.characterHealthSystem.CurrentHealth;
        
        _playerStaminaBar.maxValue = characterManager.characterMovementSystem.MaxStamina;
        _playerStaminaBar.value = characterManager.characterMovementSystem.CurrentStamina;

        _playerTotalLives.text = characterManager.totalLives.ToString();
        playerIndex.text = (player.playerIndex + 1).ToString();

        if(characterManager.teamColor == TeamColor.none){
            playerTeam.text = null;
        }
        else{
            playerTeam.text = characterManager.teamColor.ToString();
        }

        characterSprite.sprite = characterManager.Character.sprite[0];
        characterName.text = characterManager.Character.characterName.ToString();

        playerHealth.text = characterManager.characterHealthSystem.CurrentHealth.ToString() + "/" + characterManager.characterHealthSystem.MaxHealth.ToString();
        playerStamina.text = characterManager.characterMovementSystem.CurrentStamina.ToString() + "/" + characterManager.characterMovementSystem.MaxStamina.ToString();
        playerScore.text = characterManager.score.ToString();

        playerKillCount.text = characterManager.kills.ToString();
        playerDeathCount.text = characterManager.deaths.ToString();
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
        player.transform.GetComponent<CharacterManager>().OnCharacterChosen += AssignCharacter;
    }

    public void AssignCharacter(){
        StartCoroutine(AssignCharacterDelay());
    }

    IEnumerator AssignCharacterDelay(){
        yield return new WaitForSeconds(0.01f);
        characterManager = player.transform.GetComponent<CharacterManager>();
        characterWeaponSystem = player.transform.GetComponent<CharacterWeaponSystem>();
        SetStatsActive();
        SubscribeToPlayerEvents();
        InitializeStats();
        CheckForWeapon();
    }


    private void CheckForWeapon(){
        if(characterWeaponSystem.GetWeapon() != null){
            Weapon _weapon = characterWeaponSystem.GetWeapon();
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
            player.transform.GetComponent<CharacterManager>().OnCharacterChosen -= AssignCharacter;
            player = null;
            characterManager = null;
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
        weaponPanel.SetActive(true);
        weaponSprite.sprite = _weapon.sprite;
        weaponName.text = _weapon.weaponName;
        UpdateAmmo(_weapon);
    }

    private void HideWeapon(){
        weaponSprite.sprite = null;
        weaponName.text = null;
        weaponAmmo.text = null;
        weaponPanel.SetActive(false);
    }

    private void UpdateAmmo(Weapon _weapon){
        weaponAmmo.text = _weapon.ammo.ToString() + "/" + _weapon.ammoCapacity.ToString();
    }

    private void UpdateKillCount(GameObject character){
        playerKillCount.text = characterManager.kills.ToString();
    }

    private void UpdateDeathCount(GameObject character){
        playerDeathCount.text = characterManager.deaths.ToString();
        _playerTotalLives.text = characterManager.totalLives.ToString();
    }
}
