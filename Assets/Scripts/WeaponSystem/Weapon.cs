using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
    
public enum ActionType{manual, semiAuto, fullAuto}
public enum ChamberRefillType{pump, revolver}
public enum ReloadType{singleBullet, magazine}
public enum Size{handGun, longGun}

[RequireComponent(typeof(AudioSource))]

public class Weapon : Item{
    [SerializeField] private WeaponScriptableObject weapon;
    [SerializeField] public ProjectileScriptableObject projectileToCast;

    //ATTRIBUTES FROM SCRIPTABLE OBJECT
    [SerializeField] public Sprite sprite;
    [SerializeField] public string weaponName;
    [SerializeField] private ActionType actionType;
    [SerializeField] private ChamberRefillType chamberRefillType;
    [SerializeField] private Size size;
    [SerializeField] public int ammoCapacity;
    [SerializeField] private float fireRate; //how many times it can shoot per second
    [SerializeField] private float reloadTime;

    //VARIABLES FOR INTERNAL USE
    [SerializeField] public int ammo = 0;
    [SerializeField] public int totalAmmo = 0;
    [SerializeField] private float fullAutoClock;
    [SerializeField] private float fullAutoReady; //firerate clock
    [SerializeField] private bool shooting;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool triggerHeld;

    //OTHER ATTRIBUTES
    [SerializeField] private Transform castPoint;
    [SerializeField] protected SphereCollider gunCollider;
    [SerializeField] public GameObject holder;

    [SerializeField] private AudioSource audioData;

    protected override void Update(){
        AgeBehaviour();
        CollectableBehaviour();
        FullAutoBehavior();
    }

    protected override void GetScriptableObjectVariables(){
        item = weapon;

        base.GetScriptableObjectVariables();

        sprite = weapon.itemSprite;
        weaponName = weapon.itemName;
        actionType = weapon.actionType;
        chamberRefillType = weapon.chamberRefillType;
        size = weapon.size;
        ammoCapacity = weapon.ammoCapacity;
        fireRate = weapon.fireRate;
        reloadTime = weapon.reloadTime;
    }

    protected override void InitializeItemVariables(){
        base.InitializeItemVariables();

        audioData = GetComponent<AudioSource>();

        holder = null;
        ammo = ammoCapacity;
        totalAmmo = ammoCapacity * 10;
        fullAutoClock = 0;
        fullAutoReady = 1 / fireRate;
        shooting = false;
        canShoot = true;
        triggerHeld = false;
    }

    protected override IEnumerator CanBePickedUpDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canBePickedUp = (holder == null);
    }

    protected void OnDisable(){
        StopCoroutine(Reload());
    }

    private void FullAutoBehavior(){
        if(actionType == ActionType.fullAuto){
            if(shooting && fullAutoClock >= 1 / fireRate){
                Fire();
            }
            if(fullAutoClock > 1 / fireRate){
                fullAutoClock = 1 / fireRate;
            }
            if(fullAutoClock < 1 / fireRate){
                fullAutoClock += Time.deltaTime;
            }
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(context.performed)
        {
            StopCoroutine(Reload());
            if(ammo - (int)projectileToCast.cost >= 0)
            {
                if(actionType == ActionType.semiAuto && canShoot)
                {
                    Fire();
                }
                if(actionType == ActionType.fullAuto)
                {
                    shooting = true;
                }
            }
            triggerHeld = true;
        }
        if(context.canceled)
        {
            canShoot = true;
            shooting = false;
            triggerHeld = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(context.performed){
            if(CanReload()){
                StartCoroutine(Reload());
            }
        }
    }

    private bool CanReload(){
        if(!triggerHeld && (ammo < ammoCapacity && totalAmmo > 0)){
            return true;
        }
        return false;
    }
    
    IEnumerator Reload()
    {
        if(CanReload())
        {
            yield return new WaitForSeconds(reloadTime);

            for(int i = 0; i < ammoCapacity && totalAmmo > 0; i++)
            {
                ammo++;
                totalAmmo--;
            }

            holder.transform.GetComponent<CharacterWeaponSystem>()?.WeaponReloaded();
        }
    }

    public void PickedUp(GameObject character){
        holder = character;
        canBePickedUp = false;
        age = 0;
        isBlinking = false;
        itemCollider.enabled = false;
        gunCollider.enabled = false;
        itemRigidbody.useGravity = false;
    }

    public void Dropped(){
        holder = null;
        itemCollider.enabled = true;
        gunCollider.enabled = true;
        itemRigidbody.useGravity = true;
        persistenceRequired = false;
        StartCoroutine(CanBePickedUpDelay());
    }

    private void Fire(){
        if(ammo - projectileToCast.cost >= 0){
            //play gun fire sound
            audioData.Play(0);

            CastProjectile();
            ammo -= projectileToCast.cost;
            canShoot = false;
            fullAutoClock = 0;

            holder.transform.GetComponent<CharacterWeaponSystem>().WeaponFired();

            if(ammo == 0)
            {
                StartCoroutine(Reload());
            }
            if(ammo == 0 && totalAmmo == 0)
            {
                holder.transform.TryGetComponent(out CharacterInventory characterInventory);

                characterInventory.DropWeapon();

                canBePickedUp = false;
                age = maxAge - 10;
            }

        }
    }

    private void CastProjectile(){
        if(ObjectPooler.Instance.SpawnFromPool(projectileToCast.projectileModel, castPoint.position, castPoint.rotation, this.gameObject) == null)
        {
            Debug.LogWarning("Something went wrong. Object Pooler couldn't Spawn " + projectileToCast.projectileModel);
        }
    }
}
