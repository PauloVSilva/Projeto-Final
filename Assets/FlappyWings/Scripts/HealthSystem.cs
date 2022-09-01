using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class HealthSystem : MonoBehaviour{
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthRegenRate;
    public float timeToRespawn = 3f;
    public int extraLifes = 1;
    public bool isAlive = true;

    public static event Action<GameObject> OnPlayerDied;
    public static event Action<GameObject> OnPlayerReborn;


    private void Awake(){
        currentHealth = maxHealth;
    }

    private void Start(){
        OnPlayerReborn?.Invoke(gameObject);
    }

    public void TakeDamage(float damageTaken){
        currentHealth -= damageTaken;
        //print(currentHealth);
        if(currentHealth <= 0){
            //print("died");
            isAlive = false;
            OnPlayerDied?.Invoke(gameObject);
        }
    }

    public void Heal(float heal){
        currentHealth += heal;
        //print(currentHealth);
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
    }

    public void Kill(){
        TakeDamage(float.MaxValue);
    }

    public void Respawn(){
        currentHealth = maxHealth;
        isAlive = true;
    }

}
