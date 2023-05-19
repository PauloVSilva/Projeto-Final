using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public abstract class Item : Entity
{
    [field: SerializeField] public ItemScriptableObject item { get; protected set; }
    [SerializeField] protected bool canBePickedUp;
    [HideInInspector] public bool canSpin;
    protected bool canBeStored;
    protected float pickUpRadius;
    protected float rotationSpeed;
    protected bool isBlinking;
    protected SphereCollider itemCollider;
    protected Rigidbody itemRigidbody;
    protected Renderer[] objectRenderers;
    protected AudioSource audioSource;

    public bool CanBePickedUp => canBePickedUp;
    public bool CanBeStored => canBeStored;

    protected virtual void Awake()
    {
        GetScriptableObjectVariables();
        InitializeItemComponents();
        InitializeItemVariables();
    }

    private void InitializeItemComponents()
    {
        itemCollider = GetComponent<SphereCollider>();
        itemRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        objectRenderers = GetComponentsInChildren<Renderer>().ToArray();
        foreach (Renderer objectRenderer in objectRenderers)
        {
            objectRenderer.enabled = true;
        }

        itemCollider.isTrigger = true;
        itemCollider.enabled = true;
        itemCollider.radius = pickUpRadius;
        itemRigidbody.velocity = Vector3.zero;
        itemRigidbody.angularVelocity = Vector3.zero;

        isPooled = false;

        transform.Rotate(0f, 0f, 0f, Space.World); //just to make sure object isn't rotated when it spawns
    }

    protected virtual void GetScriptableObjectVariables(){
        maxAge = item.maxAge;
        canBeStored = item.canBeStored;
        pickUpRadius = item.pickUpRadius;
        rotationSpeed = item.rotationSpeed;
    }

    protected virtual void InitializeItemVariables()
    {
        age = 0;
        canBePickedUp = false;
        canSpin = true;
        StartCoroutine(CanBePickedUpDelay());
        isBlinking = false;
    }
    
    protected virtual IEnumerator CanBePickedUpDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canBePickedUp = true;
    }

    protected override void Update()
    {
        AgeBehaviour();
        CollectableBehaviour();
    }

    protected override void AgeBehaviour()
    {
        if(!persistenceRequired && canBePickedUp)
        {
            if(age >= 0)
            {
                age += Time.deltaTime;
            }

            if (age > maxAge - 10 && !isBlinking)
            {
                isBlinking = true;
                StartCoroutine(Flash(0.25f));
            }

            if(age > maxAge)
            {
                Despawn();
            }
        }
    }

    protected virtual void CollectableBehaviour()
    {
        if(canBePickedUp && canSpin)
        {
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }
    }

    private IEnumerator Flash(float time)
    {
        yield return new WaitForSeconds(time);

        if(isBlinking)
        {
            foreach (Renderer objectRenderer in objectRenderers)
            {
                objectRenderer.enabled = !objectRenderer.enabled;
            }

            if (age < maxAge - 3)
            {
                StartCoroutine(Flash(0.25f));
            }
            else 
            {
                StartCoroutine(Flash(0.1f));
            }
        }
        else
        {
            foreach (Renderer objectRenderer in objectRenderers)
            {
                objectRenderer.enabled = true;
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}
