using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class HealthSystem : MonoBehaviour{
    public float maxHealth;
    public float currentHealth;
    public float healthRegenRate;
    public bool isAlive;

    public event Action<GameObject> OnEntityDied;
    public event Action<GameObject> OnEntityScoredKill;
    public event Action<GameObject> OnEntityBorn;
    public event Action<float> OnEntityTookDamage;
    public event Action<float> OnEntityHealed;
    public event Action<float> OnEntityHealthUpdated;

    private void Awake(){
        Spawn();
    }

    private void Update(){
        Regenerate();
    }

    private void Regenerate(){
        if(currentHealth < maxHealth){
            currentHealth += healthRegenRate * Time.deltaTime;
            OnEntityHealthUpdated?.Invoke(currentHealth);
        }
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(GameObject damageSource, float damageTaken){
        currentHealth -= damageTaken;
        if(currentHealth <= 0){
            Die(damageSource);
        }
        OnEntityTookDamage?.Invoke(damageTaken);
        OnEntityHealthUpdated?.Invoke(currentHealth);
    }

    public void Heal(float heal){
        currentHealth += heal;
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
        OnEntityHealed?.Invoke(heal);
        OnEntityHealthUpdated?.Invoke(currentHealth);
    }

    public void Kill(){
        TakeDamage(GameManager.instance.gameObject, float.MaxValue);
    }

    public void Die(GameObject damageSource){
        isAlive = false;
        currentHealth = 0;
        Instantiate(GameManager.instance.DeathSpot, this.transform.position, Quaternion.Euler(0, 0, 0));
        OnEntityDied?.Invoke(gameObject);
        if(damageSource.CompareTag("Player")){
            OnEntityScoredKill?.Invoke(damageSource);
        }
    }

    public void Spawn(){
        isAlive = true;
        currentHealth = maxHealth;
        healthRegenRate = 2f;
        maxHealth = 100f;
        OnEntityBorn?.Invoke(gameObject);
        OnEntityHealthUpdated?.Invoke(currentHealth);
    }
}
