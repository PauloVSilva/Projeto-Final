using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class PlayerUIPanel : MonoBehaviour
{
    public PlayerInput player;
    private CharacterManager characterManager;
    private CharacterWeaponSystem characterWeaponSystem;

    [SerializeField] private GameObject playerInfo;
    [SerializeField] private GameObject joinMessage;

    
    [Header("MainInfo")]
    [SerializeField] private GameObject mainInfoPanel;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI playerIndex;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private Slider _playerHealthBar;
    [SerializeField] private Slider _playerShadowHealthBar;
 
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


    private void Start()
    {
        UpdatePanels();
        InitializeBars();
    }

    private void OnDestroy()
    {
        UnassignPlayer();
    }


    private void UpdatePanels()
    {
        playerInfo.gameObject.SetActive(player != null);
        joinMessage.gameObject.SetActive(player == null);
    }
    
    private void InitializeBars(){
        _playerHealthBar.minValue = 0;
        _playerShadowHealthBar.minValue = 0;
    }

    
    public void AssignPlayer(PlayerInput playerInput)
    {
        StartCoroutine(AssignPlayerDelay(playerInput));
        IEnumerator AssignPlayerDelay(PlayerInput playerInput)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            player = playerInput;

            player.transform.TryGetComponent(out CharacterManager _characterManager);

            characterManager = _characterManager;
            characterWeaponSystem = characterManager.characterWeaponSystem;

            SubscribeToPlayerEvents();
        }
    }
    
    private void SubscribeToPlayerEvents()
    {
        characterManager.OnCharacterChosen += AssignCharacter;

        characterManager.OnPlayerHealthUpdated += UpdateHealth;
        characterManager.OnPlayerScoreChanged += UpdateScore;

        characterManager.OnPlayerPickedUpWeapon += DisplayWeapon;
        characterManager.OnPlayerDroppedWeapon += HideWeapon;
        characterManager.OnPlayerShotWeapon += UpdateAmmo;
        characterManager.OnPlayerReloadedWeapon += UpdateAmmo;

        characterManager.OnPlayerScoredKill += UpdateKillCount;
        characterManager.OnPlayerDied += UpdateDeathCount;

        characterManager.OnPlayerStatsReset += UpdatePanelStats;
    }

    public void AssignCharacter()
    {
        StartCoroutine(AssignCharacterDelay());
        IEnumerator AssignCharacterDelay()
        {
            yield return new WaitForSecondsRealtime(0.01f);

            UpdatePanels();
            InitializeStats();
            CheckForWeapon();
        }
    }
    
    private void InitializeStats()
    {
        characterSprite.sprite = characterManager.characterSkin.characterSprite;
        characterName.text = characterManager.characterSkin.characterName;

        playerIndex.text = (player.playerIndex + 1).ToString();
        mainInfoPanel.GetComponent<Image>().color = characterManager.UIColor;

        _playerHealthBar.maxValue = characterManager.characterHealthSystem.MaxHealth;
        _playerHealthBar.value = characterManager.characterHealthSystem.CurrentHealth;

        _playerShadowHealthBar.maxValue = _playerHealthBar.maxValue;
        _playerShadowHealthBar.value = _playerHealthBar.value;

        _playerTotalLives.text = characterManager.totalLives.ToString();
        playerKillCount.text = characterManager.kills.ToString();
        playerDeathCount.text = characterManager.deaths.ToString();
        playerScore.text = characterManager.score.ToString();

        StartCoroutine(UpdateShadowBars());
    }

    private void CheckForWeapon(){
        if(characterWeaponSystem.CharacterWeapon == null) HideWeapon();
        else
        {
            Weapon _weapon = characterWeaponSystem.CharacterWeapon;
            DisplayWeapon(_weapon);
        }
    }

    public void UnassignPlayer()
    {
        if (player == null) return;

        player = null;
        UpdatePanels();
        UnsubscribeToPlayerEvents();
        characterManager = null;
    }

    private void UnsubscribeToPlayerEvents(){
        characterManager.OnCharacterChosen -= AssignCharacter;

        characterManager.OnPlayerHealthUpdated -= UpdateHealth;
        characterManager.OnPlayerScoreChanged -= UpdateScore;

        characterManager.OnPlayerPickedUpWeapon -= DisplayWeapon;
        characterManager.OnPlayerDroppedWeapon -= HideWeapon;
        characterManager.OnPlayerShotWeapon -= UpdateAmmo;
        characterManager.OnPlayerReloadedWeapon -= UpdateAmmo;

        characterManager.OnPlayerScoredKill -= UpdateKillCount;
        characterManager.OnPlayerDied -= UpdateDeathCount;

        characterManager.OnPlayerStatsReset -= UpdatePanelStats;
    }

    private void UpdatePanelStats(){
        InitializeStats();
        CheckForWeapon();
    }



    private void UpdateHealth(float _health, float _maxHealth){
        _health = (int)_health;
        _playerHealthBar.value = _health;
    }

    IEnumerator UpdateShadowBars()
    {
        yield return new WaitForSeconds(0.001f);

        if(_playerShadowHealthBar.value < _playerHealthBar.value)
        {
            _playerShadowHealthBar.value++;
        }
        if(_playerShadowHealthBar.value > _playerHealthBar.value)
        {
            _playerShadowHealthBar.value--;
        }

        StartCoroutine(UpdateShadowBars());
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
        //weaponAmmoCapacity.text = "/" + _weapon.ammoCapacity.ToString();
        //weaponAmmoTotal.text = "(" + _weapon.totalAmmo.ToString() + ")";
    }

    private void UpdateKillCount(GameObject character){
        playerKillCount.text = characterManager.kills.ToString();
    }

    private void UpdateDeathCount(GameObject character){
        playerDeathCount.text = characterManager.deaths.ToString();
        _playerTotalLives.text = characterManager.totalLives.ToString();
    }
}
