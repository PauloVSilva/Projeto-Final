using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class Coin : MonoBehaviour{
    public float age = 0;
    public bool canBePickedUp = true;

    public float pickUpRadius = 1.5f;
    public float rotationSpeed = 10f;
    public int value = 1;
    private bool isBlinking = false;
    SphereCollider myCollider;

    public Renderer renderer;

    private void Awake(){
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = pickUpRadius;
    }

    private void Start(){
        renderer = this.transform.GetChild(0).GetComponent<Renderer>();
        renderer.enabled = true;
    }

    private void Update(){
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        age += Time.deltaTime;
        AgeBehaviour();
    }

    private void AgeBehaviour(){
        if (age > 20 && !isBlinking){
            isBlinking = true;
            StartCoroutine(Flash(0.25f));
        }

        if(age > 30){
            Destroy(this.gameObject);
        }
    }

    IEnumerator Flash(float time){
        yield return new WaitForSeconds(time);
        renderer.enabled = !renderer.enabled;
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

    public void OnTriggerEnter(Collider other){
        PickUpCoin(other);
    }

    public void PickUpCoin(Collider other){
        if(canBePickedUp){
            if(other.gameObject.CompareTag("Player")){
                PlayerController player = other.gameObject.GetComponent<PlayerController>();
                if(player != null){
                    player.IncreaseScore(value);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
