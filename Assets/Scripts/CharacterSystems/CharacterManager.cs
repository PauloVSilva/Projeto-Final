using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Animal{hedgehog, pangolin, threeBandedArmadillo}
public enum TeamColor{none, blue, red, green, yellow}

public class CharacterManager : MonoBehaviour{
    #region "SYSTEMS"
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] public PlayerInputHandler playerInputHandler;
    [SerializeField] public CharacterHealthSystem characterHealthSystem;
    [SerializeField] public MovementSystem characterMovementSystem;
    [SerializeField] public CharacterInventory characterInventory;
    [SerializeField] public CharacterWeaponSystem characterWeaponSystem;
    [SerializeField] public CharacterStatsScriptableObject Character;
    [SerializeField] public GameObject characterObject;
    #endregion "SYSTEMS"

    #region "STATS"
    public TeamColor teamColor;
    public int score;
    public int kills;
    public int deaths;
    public bool unlimitedLives;
    public int totalLives;
    public float timeToRespawn;
    public bool actionsAreBlocked;
    public bool isMountedOnTurret;
    #endregion "STATS"

    #region "EVENTS"
    public event System.Action<GameObject, int> OnPlayerScoreChanged;
    public event System.Action<GameObject> OnPlayerScoredKill;
    public event System.Action<GameObject> OnPlayerDied;
    public event System.Action<GameObject> OnPlayerBorn;
    public event System.Action<float> OnPlayerWasDamaged;
    public event System.Action<float> OnPlayerWasHealed;
    public event System.Action<float, float> OnPlayerHealthUpdated; 
    public event System.Action<float, float> OnPlayerStaminaUpdated; 
    public event System.Action<Weapon> OnPlayerShotWeapon; 
    public event System.Action<Weapon> OnPlayerReloadedWeapon;
    public event System.Action<Weapon> OnPlayerPickedUpWeapon;
    public event System.Action OnPlayerDroppedWeapon;
    public event System.Action OnPlayerStatsReset;
    public event System.Action OnCharacterChosen;
    #endregion "EVENTS"

    private void Awake(){
        InitializeComponents();
        InitializePlayerVariables();
    }

    private void InitializeComponents(){
        characterObject = null;
        playerInput = GetComponent<PlayerInput>();
        playerInputHandler = GetComponent<PlayerInputHandler>();

        characterHealthSystem = GetComponent<CharacterHealthSystem>();
        characterMovementSystem = GetComponent<MovementSystem>();
        characterInventory = GetComponent<CharacterInventory>();
        characterWeaponSystem = GetComponent<CharacterWeaponSystem>();
    }

    private void InitializePlayerVariables(){
        teamColor = TeamColor.none;
        score = 0;
        kills = 0;
        deaths = 0;
        unlimitedLives = true;
        totalLives = 0;
        timeToRespawn = 3f;

        actionsAreBlocked = false;
        isMountedOnTurret = false;
    }

    public void ResetStats(){
        InitializePlayerVariables();
    }

    public void SpawnCharacter(CharacterStatsScriptableObject _character){
        Character = _character;

        characterObject = GameObject.Instantiate(Character.characterModel[0], transform.position, transform.rotation, this.transform);
        characterHealthSystem.Initialize();
        characterMovementSystem.Initialize();
        characterWeaponSystem.SetGunPosition();

        transform.parent = GameManager.instance.transform;
        playerInput.SwitchCurrentActionMap("Player");
        OnCharacterChosen?.Invoke();
    }

    public void OnTriggerEnter(Collider other){
        if(!IsBlocked()){
            if(other.gameObject.GetComponent<Item>() != null && other.gameObject.GetComponent<Item>().CanBePickedUp){
                Debug.Log("Collided");
                if(other.gameObject.GetComponent<Coin>()){
                    if(characterInventory.AddToInventory(other.gameObject.GetComponent<Item>().item)){
                        other.GetComponent<Coin>().PickedUp(this.gameObject);
                    }
                }
                if(other.gameObject.GetComponent<Food>()){
                    other.GetComponent<Food>().PickedUp(this.gameObject);
                }
                if(other.gameObject.GetComponent<Weapon>()){
                    if(!IsArmed()){
                        characterInventory.PickWeapon(other.gameObject);
                    }
                }
            }
        }
    }

    public void RefreshStatsUponRespawning(){
        characterHealthSystem.Initialize();
        characterMovementSystem.Initialize();
    }


    public void BeginRespawnProcess(){
        if(CanRespawn()){
            StartCoroutine(RespawnCharacterDelay());
        }
        characterObject.SetActive(false);
        BlockActions();
    }

    IEnumerator RespawnCharacterDelay(){
        yield return new WaitForSeconds(timeToRespawn);
        RespawnCharacter();
    }

    public void RespawnCharacter(){
        int randomIndex = UnityEngine.Random.Range(0, GameManager.instance.spawnPoints.Length);
        this.transform.position = GameManager.instance.spawnPoints[randomIndex].transform.position;
        characterObject.SetActive(true);
        UnblockActions();
        RefreshStatsUponRespawning();
    }

    public void BlockActions(){
        actionsAreBlocked = true;
        playerInputHandler.DisableActions();
    }

    public void UnblockActions(){
        actionsAreBlocked = false;
        playerInputHandler.RestoreActions();
    }





    public void SetTeam(TeamColor _teamColor){
        teamColor = _teamColor;
    }

    public int GetScore(){
        return score;
    }

    public void IncreaseLives(int _amount){
        totalLives += _amount;
    }

    public void SetUnlimitedLives(){
        totalLives = -1;
        unlimitedLives = true;
    }

    public bool IsBlocked(){
        return actionsAreBlocked;
    }

    public bool CanRespawn(){
        if(totalLives > 0 || unlimitedLives){
            return true;
        }
        else{
            return false;
        }
    }

    public bool CanMove(){
        if(actionsAreBlocked) return false;
        
        return true;
    }

    public bool CanUseFireGun(){
        if(actionsAreBlocked) return false;
        if(!IsArmed()) return false;

        return true;
    }

    public bool IsArmed(){
        return characterInventory.IsArmed();
    }

    #region "Methods that invoke events"
    public void FullReset(){
        //reset all base stats
        //clear inventory
        //move player to default spawnpoint
        //set character active
        //unblock actions
        //reset health and movement systems
        //send that to the UI
        characterInventory.ClearInventory();
        ResetStats();
        this.transform.position = GameManager.instance.spawnPoints[0].transform.position;
        characterObject.SetActive(true);
        UnblockActions();
        characterHealthSystem.Initialize();
        characterMovementSystem.Initialize();
        OnPlayerStatsReset?.Invoke();
    }

    public void SetLimitedLives(int _amount){
        totalLives = _amount;
        unlimitedLives = false;
        OnPlayerStatsReset?.Invoke();
    }

    public void IncreaseScore(int value){
        score += value;
        OnPlayerScoreChanged?.Invoke(gameObject, score);
    }

    public void DecreaseScore(int value){
        score = Math.Max(score -= value, 0);
    }

    public void PlayerScoredKill(GameObject character){
        kills++;
        OnPlayerScoredKill?.Invoke(character);
    }

    public void PlayerDied(GameObject character){
        deaths++;
        if(!unlimitedLives){
            totalLives--;
        }
        BeginRespawnProcess();
        characterInventory.DropAllInventory();

        Instantiate(GameManager.instance.DeathSpot, character.transform.position, Quaternion.Euler(0, 0, 0), this.transform);
        OnPlayerDied?.Invoke(character);
    }

    public void PlayerBorn(GameObject character){
        OnPlayerBorn?.Invoke(character);
    }

    public void PlayerWasDamaged(float _damageAmount){
        OnPlayerWasDamaged?.Invoke(_damageAmount);
    }

    public void PlayerWasHealed(float _healAmount){
        OnPlayerWasHealed?.Invoke(_healAmount);
    }

    public void PlayerHealthUpdated(float _currentHealth, float _maxHealth){
        OnPlayerHealthUpdated?.Invoke(_currentHealth, _maxHealth);
    }

    public void PlayerStaminaUpdated(float _currentStamina, float _maxStamina){
        OnPlayerStaminaUpdated?.Invoke(_currentStamina, _maxStamina);
    }

    public void PlayerShotWeapon(Weapon _weapon){
        OnPlayerShotWeapon?.Invoke(_weapon);
    }

    public void PlayerReloadedWeapon(Weapon _weapon){
        OnPlayerReloadedWeapon?.Invoke(_weapon);
    }

    public void PlayerPickedUpWeapon(Weapon _weapon){
        OnPlayerPickedUpWeapon?.Invoke(_weapon);
    }

    public void PlayerDroppedWeapon(){
        OnPlayerDroppedWeapon?.Invoke();
    }
    #endregion "Methods that invoke events"
}
