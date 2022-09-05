using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Weapon : MonoBehaviour{
    public WeaponScriptableObject FireWeapon;
    public Projectile projectileToCast;
    
    public enum ActionType{manual, semiAuto, fullAuto}
    public enum ChamberReloadType{pump, revolver}
    public enum Size{handGun, longGun}
    public ActionType actionType;
    public ChamberReloadType chamberReloadType;
    public Size size;
    public int ammoCapacity;
    public float fireRate; //how many times it can shoot per second

    public int ammo = 0;
    public int extraAmmo = 0;
    public float fullAutoClock = 0;
    public float fullAutoReady; //firerate clock
    public bool shooting = false;
    public bool canShoot = true;
    public bool hammerIsCocked = false; //for revolvers
    public bool pumpIsReady = false; //for shotguns

    public Transform castPoint;

    public void Awake(){
        actionType = (ActionType)FireWeapon.actionType;
        chamberReloadType = (ChamberReloadType)FireWeapon.chamberReloadType;
        size = (Size)FireWeapon.size;
        ammoCapacity = FireWeapon.ammoCapacity;
        fireRate = FireWeapon.fireRate;
        
    }

    public void Start(){
        ammo = ammoCapacity;
        extraAmmo = 999;
        fullAutoReady = 1 / FireWeapon.fireRate;
        shooting = false;

        if(actionType.ToString() == "manual"){
            hammerIsCocked = false;
        }
        else{
            hammerIsCocked = true;
        }

        pumpIsReady = false;

        castPoint = this.transform;
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
        }
        if(context.canceled){
            canShoot = true;
            shooting = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(context.performed){
            //Debug.Log("reloading");
            while (ammo < FireWeapon.ammoCapacity && extraAmmo > 0){
                extraAmmo--;
                ammo++;
            }
        }
    }

    private void Update() {
        if(actionType.ToString() == "fullAuto"){
            if(shooting && fullAutoClock >= fullAutoReady){
                Fire();
            }
            if(fullAutoClock <= fullAutoReady){
                fullAutoClock += Time.deltaTime;
            }
        }
    }

    public void Fire(){
        if(ammo - (int)projectileToCast.ProjectileToCast.Cost >= 0){
            CastProjectile();
            ammo -= (int)projectileToCast.ProjectileToCast.Cost;
            hammerIsCocked = false;
            canShoot = false;
            fullAutoClock = 0;
        }
    }

    public void CastProjectile(){
        Instantiate(projectileToCast, castPoint.position, castPoint.rotation, this.transform);
    }
}
