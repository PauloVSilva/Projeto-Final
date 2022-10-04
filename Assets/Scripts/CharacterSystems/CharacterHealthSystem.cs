using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthSystem : HealthSystem{
    private CharacterManager characterManager;

    //VARIABLES THAT WILL COME FROM CHARACTER SCRIPTABLE OBJECT
    public override float MaxHealth {get; protected set;}
    public float HealthRegenRate {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public override float CurrentHealth {get; protected set;}
    public float HealthRegenCooldown {get; protected set;}
    public bool CanRegenHealth {get; protected set;}
    public override bool IsAlive {get; protected set;}
    public override bool IsInvulnerable {get; protected set;}

    private void Awake(){
        InitializeComponents();
    }

    private void InitializeComponents(){
        characterManager = GetComponent<CharacterManager>();
    }

    private void Update(){
        RegenerateHealth();
    }

    public void Initialize(){
        InitializeVariables();
        StartCoroutine(OnEntityBornDelay());
        IEnumerator OnEntityBornDelay(){
            yield return new WaitForSeconds(0.05f);
            characterManager.PlayerBorn(gameObject);
        }
    }

    protected override void InitializeVariables(){        
        MaxHealth = characterManager.Character.maxHealth;
        HealthRegenRate = characterManager.Character.healthRegenRate;

        CurrentHealth = MaxHealth;
        IsAlive = true;
        IsInvulnerable = false;
        SendHealthUpdateEvent();
    }

    public void ResetStats(){
        InitializeVariables();
    }

    private void RegenerateHealth(){
        if(IsAlive && CanRegenHealth){
            CurrentHealth = Math.Min(CurrentHealth += HealthRegenRate * Time.deltaTime, MaxHealth);
            SendHealthUpdateEvent();
        }
        if(HealthRegenCooldown > 0){
            HealthRegenCooldown -= Time.deltaTime;
        }
        if(HealthRegenCooldown <= 0){
            HealthRegenCooldown = 0;
            CanRegenHealth = true;
        }
    }

    private void SendHealthUpdateEvent(){
        characterManager.PlayerHealthUpdated(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(float damageTaken){
        TakeDamage(null, damageTaken);
    }

    public override void TakeDamage(GameObject damageSource, float damageTaken){
        if(!IsInvulnerable){
            CurrentHealth = Math.Max(CurrentHealth -= damageTaken, 0);
            if(CurrentHealth <= 0){
                Die(damageSource);
            }
            characterManager.PlayerWasDamaged(damageTaken);
            HealthRegenCooldown = 1f;
            CanRegenHealth = false;
            SendHealthUpdateEvent();
        }
    }

    public override void Heal(float heal){
        CurrentHealth = Math.Min(CurrentHealth += heal, MaxHealth);
        characterManager.PlayerWasHealed(heal);
        SendHealthUpdateEvent();
    }

    public override void Die(GameObject damageSource){
        IsAlive = false;
        IsInvulnerable = true;
        CurrentHealth = 0;
        characterManager.PlayerDied(gameObject);
        if(damageSource.CompareTag("Player")){
            damageSource.transform.GetComponent<CharacterManager>().PlayerScoredKill(damageSource);
        }
    }
}
