using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEvents : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] public GameObject characterObject;

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

    public void SetEvents(){
        characterObject = GetComponent<CharacterSelection>().characterObject;
        characterStats = gameObject.GetComponent<CharacterStats>();
        SubscribeToOwnEvents();
    }

    private void SubscribeToOwnEvents(){ //subscribe to child system events like health or movement
        //HEALTH EVENTS
        characterObject.GetComponent<HealthSystem>().OnEntityDied += PlayerDied;
        characterObject.GetComponent<HealthSystem>().OnEntityScoredKill += PlayerScoredKill;
        characterObject.GetComponent<HealthSystem>().OnEntityBorn += PlayerBorn;
        characterObject.GetComponent<HealthSystem>().OnEntityTookDamage += PlayerWasDamaged;
        characterObject.GetComponent<HealthSystem>().OnEntityHealed += PlayerWasHealed;
        characterObject.GetComponent<HealthSystem>().OnEntityHealthUpdated += PlayerHealthUpdated;
        //MOVEMENT EVENTS
        characterObject.GetComponent<MovementSystem>().OnEntityStaminaUpdated += PlayerStaminaUpdated;
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
        GetComponent<CharacterStats>().ResetScores();
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
            if(!character.transform.parent.GetComponent<CharacterStats>().isArmed && _gameObject.GetComponent<Weapon>().CanBePickedUp()){
                _gameObject.GetComponent<Weapon>().PickUpWeapon(characterObject.gameObject);
            }
        }
    }

    public void RefreshStatsUponRespawning(){
        characterObject.GetComponent<HealthSystem>().ResetStats();
        characterObject.GetComponent<MovementSystem>().ResetStats();
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
        OnPlayerScoredKill?.Invoke(characterObject.gameObject); //other player sends their character
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
