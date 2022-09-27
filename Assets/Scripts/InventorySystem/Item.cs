using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class Item : MonoBehaviour{
    [SerializeField] public ItemScriptableObject item;
    [SerializeField] public bool persistenceRequired;
    [SerializeField] protected float age;
    [SerializeField] protected int maxAge;
    [SerializeField] public bool canBePickedUp;
    [SerializeField] protected float pickUpRadius;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected bool isBlinking;
    [SerializeField] protected SphereCollider myCollider;
    [SerializeField] protected Renderer objectRenderer;

    protected virtual void Awake(){
        InitializeVariables();
    }

    protected virtual void InitializeVariables(){
        objectRenderer.enabled = true;
        myCollider.isTrigger = true;
        myCollider.radius = pickUpRadius;

        age = 0;
        canBePickedUp = false;
        StartCoroutine(CanBePickedUpDelay());
        pickUpRadius = 1.5f;
        isBlinking = false;
    }
    protected IEnumerator CanBePickedUpDelay(){
        yield return new WaitForSeconds(1f);
        canBePickedUp = true;
    }

    protected virtual void Update(){
        CollectableBehaviour();
        AgeBehaviour();
    }

    protected virtual void CollectableBehaviour(){
        if(canBePickedUp){
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
    }

    protected virtual void AgeBehaviour(){
        if(!persistenceRequired && canBePickedUp){
            if(age >= 0){
                age += Time.deltaTime;
            }
            if (age > maxAge - 10 && !isBlinking){
                isBlinking = true;
                StartCoroutine(Flash(0.25f));
            }
            if(age > maxAge){
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator Flash(float time){
        yield return new WaitForSeconds(time);
        if(isBlinking){
            objectRenderer.enabled = !objectRenderer.enabled;
            if (age < maxAge - 3){
                StartCoroutine(Flash(0.25f));
            }
            else {
                StartCoroutine(Flash(0.1f));
            }
        }
        else{
            objectRenderer.enabled = true;
        }
    }

    protected virtual void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }

    
    public bool CanBePickedUp(){
        return canBePickedUp;
    }
}
