using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthSystem : MonoBehaviour{
    public event System.Action<float> OnDamaged;

    //VARIABLES THAT WILL COME FROM SOMEWHERE ELSE
    public abstract float MaxHealth {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public abstract float CurrentHealth {get; protected set;}
    public abstract bool IsAlive {get; protected set;}
    public abstract bool IsInvulnerable {get; protected set;}

    public void Initialize(){
        InitializeVariables();
        StartCoroutine(OnEntityBornDelay());
    }

    public void ResetStats(){
        InitializeVariables();
    }

    public void Kill(){
        TakeDamage(GameManager.instance.gameObject, float.MaxValue);
    }

    protected abstract void InitializeVariables();
    protected virtual IEnumerator OnEntityBornDelay(){
        yield return new WaitForSeconds(0.05f);
        //holy shirt that's some ugly ash code
    }

    public virtual void TakeDamage(GameObject damageSource, float damageTaken){
    }

    public virtual void Heal(float heal){
    }

    public virtual void Die(GameObject damageSource){
    }


    protected void InvokeOnDamaged(float _damage){
        OnDamaged?.Invoke(_damage);
    }

}
