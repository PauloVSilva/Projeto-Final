using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Weapon : MonoBehaviour{
    public WeaponScriptableObject FireWeapon;
    public Projectile projectileToCast;
    
    [SerializeField] private enum ActionType{manual, semiAuto, fullAuto}
    [SerializeField] private enum ChamberReloadType{pump, revolver}
    [SerializeField] private enum Size{handGun, longGun}
     
    //VARIABLES THAT WILL COME FROM SCRIPTABLE OBJECT
    [SerializeField] private ActionType actionType;
    [SerializeField] private ChamberReloadType chamberReloadType;
    [SerializeField] private Size size;
    [SerializeField] private int ammoCapacity;
    [SerializeField] private float fireRate; //how many times it can shoot per second
    [SerializeField] private float weight;
    [SerializeField] private float reloadTime;

    //VARIABLES FOR INTERNAL USE
    [SerializeField] private int ammo = 0;
    [SerializeField] private int extraAmmo = 0;
    [SerializeField] private float fullAutoClock;
    [SerializeField] private float fullAutoReady; //firerate clock
    [SerializeField] private bool shooting;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool hammerIsCocked; //for revolvers
    //[SerializeField] private bool pumpIsReady; //for shotguns
    [SerializeField] private bool canReload;
    [SerializeField] private Transform castPoint;

    private void Awake(){
        GetScriptableObjectVariables();
    }

    private void Start(){
        InitializeInternalVariables();
    }

    private void Update(){
        FullAutoBehavior();
    }

    private void GetScriptableObjectVariables(){
        actionType = (ActionType)FireWeapon.actionType;
        chamberReloadType = (ChamberReloadType)FireWeapon.chamberReloadType;
        size = (Size)FireWeapon.size;
        ammoCapacity = FireWeapon.ammoCapacity;
        fireRate = FireWeapon.fireRate;
        weight = FireWeapon.weight;
        reloadTime = FireWeapon.reloadTime;
    }

    private void InitializeInternalVariables(){
        ammo = ammoCapacity;
        extraAmmo = 999;
        fullAutoClock = 0;
        fullAutoReady = 1 / fireRate;
        shooting = false;
        canShoot = true;
        hammerIsCocked = false;
        //pumpIsReady = false;
        canReload = true;
        castPoint = this.transform;
    }

    private void FullAutoBehavior(){
        if(actionType.ToString() == "fullAuto"){
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
            if(ammo - (int)projectileToCast.ProjectileToCast.Cost >= 0){
                if(actionType.ToString() == "manual" && hammerIsCocked && canShoot){
                    Fire();
                }
                if(actionType.ToString() == "semiAuto" && canShoot){
                    Fire();
                }
                if(actionType.ToString() == "fullAuto"){
                    shooting = true;
                }
            }
            canReload = false;
        }
        if(context.canceled){
            if(actionType.ToString() == "semiAuto"){
                hammerIsCocked = true;
            }
            canShoot = true;
            shooting = false;
            canReload = true;
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(context.performed){
            if(canReload){
                StartCoroutine(Reload(reloadTime));
            }
        }
    }

    private void Fire(){
        if(ammo - (int)projectileToCast.ProjectileToCast.Cost >= 0){
            CastProjectile();
            ammo -= (int)projectileToCast.ProjectileToCast.Cost;
            hammerIsCocked = false;
            canShoot = false;
            fullAutoClock = 0;
        }
    }

    IEnumerator Reload(float reloadTime){
        yield return new WaitForSeconds(reloadTime);
        while (ammo < ammoCapacity && extraAmmo > 0){
            extraAmmo--;
            ammo++;
        }
    }

    private void CastProjectile(){
        Instantiate(projectileToCast, castPoint.position, castPoint.rotation, this.transform);
    }
}
