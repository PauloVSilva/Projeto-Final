using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionType{contact, explosive, passThrough}

public class Projectile : MonoBehaviour, IPooledObjects{
//public class Projectile : MonoBehaviour{
    public ProjectileScriptableObject ProjectileToCast;

    //ATTRIBUTES FROM SCRIPTABLE OBJECT
    [SerializeField] public GameObject characterModel;
    [SerializeField] public Sprite sprite;
    [SerializeField] public string projectileName;
    [SerializeField] public CollisionType collisionType;
    [SerializeField] public float damageAmount;
    [SerializeField] public float cost;
    [SerializeField] public float lifeTime;
    [SerializeField] public float speed;

    //VARIABLES FOR INTERNAL USE
    [SerializeField] private bool canDamage;
    [SerializeField] private bool disableCoroutineIsRunning;

    //OTHER ATTRIBUTES
    [SerializeField] private BoxCollider myCollider;
    [SerializeField] private Rigidbody myRigidbody;
    [SerializeField] private GameObject playerOfOrigin;
    [SerializeField] private GameObject characterOfOrigin;
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

    public void OnObjectSpawn(){ //replaces Start()
        if(disableCoroutineIsRunning){
            DisableCoroutine();
        }

        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.AddForce(transform.forward * ProjectileToCast.speed, ForceMode.Impulse);

        weaponOfOrigin = this.transform.parent.gameObject;
        characterOfOrigin = weaponOfOrigin.transform.parent.gameObject;
        playerOfOrigin = characterOfOrigin.transform.parent.gameObject;

        this.transform.parent = null;

        canDamage = true;

        StartCoroutine(DisableObject());
    }

    IEnumerator DisableObject(){
        disableCoroutineIsRunning = true;
        yield return new WaitForSeconds(ProjectileToCast.lifeTime);
        this.gameObject.SetActive(false);
    }

    private void DisableCoroutine(){
        StopCoroutine(DisableObject());
        disableCoroutineIsRunning = false;
    }

    private void OnDisable(){
        DisableCoroutine();
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.GetComponent<HealthSystem>() != null && canDamage){
            other.GetComponent<HealthSystem>().TakeDamage(characterOfOrigin, ProjectileToCast.damageAmount);
            this.gameObject.SetActive(false);
        }
        canDamage = false;
    }
}
