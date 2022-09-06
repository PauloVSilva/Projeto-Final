using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour{
    public ProjectileScriptableObject ProjectileToCast;

    private SphereCollider myCollider;
    private Rigidbody myRigidbody;
    [SerializeField] private GameObject playerOfOrigin;
    [SerializeField] private GameObject characterOfOrigin;
    [SerializeField] private GameObject weaponOfOrigin;

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

        playerOfOrigin = this.transform.parent.parent.parent.gameObject;
        characterOfOrigin = this.transform.parent.parent.gameObject;
        weaponOfOrigin = this.transform.parent.gameObject;

        this.transform.parent = null;
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            //HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            //enemyHealth.TakeDamage(ProjectileToCast.DamageAmount);
            other.GetComponent<HealthSystem>().TakeDamage(characterOfOrigin, ProjectileToCast.DamageAmount);
        }

        Destroy(this.gameObject);
    }
}
