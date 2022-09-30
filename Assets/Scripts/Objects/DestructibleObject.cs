using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour{
    [SerializeField] private DestructibleObjectHealthSystem healthSystem;
    [SerializeField] private Inventory inventory;
    [SerializeField] private DamageFeedback damageFeedback;
    [SerializeField] public int MaxHealth = 1;
    //[SerializeField] public int MaxHealth {get; protected set;}

    private void Awake(){
        //MaxHealth = 300;
    }

    private void Start(){
        healthSystem.OnDeath += ObjectDestroyed;
        healthSystem.OnDamaged += ObjectTookDamage;
    }

    private void ObjectDestroyed(){
        inventory.DropAllInventory();
    }

    private void ObjectTookDamage(float _damage){
        damageFeedback.DisplayDamageTaken(_damage);
    }
}
