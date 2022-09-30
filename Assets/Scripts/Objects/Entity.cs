using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour{
    [SerializeField] protected bool isPooled;
    [SerializeField] public bool persistenceRequired;
    [SerializeField] protected float age;
    [SerializeField] protected float maxAge;

    protected abstract void Update();

    protected virtual void AgeBehaviour(){
        if(!persistenceRequired){
            if(age >= 0){
                age += Time.deltaTime;
            }
            if(age > maxAge){
                MaxAgeReached();
            }
        }
    }

    protected virtual void MaxAgeReached(){
        if(isPooled){
            this.gameObject.SetActive(false);
        }
        else{
            Destroy(this.gameObject);
        }
    }
}
