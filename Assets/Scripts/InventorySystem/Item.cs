using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class Item : MonoBehaviour{
    [SerializeField] public ItemScriptableObject item;
    [SerializeField] bool persistenceRequired;
    [SerializeField] protected float age;
    [SerializeField] protected int maxAge;
    [SerializeField] public bool canBePickedUp;
    [SerializeField] protected float pickUpRadius;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected bool isBlinking;
    [SerializeField] protected SphereCollider myCollider;
    [SerializeField] protected Renderer objectRenderer;

    protected void Awake(){
        InitializeVariables();
    }

    protected void Start(){
        objectRenderer.enabled = true;
    }

    protected void InitializeVariables(){
        myCollider.isTrigger = true;
        myCollider.radius = pickUpRadius;

        age = 0;
        canBePickedUp = false;
        StartCoroutine(CanBePickedUpDelay());
        IEnumerator CanBePickedUpDelay(){
            yield return new WaitForSeconds(2f);
            canBePickedUp = true;
        }
        pickUpRadius = 1.5f;
        isBlinking = false;
    }

    protected void Update(){
        CollectableBehaviuor();
        AgeBehaviour();
    }

    protected void CollectableBehaviuor(){
        if(canBePickedUp){
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
    }

    protected void AgeBehaviour(){
        if(!persistenceRequired && canBePickedUp){
            age += Time.deltaTime;
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
        objectRenderer.enabled = !objectRenderer.enabled;
        if (age < maxAge - 3){
            StartCoroutine(Flash(0.25f));
        }
        else {
            StartCoroutine(Flash(0.1f));
        }
    }

    protected void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}
