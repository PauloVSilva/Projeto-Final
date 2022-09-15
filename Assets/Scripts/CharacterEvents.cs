using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEvents : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;

    //EVENTS THAT WILL BE SENT TO OTHER CLASSES
    public event System.Action<GameObject> OnPlayerScoreChanged;
    public event System.Action<GameObject> OnPlayerScoredKill;
    public event System.Action<GameObject> OnPlayerDied;
    public event System.Action<GameObject> OnPlayerBorn;
    public event System.Action<float> OnPlayerWasDamaged;
    public event System.Action<float> OnPlayerWasHealed;
    public event System.Action<float> OnPlayerHealthUpdated; 
    public event System.Action<float> OnPlayerStaminaUpdated; 
    //public event System.Action<int> OnWeaponFired;

    private void Start() {
        SubscribeToOwnEvents();
        characterStats = gameObject.GetComponent<CharacterStats>();
    }

    private void SubscribeToOwnEvents(){ //subscribe to child system events like health or movement
        //HEALTH EVENTS
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityDied += PlayerDied;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityScoredKill += PlayerScoredKill;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityBorn += PlayerBorn;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityTookDamage += PlayerWasDamaged;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityHealed += PlayerWasHealed;
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().OnEntityHealthUpdated += PlayerHealthUpdated;
        //MOVEMENT EVENTS
        gameObject.transform.GetChild(0).GetComponent<MovementSystem>().OnEntityStaminaUpdated += PlayerStaminaUpdated;
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

    public void ResetScores(){
        gameObject.GetComponent<CharacterStats>().ResetScores();
    }

    public void FilterCollision(GameObject player, GameObject _gameObject){
        if(_gameObject.CompareTag("Coin")){
            if(_gameObject.GetComponent<Coin>().canBePickedUp){
                IncreaseScore(_gameObject.GetComponent<Coin>().value);
                Destroy(_gameObject);
            }
        }
        if(_gameObject.CompareTag("Instadeath")){
            player.GetComponent<HealthSystem>().Kill();
        }
        if(_gameObject.CompareTag("Weapon")){
            if(!gameObject.GetComponent<CharacterStats>().isArmed && _gameObject.GetComponent<Weapon>().CanBePickedUp()){
                _gameObject.GetComponent<Weapon>().PickUpWeapon(gameObject.transform.GetChild(0).gameObject);
            }
        }
    }

    public void RefreshStatsUponRespawning(){
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().ResetStats();
        gameObject.transform.GetChild(0).GetComponent<MovementSystem>().ResetStats();
    }

    private void IncreaseScore(int value){
        characterStats.IncreaseScore(value);
        OnPlayerScoreChanged?.Invoke(gameObject);
    }

    private void PlayerScoredKill(GameObject character){ //other player
        character.transform.parent.GetComponent<CharacterStats>().IncreaseKills();
        character.transform.parent.GetComponent<CharacterEvents>().UpdateKills();
    }

    private void UpdateKills(){ //calling this directly on the above method doesn't work 
        OnPlayerScoredKill?.Invoke(gameObject.transform.GetChild(0).gameObject); //other player sends their character
    }

    private void PlayerDied(GameObject character){ //themselves
        character.transform.parent.GetComponent<CharacterStats>().IncreaseDeaths();
        if(!character.transform.parent.GetComponent<CharacterStats>().unlimitedLives){
            character.transform.parent.GetComponent<CharacterStats>().DecreaseLives();
        }
        Instantiate(GameManager.instance.DeathSpot, character.transform.position, Quaternion.Euler(0, 0, 0), this.transform);
        OnPlayerDied?.Invoke(character);
    }

    private void PlayerBorn(GameObject character){ //themselves
        OnPlayerBorn?.Invoke(character);
    }

    private void PlayerWasDamaged(float damage){
        OnPlayerWasDamaged?.Invoke(damage);
    }

    private void PlayerWasHealed(float heal){
        OnPlayerWasHealed?.Invoke(heal);
    }

    private void PlayerHealthUpdated(float currentHealth){
        OnPlayerHealthUpdated?.Invoke(currentHealth);
    }

    private void PlayerStaminaUpdated(float currentStamina){
        OnPlayerStaminaUpdated?.Invoke(currentStamina);
    }

}