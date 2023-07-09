using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;

public enum ActionType { SemiAuto, FullAuto }
public enum ChamberRefillType{pump, revolver}
public enum ReloadType{singleBullet, magazine}
public enum Size{handGun, longGun}

public class Weapon : Item
{
    [SerializeField] private WeaponScriptableObject weapon;
    [SerializeField] public ProjectileScriptableObject projectileToCast;

    //ATTRIBUTES FROM SCRIPTABLE OBJECT
    [Space(5)]
    [Header("Scriptable Object")]
    [SerializeField] public Sprite sprite;
    [SerializeField] public string weaponName;
    [SerializeField] private ActionType actionType;
    [SerializeField, Min(0)] public int ammoCapacity;
    [SerializeField, Min(0)] private float fireRate; //how many times it can shoot per second
    [SerializeField, Min(0)] private float reloadTime;

    //VARIABLES FOR INTERNAL USE
    [Space(5)]
    [Header("Internal use")]
    [SerializeField, Min(0)] public int ammo = 0;
    [SerializeField, Min(0)] public int totalAmmo = 0;
    [SerializeField, Min(0)] private int ammoMultiplier;
    [SerializeField, Min(0)] private float fullAutoTime; //time since last shot
    [SerializeField, Min(0)] private float fullAutoClock; //time until it's ready to fire again
    [SerializeField] private bool shooting;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool reloading;

    //OTHER ATTRIBUTES
    [Space(5)]
    [Header("Other")]
    [SerializeField] private Transform castPoint;
    [SerializeField] protected SphereCollider gunCollider;
    [SerializeField] public GameObject holder;

    [SerializeField] private AudioClip fireSFX;
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private GameObject collectableVFX;

    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private GameObject closestTarget;

    [SerializeField] private Animator _animator;

    protected override void Update()
    {
        AgeBehaviour();
        CollectableBehaviour();

        FullAutoBehavior();
        SearchForTargets();
    }

    protected void OnDisable()
    {
        StopCoroutine(Reload());
    }

    protected override void GetScriptableObjectVariables()
    {
        item = weapon;

        base.GetScriptableObjectVariables();

        sprite = weapon.itemSprite;
        weaponName = weapon.itemName;
        actionType = weapon.actionType;
        //chamberRefillType = weapon.chamberRefillType;
        //size = weapon.size;
        ammoCapacity = weapon.ammoCapacity;
        fireRate = weapon.fireRate;
        reloadTime = weapon.reloadTime;
    }

    protected override void InitializeItemVariables()
    {
        base.InitializeItemVariables();

        fieldOfView = GetComponentInChildren<FieldOfView>();
        _animator = GetComponent<Animator>();

        holder = null;
        ammo = ammoCapacity;
        totalAmmo = ammoCapacity * ammoMultiplier;
        fullAutoTime = 0;
        fullAutoClock = 1 / fireRate;
        shooting = false;
        canShoot = true;
        reloading = false;
        //triggerHeld = false;
    }

