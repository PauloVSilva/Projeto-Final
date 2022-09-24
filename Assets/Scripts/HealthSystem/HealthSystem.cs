using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterEvents characterEvents;

    //VARIABLES THAT WILL COME FROM CHARACTERSTATS
    public float MaxHealth {get; protected set;}
    public float HealthRegenRate {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public float CurrentHealth {get; protected set;}
    public bool IsAlive {get; protected set;}

    private void Start(){
        InitializeVariables();
    }

    private void Update(){
        RegenerateHealth();
    }

    private void InitializeVariables(){
        characterStats = gameObject.transform.parent.GetComponent<CharacterStats>();
        characterEvents = gameObject.transform.parent.GetComponent<CharacterEvents>();
        
        MaxHealth = characterStats.MaxHealth;
        HealthRegenRate = characterStats.HealthRegenRate;

        CurrentHealth = MaxHealth;
        IsAlive = true;
        StartCoroutine(OnEntityBornDelay());
        SendHealthUpdateEvent();
    }

    IEnumerator OnEntityBornDelay(){
        yield return new WaitForSeconds(0.05f);
        characterEvents.PlayerBorn(gameObject);
    }

    public void ResetStats(){
        InitializeVariables();
    }

    private void RegenerateHealth(){
        if(IsAlive){
            CurrentHealth = Math.Min(CurrentHealth += HealthRegenRate * Time.deltaTime, MaxHealth);
            SendHealthUpdateEvent();
        }
    }

    public void TakeDamage(GameObject damageSource, float damageTaken){
        CurrentHealth -= damageTaken;
        if(CurrentHealth <= 0){
            Die(damageSource);
        }
        characterEvents.PlayerWasDamaged(damageTaken);
        SendHealthUpdateEvent();
    }

    public void Heal(float heal){
        CurrentHealth += heal;
        if(CurrentHealth > MaxHealth){
            CurrentHealth = MaxHealth;
        }
        characterEvents.PlayerWasHealed(heal);
        SendHealthUpdateEvent();
    }

    public void Kill(){
        TakeDamage(GameManager.instance.gameObject, float.MaxValue);
    }

    public void Die(GameObject damageSource){
        IsAlive = false;
        CurrentHealth = 0;
        characterEvents.PlayerDied(gameObject);
        if(damageSource.CompareTag("Character")){
            damageSource.transform.parent.GetComponent<CharacterEvents>().PlayerScoredKill(damageSource);
        }
    }

    private void SendHealthUpdateEvent(){
        characterEvents.PlayerHealthUpdated(CurrentHealth, MaxHealth);
    }

}
