using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEvents : MonoBehaviour{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterController controller;
    [SerializeField] public CharacterInventory characterInventory;
    [SerializeField] public GameObject characterObject;

    //EVENTS THAT WILL BE SENT TO OTHER CLASSES
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

    public void SetEvents(){
        characterObject = GetComponent<CharacterSelection>().characterObject;
    }

    public void OnTriggerEnter(Collider other){
        if(!characterStats.IsBlocked()){
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
                    if(!characterStats.IsArmed()){
                        characterInventory.PickWeapon(other.gameObject);
                    }
                }
            }
        }
    }

    public void RefreshStatsUponRespawning(){
        GetComponent<CharacterHealthSystem>().Initialize();
        GetComponent<MovementSystem>().ResetStats();
    }


    public void BeginRespawnProcess(){
        if(characterStats.CanRespawn()){
            StartCoroutine(RespawnCharacterDelay());
        }
        characterObject.SetActive(false);
        BlockActions();
    }

    IEnumerator RespawnCharacterDelay(){
        yield return new WaitForSeconds(characterStats.timeToRespawn);
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
        characterStats.actionsAreBlocked = true;
        playerInputHandler.DisableActions();
    }

    public void UnblockActions(){
        characterStats.actionsAreBlocked = false;
        playerInputHandler.RestoreActions();
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
        characterStats.ResetStats();
        this.transform.position = GameManager.instance.spawnPoints[0].transform.position;
        characterObject.SetActive(true);
        UnblockActions();
        GetComponent<CharacterHealthSystem>().ResetStats();
        GetComponent<MovementSystem>().ResetStats();
        OnPlayerStatsReset?.Invoke();
    }

    public void RefreshStats(){
        OnPlayerStatsReset?.Invoke();
    }

    public void SetLimitedLives(int _lives){
        characterStats.SetLimitedLives(_lives);
        OnPlayerStatsReset?.Invoke();
    }

    public void IncreaseScore(int value){
        characterStats.IncreaseScore(value);
        OnPlayerScoreChanged?.Invoke(gameObject, characterStats.GetScore());
    }

    public void PlayerScoredKill(GameObject character){
        characterStats.IncreaseKills();
        OnPlayerScoredKill?.Invoke(character);
    }

    public void PlayerDied(GameObject character){
        characterStats.IncreaseDeaths();
        if(!characterStats.unlimitedLives){
            characterStats.DecreaseLives();
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