    protected override IEnumerator CanBePickedUpDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canBePickedUp = (holder == null);
    }


    private void FullAutoBehavior()
    {
        if(actionType == ActionType.FullAuto)
        {
            if(shooting && fullAutoTime >= fullAutoClock && ammo - (int)projectileToCast.cost >= 0)
            {
                Fire();
            }

            fullAutoTime = MathF.Min(fullAutoTime + Time.deltaTime, fullAutoClock);
        }
    }

    protected override void CollectableBehaviour()
    {
        base.CollectableBehaviour();

        if(collectableVFX == null)
        {
            Debug.Log("VFX is null");
            return;
        }

        collectableVFX.SetActive(canBePickedUp && ammo > 0);
    }

    private void SearchForTargets()
    {
        if (fieldOfView == null) return;

        if (fieldOfView.visibleTargets.Count < 1)
        {
            closestTarget = null;
            return;
        }

        float closest = float.MaxValue;

        foreach (var item in fieldOfView.visibleTargets)
        {
            float distance = Vector3.Distance(transform.position, item.position);

            if(distance < closest)
            {
                closest = distance;

                closestTarget = item.transform.gameObject;
            }
        }
    }


    public void OnPressTrigger(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            CanFireVerification();
        }

        if(context.canceled)
        {
            canShoot = true;
            shooting = false;
        }

        //triggerHeld = context.performed;
    }

    /*public void OnReload(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(CanReload())
            {
                StartCoroutine(Reload());
            }
        }
    }*/

    private bool CanReload()
    {
        if(ammo < ammoCapacity && totalAmmo > 0)
        {
            return true;
        }

        return false;
    }
    
    private IEnumerator Reload()
    {
        if (CanReload() && !reloading)
        {
            reloading = true;

            yield return new WaitForSeconds(reloadTime);

            for(int i = 0; i < ammoCapacity && totalAmmo > 0; i++)
            {
                ammo++;
                totalAmmo--;
            }

            holder.transform.GetComponent<CharacterWeaponSystem>()?.WeaponReloaded();

            reloading = false;
        }
    }

    public void PickedUp(GameObject character)
    {
        holder = character;
        canBePickedUp = false;
        age = 0;
        isBlinking = false;
        itemCollider.enabled = false;
        gunCollider.enabled = false;
        itemRigidbody.useGravity = false;
    }

    public void Dropped()
    {
        holder = null;
        itemCollider.enabled = true;
        gunCollider.enabled = true;
        itemRigidbody.useGravity = true;
        persistenceRequired = false;
        canSpin = true;
        StartCoroutine(CanBePickedUpDelay());
    }


    private void CanFireVerification()
    {
        if (ammo - (int)projectileToCast.cost >= 0)
        {
            if (actionType == ActionType.SemiAuto && canShoot)
            {
                Fire();
            }

            if (actionType == ActionType.FullAuto)
            {
                shooting = true;
            }
        }
    }

    private void EmptyGunCheck()
    {
        if (ammo > 0) return;

        if (totalAmmo > 0) //reload
        {
            StartCoroutine(Reload());
        }

        if (totalAmmo == 0) //drop weapon
        {
            holder.transform.TryGetComponent(out CharacterInventory characterInventory);

            characterInventory.DropWeapon();

            canBePickedUp = false;
            age = maxAge - 10;
        }
    }

    private void Fire()
    {
        CastProjectile();

        ammo -= projectileToCast.cost;
        canShoot = false;
        fullAutoTime = 0;

        holder.transform.GetComponent<CharacterWeaponSystem>().WeaponFired(); //line should be reworked

        /*_animator.SetBool("Fire", true);
        StartCoroutine(Unfire());
        IEnumerator Unfire()
        {
            yield return new WaitForSeconds(fireRate / 2);

            _animator.SetBool("Fire", false);
        }*/

        audioSource.PlayOneShot(fireSFX);

        GameObject _fireVFX = Instantiate(fireVFX, castPoint.transform.position, castPoint.transform.rotation);
        StartCoroutine(ClearVFX());
        IEnumerator ClearVFX()
        {
            yield return new WaitForSeconds(0.25f);
            _fireVFX.transform.DOScale(0.5f, 0.25f);
            Destroy(_fireVFX, 0.25f);
        }

        EmptyGunCheck();
    }

    private void CastProjectile()
    {
        GameObject projectile;

        if(closestTarget != null)
        {
            Vector3 direction_to_model = closestTarget.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction_to_model, Vector3.up);

            projectile = ObjectPooler.Instance.SpawnFromPool(projectileToCast.projectileModel, castPoint.position, rotation, this.gameObject);
        }
        else
        {
            projectile = ObjectPooler.Instance.SpawnFromPool(projectileToCast.projectileModel, castPoint.position, castPoint.rotation, this.gameObject);
        }

        if (projectile == null)
        {
            Debug.LogWarning("Something went wrong. Object Pooler couldn't Spawn " + projectileToCast.projectileModel);
        } 
    }
}
