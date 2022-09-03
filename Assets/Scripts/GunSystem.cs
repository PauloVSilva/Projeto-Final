using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GunSystem : MonoBehaviour{
    [SerializeField] private Projectile projectileToCast;

    //public event Action<GameObject> OnGunFire;

    [SerializeField] private int maxAmmo = 6;
    [SerializeField] private int ammo = 6;
    [SerializeField] private int extraAmmo = 0;

    [SerializeField] bool hammerIsCocked = false;
    //[SerializeField] private float timeBetweenShots = 1f;
    //private float currentShotTimer;
    //private bool shooting = false;

    [SerializeField] private Transform castPoint;

    private void Update(){
        //bool isGunShootingHeldDown = playerControls.Controls.PressTrigger.ReadValue<float>() > 0.1f;
        //bool hasEnoughBullets = ammo - projectileToCast.ProjectileToCast.Cost >= 0f;

        //if(!shooting && isGunShootingHeldDown && hasEnoughBullets){
        //    shooting = true;
        //    ammo -= (int)projectileToCast.ProjectileToCast.Cost;
        //    currentCastTimer = 0;
        //    CastProjectile();
        //}

        //if(shooting){
        //    currentCastTimer += Time.deltaTime;
        //    if(currentCastTimer > timeBetweenShots){
        //        shooting = false;
        //    }
        //}
    }

    

    public void OnCockHammer(InputAction.CallbackContext context){
        if(context.performed){
            //Debug.Log("hammer pressed");
            hammerIsCocked = true;
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(context.performed){
            //Debug.Log("trigger pressed");
            if(hammerIsCocked){
                if(ammo - (int)projectileToCast.ProjectileToCast.Cost >= 0){
                    //Debug.Log("Fire!");
                    CastProjectile();
                    ammo -= (int)projectileToCast.ProjectileToCast.Cost;
                    hammerIsCocked = false;
                }
                else{
                    //Debug.Log("clic clic D;");
                }
            }
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(context.performed){
            //Debug.Log("reloading");
            while (ammo < maxAmmo && extraAmmo > 0){
                extraAmmo--;
                ammo++;
            }
        }
    }

    public void CastProjectile(){
        Instantiate(projectileToCast, castPoint.position, castPoint.rotation, this.transform);
    }
}
