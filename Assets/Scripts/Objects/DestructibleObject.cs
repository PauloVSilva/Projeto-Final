using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class DestructibleObject : MonoBehaviour{
    [SerializeField] protected DestructibleObjectHealthSystem healthSystem;
    [SerializeField] protected Inventory inventory;
    [SerializeField] protected DamageFeedback damageFeedback;
    [SerializeField] protected FloatingHealthBar floatingHealthBar;
    public int MaxHealth = 1;
    //public int MaxHealth {get; protected set;}

    [SerializeField] private AudioSource audioData;

    protected virtual void Awake(){
        //MaxHealth = 300;
        InitializeComponents();
        InitializeVariables();
        SubscribeToEvents();
    }

    protected void Start(){
        InitializeOthers();
    }

    protected virtual void InitializeComponents(){
        healthSystem = GetComponent<DestructibleObjectHealthSystem>();
        inventory = GetComponent<Inventory>();
        //damageFeedback = GetComponent<DamageFeedback>();
        //floatingHealthBar = GetComponent<FloatingHealthBar>();

        damageFeedback = GetComponentInChildren<DamageFeedback>();
        floatingHealthBar = GetComponentInChildren<FloatingHealthBar>();

        audioData = GetComponent<AudioSource>();
    }

    protected virtual void InitializeVariables(){}

    protected virtual void SubscribeToEvents(){
        healthSystem.OnDeath += ObjectDestroyed;
        healthSystem.OnDamaged += ObjectTookDamage;
    }

    protected void InitializeOthers(){
        floatingHealthBar?.SetMaxHealth(healthSystem.MaxHealth);
    }

    protected virtual void ObjectTookDamage(float _damage){
        damageFeedback?.DisplayDamageTaken(_damage);
        floatingHealthBar?.UpdateHealthBar(healthSystem.CurrentHealth);

        //play destruction damage
        audioData.Play(0);
    }

    protected virtual void ObjectDestroyed(){
        //play destruction damage
        audioData.Play(0);

        inventory?.DropAllInventory();
        Destroy(gameObject);
    }
}
