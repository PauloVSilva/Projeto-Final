using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public abstract class Item : Entity{
    [SerializeField] public ItemScriptableObject item;
    [SerializeField] public bool canBePickedUp;
    [SerializeField] protected float pickUpRadius;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected bool isBlinking;
    [SerializeField] protected SphereCollider itemCollider;
    [SerializeField] protected Rigidbody itemRigidbody;
    [SerializeField] protected Renderer objectRenderer;

    protected virtual void Awake(){
        InitializeItemVariables();
    }

    protected virtual void InitializeItemVariables(){
        itemCollider = GetComponent<SphereCollider>();
        itemRigidbody = GetComponent<Rigidbody>();
        
        objectRenderer.enabled = true;
        itemCollider.isTrigger = true;
        itemCollider.enabled = true;
        itemCollider.radius = pickUpRadius;

        itemRigidbody.velocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        isPooled = false;
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

    protected override void Update(){
        CollectableBehaviour();
        AgeBehaviour();
    }

    protected virtual void CollectableBehaviour(){
        if(canBePickedUp){
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
    }

    protected override void AgeBehaviour(){
        if(!persistenceRequired && canBePickedUp){
            if(age >= 0){
                age += Time.deltaTime;
            }
            if (age > maxAge - 10 && !isBlinking){
                isBlinking = true;
                StartCoroutine(Flash(0.25f));
            }
            if(age > maxAge){
                MaxAgeReached();
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
