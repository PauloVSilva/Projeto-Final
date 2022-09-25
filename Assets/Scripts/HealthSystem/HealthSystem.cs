using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthSystem : MonoBehaviour{

    //VARIABLES THAT WILL COME FROM SOMEWHERE ELSE
    public abstract float MaxHealth {get; protected set;}

    //VARIABLES FOR INTERNAL USE
    public abstract float CurrentHealth {get; protected set;}
    public abstract bool IsAlive {get; protected set;}

    protected void Start(){
        InitializeVariables();
    }

    public void ResetStats(){
        InitializeVariables();
    }

    public void Kill(){
        TakeDamage(GameManager.instance.gameObject, float.MaxValue);
    }

    protected virtual void InitializeVariables(){
    }

    public virtual void TakeDamage(GameObject damageSource, float damageTaken){
    }

    public virtual void Heal(float heal){
    }

    public virtual void Die(GameObject damageSource){
    }

}
