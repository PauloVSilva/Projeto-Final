using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour{
    [SerializeField] protected DestructibleObjectHealthSystem healthSystem;
    [SerializeField] protected Inventory inventory;
    [SerializeField] protected DamageFeedback damageFeedback;
    [SerializeField] public int MaxHealth = 1;
    //[SerializeField] public int MaxHealth {get; protected set;}

    protected virtual void Awake(){
        //MaxHealth = 300;
    }

    protected virtual void Start(){
        healthSystem.OnDeath += ObjectDestroyed;
        healthSystem.OnDamaged += ObjectTookDamage;
    }

    protected virtual void ObjectDestroyed(){
        inventory.DropAllInventory();
    }

    protected virtual void ObjectTookDamage(float _damage){
        damageFeedback.DisplayDamageTaken(_damage);
    }
}
