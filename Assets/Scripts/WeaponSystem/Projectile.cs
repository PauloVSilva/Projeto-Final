using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionType{contact, explosive, passThrough}

public class Projectile : Entity, IPooledObjects{
//public class Projectile : MonoBehaviour{
    public ProjectileScriptableObject ProjectileToCast;

    //ATTRIBUTES FROM SCRIPTABLE OBJECT
    [Space(5)]
    [Header("Scriptable Object")]
    [SerializeField] public Sprite sprite;
    [SerializeField] public string projectileName;
    [SerializeField] public CollisionType collisionType;
    [SerializeField] public float damageAmount;
    [SerializeField] public float cost;
    [SerializeField] public float speed;

    //VARIABLES FOR INTERNAL USE
    [Space(5)]
    [Header("Internal use")]
    [SerializeField] private bool canDamage;

    //OTHER ATTRIBUTES
    [Space(5)]
    [Header("Other")]
    [SerializeField] private BoxCollider myCollider;
    [SerializeField] private Rigidbody myRigidbody;
    [SerializeField] private GameObject playerOfOrigin;
    [SerializeField] private GameObject weaponOfOrigin;
    [SerializeField] private TrailRenderer trailRenderer;

    private void Awake()
    {
        GetScriptableObjectVariables();
        InitializeVariables();
    }

    protected override void Update()
    {
        AgeBehaviour();
        SpeedBehaviour();
    }

    private void GetScriptableObjectVariables()
    {
        sprite = ProjectileToCast.sprite;
        projectileName = ProjectileToCast.projectileName;
        collisionType = ProjectileToCast.collisionType;
        damageAmount = ProjectileToCast.damageAmount;
        cost = ProjectileToCast.cost;
        maxAge = ProjectileToCast.maxAge;
        speed = ProjectileToCast.speed;
    }

    private void InitializeVariables()
    {
        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
        myRigidbody = GetComponent<Rigidbody>();
        age = 0;
        isPooled = false;
    }

    private void SpeedBehaviour()
    {
        if(myRigidbody.velocity.magnitude < 0.2)
        {
            canDamage = false;
            trailRenderer.emitting = false;
        }
        else
        {
            canDamage = true;
            trailRenderer.emitting = true;
        }
    }

    public void OnObjectSpawn()
    { 
        //replaces Start()
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.AddForce(transform.forward * ProjectileToCast.speed, ForceMode.Impulse);

        weaponOfOrigin = this.transform.parent.gameObject;
        playerOfOrigin = weaponOfOrigin.transform.parent.gameObject;

        this.transform.parent = ObjectPooler.Instance.transform;

        age = 0;
        isPooled = true;
        canDamage = true;

        Color _color = playerOfOrigin.GetComponent<CharacterManager>().UIColor;

        trailRenderer.startColor = _color;
        trailRenderer.endColor = _color;
        trailRenderer.emitting = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<HealthSystem>() != null && canDamage)
        {
            other.GetComponent<HealthSystem>().TakeDamage(playerOfOrigin, ProjectileToCast.damageAmount);
            Despawn();
        }
        //canDamage = false;
        //trailRenderer.emitting = false;
    }
}
