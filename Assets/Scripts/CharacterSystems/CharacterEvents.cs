using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEvents : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;
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
        characterStats = gameObject.GetComponent<CharacterStats>();
    }

    public void SubscribeToPlayerEvents(){ //allow managers to subscribe to this class' events
        OnPlayerScoredKill += GameManager.instance.GameManagerCharacterKilled;
        OnPlayerDied += GameManager.instance.GameManagerCharacterDied;
        OnPlayerBorn += GameManager.instance.GameManagerCharacterSpawned;
    }

    public void UnsubscribeFromPlayerEvents(){ //allow managers to unsubscribe from this class' events
        OnPlayerScoredKill -= GameManager.instance.GameManagerCharacterKilled;
        OnPlayerDied -= GameManager.instance.GameManagerCharacterDied;
        OnPlayerBorn -= GameManager.instance.GameManagerCharacterSpawned;
    }


    public void FilterCollision(GameObject character, GameObject _gameObject){
        if(_gameObject.CompareTag("Coin")){
            if(_gameObject.GetComponent<Coin>().canBePickedUp){
                IncreaseScore(_gameObject.GetComponent<Coin>().value);
                Destroy(_gameObject);
            }
        }
        if(_gameObject.CompareTag("Instadeath")){
            character.GetComponent<HealthSystem>().Kill();
        }
        if(_gameObject.CompareTag("Weapon")){
            if(_gameObject.GetComponent<Weapon>().CanBePickedUp()){
                character.GetComponent<CharacterWeaponSystem>().PickUpWeapon(_gameObject);
            }
        }
    }

    public void RefreshStatsUponRespawning(){
        characterObject.GetComponent<HealthSystem>().ResetStats();
        characterObject.GetComponent<MovementSystem>().ResetStats();
    }


    public void BeginRespawnProcess(){
        if(characterStats.CanRespawn()){
            StartCoroutine(RespawnCharacterDelay());
        }
        characterObject.SetActive(false);
    }

    IEnumerator RespawnCharacterDelay(){
        yield return new WaitForSeconds(characterStats.timeToRespawn);
        RespawnCharacter();
    }

    public void RespawnCharacter(){
        int randomIndex = UnityEngine.Random.Range(0, GameManager.instance.spawnPoints.Length);
        characterObject.transform.position = GameManager.instance.spawnPoints[randomIndex].transform.position;
        characterObject.SetActive(true);
        RefreshStatsUponRespawning();
    }






    //METHODS THAT INVOKE EVENTS
    public void ResetStats(){
        characterStats.ResetStats();
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

}
