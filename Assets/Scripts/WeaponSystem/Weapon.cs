using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
    
public enum ActionType{manual, semiAuto, fullAuto}
public enum ChamberRefillType{pump, revolver}
public enum ReloadType{singleBullet, magazine}
public enum Size{handGun, longGun}

public class Weapon : Item{
    [SerializeField] public WeaponScriptableObject weapon;
    [SerializeField] public ProjectileScriptableObject projectileToCast;

    //ATTRIBUTES FROM SCRIPTABLE OBJECT
    [SerializeField] public Sprite sprite;
    [SerializeField] public string weaponName;
    [SerializeField] private ActionType actionType;
    [SerializeField] private ChamberRefillType chamberRefillType;
    [SerializeField] private Size size;
    [SerializeField] public int ammoCapacity;
    [SerializeField] private float fireRate; //how many times it can shoot per second
    [SerializeField] private float weight;
    [SerializeField] private float reloadTime;

    //VARIABLES FOR INTERNAL USE
    [SerializeField] public int ammo = 0;
    [SerializeField] private int extraAmmo = 0;
    [SerializeField] private float fullAutoClock;
    [SerializeField] private float fullAutoReady; //firerate clock
    [SerializeField] private bool shooting;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool hammerIsCocked; //for revolvers
    //[SerializeField] private bool pumpIsReady; //for shotguns
    [SerializeField] private bool triggerHeld;

    //OTHER ATTRIBUTES
    [SerializeField] private Transform castPoint;
    [SerializeField] public GameObject holder;

    protected override void Awake(){
        GetScriptableObjectVariables();
    }

    protected void Start(){
        InitializeInternalVariables();
    }

    protected override void Update(){
        FullAutoBehavior();
        CollectableBehaviour();
        AgeBehaviour();
    }

    private void GetScriptableObjectVariables(){
        sprite = weapon.sprite;
        weaponName = weapon.weaponName;
        actionType = weapon.actionType;
        chamberRefillType = weapon.chamberRefillType;
        size = weapon.size;
        ammoCapacity = weapon.ammoCapacity;
        fireRate = weapon.fireRate;
        weight = weapon.weight;
        reloadTime = weapon.reloadTime;
    }

    private void InitializeInternalVariables(){
        myCollider.isTrigger = true;
        if(transform.parent != null){
            holder = gameObject.transform.parent.gameObject;
            canBePickedUp = false;
            myCollider.enabled = false;
        }
        else{
            holder = null;
            canBePickedUp = true;
            myCollider.enabled = true;
        }

        ammo = ammoCapacity;
        extraAmmo = 999;
        fullAutoClock = 0;
        fullAutoReady = 1 / fireRate;
        shooting = false;
        canShoot = true;
        hammerIsCocked = false;
        //pumpIsReady = false;
        triggerHeld = false;
    }

    private void OnDisable(){
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

    public void OnCockHammer(InputAction.CallbackContext context){
        if(context.performed){
            //Debug.Log("hammer pressed");
            hammerIsCocked = true;
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(context.performed){
            StopCoroutine(Reload());
            if(ammo - (int)projectileToCast.cost >= 0){
                if(actionType == ActionType.manual && hammerIsCocked && canShoot){
                    Fire();
                }
                if(actionType == ActionType.semiAuto && canShoot){
                    Fire();
                }
                if(actionType == ActionType.fullAuto){
                    shooting = true;
                }
            }
            triggerHeld = true;
        }
        if(context.canceled){
            if(actionType.ToString() == "semiAuto"){
                hammerIsCocked = true;
            }
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
        if(!triggerHeld && (ammo < ammoCapacity && extraAmmo > 0)){
            return true;
        }
        return false;
    }
    
    IEnumerator Reload(){
        while(CanReload()){
            yield return new WaitForSeconds(reloadTime);
            extraAmmo--;
            ammo++;
            holder.transform.GetComponent<CharacterWeaponSystem>()?.WeaponReloaded();
        }
    }

    public void PickedUp(GameObject character){
        holder = character;
        canBePickedUp = false;
        age = 0;
        isBlinking = false;
        myCollider.enabled = false;
    }

    public void Dropped(){
        holder = null;
        myCollider.enabled = true;
        persistenceRequired = false;
        StartCoroutine(CanBePickedUpDelay());
    }

    private void Fire(){
        if(ammo - (int)projectileToCast.cost >= 0){
            CastProjectile();
            ammo -= (int)projectileToCast.cost;
            hammerIsCocked = false;
            canShoot = false;
            fullAutoClock = 0;

            holder.transform.GetComponent<CharacterWeaponSystem>().WeaponFired();
        }
    }

    private void CastProjectile(){
        //ObjectPooler.instance.SpawnFromPool(projectileToCast.projectileName, castPoint.position, castPoint.rotation, this.gameObject);
        ObjectPooler.instance.SpawnFromPool(projectileToCast.projectileModel, castPoint.position, castPoint.rotation, this.gameObject);
    }
}
