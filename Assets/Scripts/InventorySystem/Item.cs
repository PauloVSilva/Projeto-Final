using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public abstract class Item : Entity{
    public ItemScriptableObject item {get; protected set;}
    [SerializeField] protected bool canBePickedUp;
    protected bool canBeStored;
    protected float pickUpRadius;
    protected float rotationSpeed;
    protected bool isBlinking;
    protected SphereCollider itemCollider;
    protected Rigidbody itemRigidbody;
    [SerializeField] protected Renderer objectRenderer;

    public bool CanBePickedUp => canBePickedUp;
    public bool CanBeStored => canBeStored;

    protected virtual void Awake(){
        SetScriptableObjectVariables();
        InitializeItemComponents();
        InitializeItemVariables();
    }

    private void InitializeItemComponents(){
        itemCollider = GetComponent<SphereCollider>();
        itemRigidbody = GetComponent<Rigidbody>();

        objectRenderer.enabled = true;
        itemCollider.isTrigger = true;
        itemCollider.enabled = true;
        itemCollider.radius = pickUpRadius;
        itemRigidbody.velocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        isPooled = false;
    }

    protected virtual void SetScriptableObjectVariables(){
        maxAge = item.maxAge;
        canBeStored = item.canBeStored;
        pickUpRadius = item.pickUpRadius;
        rotationSpeed = item.rotationSpeed;
    }

    protected virtual void InitializeItemVariables(){
        age = 0;
        canBePickedUp = false;
        StartCoroutine(CanBePickedUpDelay());
        isBlinking = false;
    }
    
    protected IEnumerator CanBePickedUpDelay(){
        yield return new WaitForSeconds(0.5f);
        canBePickedUp = true;
    }

    protected override void Update(){
        AgeBehaviour();
        CollectableBehaviour();
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
                Despawn();
            }
        }
    }

    protected virtual void CollectableBehaviour(){
        if(canBePickedUp){
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
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
}
