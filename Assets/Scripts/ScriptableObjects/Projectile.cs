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
        //myRigidbody.isKinematic = true;

        Destroy(this.gameObject, ProjectileToCast.LifeTime);
    }

    void Start(){
        myRigidbody.AddForce(transform.forward * ProjectileToCast.Speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            //HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            //enemyHealth.TakeDamage(ProjectileToCast.DamageAmount);
            GameObject damageSource = this.transform.parent.parent.parent.gameObject; //this.transform.root.gameObject
            other.GetComponent<HealthSystem>().TakeDamage(damageSource, ProjectileToCast.DamageAmount);
        }

        Destroy(this.gameObject);
    }
}
