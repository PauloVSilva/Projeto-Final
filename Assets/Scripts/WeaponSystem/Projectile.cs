using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionType{contact, explosive, passThrough}

public class Projectile : IPooledObjects{
//public class Projectile : MonoBehaviour{
    public ProjectileScriptableObject ProjectileToCast;

    //ATTRIBUTES FROM SCRIPTABLE OBJECT
    [SerializeField] public GameObject projectileModel;
    [SerializeField] public Sprite sprite;
    [SerializeField] public string projectileName;
    [SerializeField] public CollisionType collisionType;
    [SerializeField] public float damageAmount;
    [SerializeField] public float cost;
    [SerializeField] public float lifeTime;
    [SerializeField] public float speed;

    //VARIABLES FOR INTERNAL USE
    [SerializeField] private bool canDamage;

    //OTHER ATTRIBUTES
    [SerializeField] private BoxCollider myCollider;
    [SerializeField] private Rigidbody myRigidbody;
    [SerializeField] private GameObject playerOfOrigin;
    [SerializeField] private GameObject weaponOfOrigin;

    private void Awake(){
        GetScriptableObjectVariables();

        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
        myRigidbody = GetComponent<Rigidbody>();
        disableCoroutineIsRunning = false;
    }

    private void GetScriptableObjectVariables(){
        sprite = ProjectileToCast.sprite;
        projectileName = ProjectileToCast.projectileName;
        collisionType = (CollisionType)ProjectileToCast.collisionType;
        damageAmount = ProjectileToCast.damageAmount;
        cost = ProjectileToCast.cost;
        lifeTime = ProjectileToCast.lifeTime;
        speed = ProjectileToCast.speed;
    }

    public override void OnObjectSpawn(){ //replaces Start()
        if(disableCoroutineIsRunning){
            DisableCoroutine(ProjectileToCast.lifeTime);
        }

        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.AddForce(transform.forward * ProjectileToCast.speed, ForceMode.Impulse);

        weaponOfOrigin = this.transform.parent.gameObject;
        playerOfOrigin = weaponOfOrigin.transform.parent.gameObject;

        this.transform.parent = ObjectPooler.instance.transform;

        canDamage = true;

        StartCoroutine(DisableObject(ProjectileToCast.lifeTime));
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.GetComponent<HealthSystem>() != null && canDamage){
            other.GetComponent<HealthSystem>().TakeDamage(playerOfOrigin, ProjectileToCast.damageAmount);
            this.gameObject.SetActive(false);
        }
        canDamage = false;
    }
}
