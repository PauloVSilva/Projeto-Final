using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectHealthSystem : HealthSystem{
    [SerializeField] private Inventory inventory;
    //VARIABLES THAT WILL COME FROM SOMEWHERE
    public override float MaxHealth {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public override float CurrentHealth {get; protected set;}
    public override bool IsAlive {get; protected set;}
    public override bool IsInvulnerable {get; protected set;}

    protected void Start(){
        InitializeVariables();
    }

    protected override void InitializeVariables(){
        MaxHealth = 300f;
        CurrentHealth = MaxHealth;
        IsAlive = true;
        IsInvulnerable = false;
    }

    public override void TakeDamage(GameObject damageSource, float damageTaken){
        CurrentHealth -= damageTaken;
        if(CurrentHealth <= 0){
            Die(damageSource);
        }
        InvokeOnDamaged(damageTaken);
    }

    public override void Heal(float heal){
        CurrentHealth += heal;
        if(CurrentHealth > MaxHealth){
            CurrentHealth = MaxHealth;
        }
    }

    public override void Die(GameObject damageSource){
        IsAlive = false;
        CurrentHealth = 0;
        inventory.DropAllInventory();
        Destroy(gameObject);
    }
}
