using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEvents : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;
    

    //IMPORTANT
    //RESPAWNING STILL HASN'T BEEN IMPLEMENTED IN THIS SCRIPT
    //THIS RESPONSABILITY WILL PROBABLY BE SHIFTED TO GAMEMANAGER


    //EVENTS THAT WILL BE SENT TO OTHER CLASSES
    public event System.Action<int> OnPlayerScoreChanged;
    public event System.Action<GameObject> OnPlayerScoredKill;
    public event System.Action<GameObject> OnPlayerDied;
    public event System.Action<GameObject> OnPlayerBorn;
    public event System.Action<float> OnPlayerWasDamaged;
    public event System.Action<float> OnPlayerWasHealed;
    public event System.Action<float> OnPlayerHealthUpdated; 
    public event System.Action<float> OnPlayerStaminaUpdated; 
    //public event System.Action<int> OnWeaponFired;

    private void Awake() {
        characterStats = gameObject.transform.parent.GetComponent<CharacterStats>();
    }

    private void Start() {
        SubscribeToOwnEvents();
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

    public void UnsubscribeFromPlayerEvents(){ //allow managers to subscribe to this class' events
    }


    public void FilterCollision(GameObject player, GameObject gameObject){
        if(gameObject.CompareTag("Coin")){
            if(gameObject.GetComponent<Coin>().canBePickedUp){
                IncreaseScore(gameObject.GetComponent<Coin>().value);
                Destroy(gameObject);
            }
        }
        if(gameObject.CompareTag("Instadeath")){
            player.GetComponent<HealthSystem>().Kill();
        }
    }

    public void RefreshStatsUponRespawning(){
        gameObject.transform.GetChild(0).GetComponent<HealthSystem>().ResetStats();
        gameObject.transform.GetChild(0).GetComponent<MovementSystem>().ResetStats();
    }

    private void IncreaseScore(int value){
        characterStats.IncreaseScore(value);
        OnPlayerScoreChanged?.Invoke(characterStats.score);
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
        OnPlayerDied?.Invoke(character);
    }

    private void PlayerBorn(GameObject character){ //themselves
        character.transform.parent.GetComponent<CharacterStats>().DecreaseLives();
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
