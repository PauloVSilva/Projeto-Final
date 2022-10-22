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
    [SerializeField] private Slider _playerHealthBar;
    [SerializeField] private Slider _playerShadowHealthBar;
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
    [SerializeField] private TextMeshProUGUI weaponAmmoCapacity;
    [SerializeField] private TextMeshProUGUI weaponAmmoTotal;




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
        characterSprite.sprite = characterManager.Character.sprite[0];
        playerIndex.text = (player.playerIndex + 1).ToString();
        characterName.text = characterManager.Character.characterName.ToString();

        if(characterManager.teamColor == TeamColor.none)    playerTeam.text = null;
        else playerTeam.text = characterManager.teamColor.ToString();

        _playerHealthBar.maxValue = characterManager.characterHealthSystem.MaxHealth;
        _playerHealthBar.value = characterManager.characterHealthSystem.CurrentHealth;

        _playerShadowHealthBar.maxValue = _playerHealthBar.maxValue;
        _playerShadowHealthBar.value = _playerHealthBar.value;
        
        _playerStaminaBar.maxValue = characterManager.characterMovementSystem.MaxStamina;
        _playerStaminaBar.value = characterManager.characterMovementSystem.CurrentStamina;


        _playerTotalLives.text = characterManager.totalLives.ToString();
        playerKillCount.text = characterManager.kills.ToString();
        playerDeathCount.text = characterManager.deaths.ToString();
        playerScore.text = characterManager.score.ToString();
    }

    private void InitializeBars(){
        _playerHealthBar.minValue = 0;
        _playerStaminaBar.minValue = 0;
        _playerShadowHealthBar.minValue = 0;
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
        _playerHealthBar.value = _health;
        StartCoroutine(UpdateShadowHealthDelay());
    }

    IEnumerator UpdateShadowHealthDelay(){
        yield return new WaitForSeconds(0.1f);
        UpdateShadowHealth();
    }

    private void UpdateShadowHealth(){
        if(_playerShadowHealthBar.value < _playerHealthBar.value){
            _playerShadowHealthBar.value++;
            UpdateShadowHealthDelay();
        }
        if(_playerShadowHealthBar.value > _playerHealthBar.value){
            _playerShadowHealthBar.value--;
            UpdateShadowHealthDelay();
        }

    }

    private void UpdateStamina(float _stamina, float _maxStamina){
        _stamina = (int)_stamina;
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
        weaponAmmo.text = _weapon.ammo.ToString();
        weaponAmmoCapacity.text = "/" + _weapon.ammoCapacity.ToString();
        weaponAmmoTotal.text = "(" + _weapon.totalAmmo.ToString() + ")";
    }

    private void UpdateKillCount(GameObject character){
        playerKillCount.text = characterManager.kills.ToString();
    }

    private void UpdateDeathCount(GameObject character){
        playerDeathCount.text = characterManager.deaths.ToString();
        _playerTotalLives.text = characterManager.totalLives.ToString();
    }
}
