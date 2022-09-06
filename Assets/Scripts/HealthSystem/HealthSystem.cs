using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;

    //VARIABLES THAT WILL COME FROM CHARACTERSTATS
    public float MaxHealth {get; protected set;}
    public float HealthRegenRate {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public float CurrentHealth {get; protected set;}
    public bool IsAlive {get; protected set;}

    //EVENTS
    public event System.Action<GameObject> OnEntityScoredKill;
    public event System.Action<GameObject> OnEntityDied;
    public event System.Action<GameObject> OnEntityBorn;
    public event System.Action<float> OnEntityTookDamage;
    public event System.Action<float> OnEntityHealed;
    public event System.Action<float> OnEntityHealthUpdated;

    private void Start(){
        characterStats = gameObject.transform.parent.GetComponent<CharacterStats>();
        InitializeVariables();
    }

    private void Update(){
        RegenerateHealth();
    }

    private void InitializeVariables(){
        MaxHealth = characterStats.MaxHealth;
        HealthRegenRate = characterStats.HealthRegenRate;

        CurrentHealth = MaxHealth;
        IsAlive = true;
        StartCoroutine(OnEntityBornDelay());
        OnEntityHealthUpdated?.Invoke(CurrentHealth);
    }

    IEnumerator OnEntityBornDelay(){
        yield return new WaitForSeconds(0.05f);
        OnEntityBorn?.Invoke(gameObject);
    }

    public void ResetStats(){
        InitializeVariables();
    }

    private void RegenerateHealth(){
        if(CurrentHealth < MaxHealth){
            CurrentHealth += HealthRegenRate * Time.deltaTime;
            OnEntityHealthUpdated?.Invoke(CurrentHealth);
        }
        if(CurrentHealth > MaxHealth){
            CurrentHealth = MaxHealth;
        }
    }

    public void TakeDamage(GameObject damageSource, float damageTaken){
        CurrentHealth -= damageTaken;
        if(CurrentHealth <= 0){
            Die(damageSource);
        }
        OnEntityTookDamage?.Invoke(damageTaken);
        OnEntityHealthUpdated?.Invoke(CurrentHealth);
    }

    public void Heal(float heal){
        CurrentHealth += heal;
        if(CurrentHealth > MaxHealth){
            CurrentHealth = MaxHealth;
        }
        OnEntityHealed?.Invoke(heal);
        OnEntityHealthUpdated?.Invoke(CurrentHealth);
    }

    public void Kill(){
        TakeDamage(GameManager.instance.gameObject, float.MaxValue);
    }

    public void Die(GameObject damageSource){
        IsAlive = false;
        CurrentHealth = 0;
        OnEntityDied?.Invoke(gameObject);
        if(damageSource.CompareTag("Player")){
            OnEntityScoredKill?.Invoke(damageSource);
        }
    }

}
