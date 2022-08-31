using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour{
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthRegenRate;
    public bool isAlive = true;

    private void Awake(){
        currentHealth = maxHealth;
    }

    private void Update(){
        if(currentHealth <= 0){
            //print("died");
            isAlive = false;
        }
    }

    public void TakeDamage(float damageTaken){
        currentHealth -= damageTaken;
        print(currentHealth);
    }

    public void Heal(float heal){
        currentHealth += heal;
        print(currentHealth);
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
    }

    public void Kill(){
        currentHealth = 0;
    }

    public void Respawn(){
        currentHealth = maxHealth;
        isAlive = true;
    }

}
