using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour{
    public ProjectileScriptableObject ProjectileToCast;

    private SphereCollider myCollider;
    private Rigidbody myRigidbody;

    void Awake(){
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = ProjectileToCast.ProjectileRadius;

        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.isKinematic = true;

        Destroy(this.gameObject, ProjectileToCast.LifeTime);
    }

    void Update(){
        if(ProjectileToCast.Speed > 0){
            transform.Translate(Vector3.forward * ProjectileToCast.Speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            enemyHealth.TakeDamage(ProjectileToCast.DamageAmount);
        }

        Destroy(this.gameObject);
    }
}
