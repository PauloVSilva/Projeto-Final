using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private bool isAlive = true;

    private void Awake(){
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageTaken){
        currentHealth -= damageTaken;
        print(currentHealth);
        if(currentHealth <= 0){
            //print("died");
            isAlive = false;
        }
    }

    public void Heal(float heal){
        currentHealth += heal;
        print(currentHealth);
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
    }

}
