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
    public float fireRate;

    public int ammo = 0;
    public int extraAmmo = 0;
    public float currentShotTimer; //firerate clock
    public bool shooting = false; //to determine when to reset the firerate clock
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
        currentShotTimer = 0;
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
        if(actionType.ToString() != "fullAuto"){
            if(context.performed){
                if(ammo - (int)projectileToCast.ProjectileToCast.Cost >= 0){
                    if(actionType.ToString() == "manual" && hammerIsCocked){
                        CastProjectile();
                        ammo -= (int)projectileToCast.ProjectileToCast.Cost;
                        hammerIsCocked = false;
                    }
                    if(actionType.ToString() == "semiAuto"){
                        CastProjectile();
                        ammo -= (int)projectileToCast.ProjectileToCast.Cost;
                    }
                }
            }
        }
        if(actionType.ToString() == "fullAuto"){
            if(context.started){
                if(ammo - (int)projectileToCast.ProjectileToCast.Cost >= 0){
                    CastProjectile();
                    ammo -= (int)projectileToCast.ProjectileToCast.Cost;
                }
            }
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

    public void CastProjectile(){
        Instantiate(projectileToCast, castPoint.position, castPoint.rotation, this.transform);
    }
}
