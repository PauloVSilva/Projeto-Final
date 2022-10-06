using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthSystem : MonoBehaviour{
    public event System.Action<float> OnDamaged;
    public event System.Action OnDeath;

    //VARIABLES THAT WILL COME FROM SOMEWHERE ELSE
    public abstract float MaxHealth {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public abstract float CurrentHealth {get; protected set;}
    public abstract bool IsAlive {get; protected set;}
    public abstract bool IsInvulnerable {get; protected set;}

    public void Kill(){
        TakeDamage(GameManager.instance.gameObject, float.MaxValue);
    }

    public abstract void InitializeVariables();

    public abstract void TakeDamage(GameObject damageSource, float damageTaken);

    public virtual void Heal(float heal){}

    public abstract void Die(GameObject damageSource);

    protected void InvokeOnDamaged(float _damage){
        OnDamaged?.Invoke(_damage);
    }

    protected void InvokeOnDeath(){
        OnDeath?.Invoke();
    }

}
