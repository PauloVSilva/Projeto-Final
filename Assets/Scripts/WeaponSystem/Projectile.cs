using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObjects{
//public class Projectile : MonoBehaviour{
    public ProjectileScriptableObject ProjectileToCast;

    [SerializeField] public enum CollisionType{contact, explosive}

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

    //OTHER ATTRIBUTES
    [SerializeField] private SphereCollider myCollider;
    [SerializeField] private Rigidbody myRigidbody;
    [SerializeField] private GameObject playerOfOrigin;
    [SerializeField] private GameObject characterOfOrigin;
    [SerializeField] private GameObject weaponOfOrigin;

    private void Awake(){
        GetScriptableObjectVariables();

        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myRigidbody = GetComponent<Rigidbody>();
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
    //void Start(){
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.AddForce(transform.forward * ProjectileToCast.speed, ForceMode.Impulse);

        playerOfOrigin = this.transform.parent.parent.parent.gameObject;
        characterOfOrigin = this.transform.parent.parent.gameObject;
        weaponOfOrigin = this.transform.parent.gameObject;

        this.transform.parent = null;

        canDamage = true;

        //Destroy(this.gameObject, ProjectileToCast.lifeTime);
        StartCoroutine(DisableObject());
        IEnumerator DisableObject(){
            yield return new WaitForSeconds(ProjectileToCast.lifeTime);
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Character") && canDamage){
            other.GetComponent<HealthSystem>().TakeDamage(characterOfOrigin, ProjectileToCast.damageAmount);
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }
        canDamage = false;
    }
}
