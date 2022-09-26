using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item{
    [SerializeField] private float age;
    [SerializeField] public bool canBePickedUp;
    [SerializeField] private float pickUpRadius;
    [SerializeField] private float rotationSpeed;
    [SerializeField] public int value;
    [SerializeField] private bool isBlinking;
    [SerializeField] private SphereCollider myCollider;
    [SerializeField] private Renderer objectRenderer;

    private void Awake(){
        InitializeVariables();
    }

    private void Start(){
        objectRenderer = this.transform.GetChild(0).GetComponent<Renderer>();
        objectRenderer.enabled = true;
    }

    private void Update(){
        CollectableBehavior();
    }

    private void InitializeVariables(){
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = pickUpRadius;

        age = 0;
        canBePickedUp = true;
        pickUpRadius = 1.5f;
        //rotationSpeed = 10f;
        //value = 1;
        isBlinking = false;
    }

    private void CollectableBehavior(){
        if(canBePickedUp){
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
            age += Time.deltaTime;
            if (age > 20 && !isBlinking){
                isBlinking = true;
                StartCoroutine(Flash(0.25f));
            }
            if(age > 30){
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator Flash(float time){
        yield return new WaitForSeconds(time);
        objectRenderer.enabled = !objectRenderer.enabled;
        if (age < 27){
            StartCoroutine(Flash(0.25f));
        }
        else {
            StartCoroutine(Flash(0.1f));
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}
