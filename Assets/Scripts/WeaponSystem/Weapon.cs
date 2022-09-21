using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Weapon : MonoBehaviour{
    [SerializeField] public WeaponScriptableObject FireWeapon;
    [SerializeField] public ProjectileScriptableObject projectileToCast;
    
    [SerializeField] private enum ActionType{manual, semiAuto, fullAuto}
    [SerializeField] private enum ChamberReloadType{pump, revolver}
    [SerializeField] private enum Size{handGun, longGun}

    //ATTRIBUTES FROM SCRIPTABLE OBJECT
    [SerializeField] public Sprite sprite;
    [SerializeField] public string weaponName;
    [SerializeField] private ActionType actionType;
    [SerializeField] private ChamberReloadType chamberReloadType;
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
    [SerializeField] private bool canReload;
    [SerializeField] private bool canBePickedUp;

    //OTHER ATTRIBUTES
    [SerializeField] private Transform castPoint;
    [SerializeField] public GameObject holder;
    [SerializeField] private SphereCollider myCollider;
    //[SerializeField] private Rigidbody myRigidbody;

    private void Awake(){
        GetScriptableObjectVariables();
    }

    private void Start(){
        InitializeInternalVariables();
    }

    private void Update(){
        FullAutoBehavior();
        CollectableBehavior();
    }

    private void GetScriptableObjectVariables(){
        sprite = FireWeapon.sprite;
        weaponName = FireWeapon.weaponName;
        actionType = (ActionType)FireWeapon.actionType;
        chamberReloadType = (ChamberReloadType)FireWeapon.chamberReloadType;
        size = (Size)FireWeapon.size;
        ammoCapacity = FireWeapon.ammoCapacity;
        fireRate = FireWeapon.fireRate;
        weight = FireWeapon.weight;
        reloadTime = FireWeapon.reloadTime;
    }

    private void InitializeInternalVariables(){
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        //myRigidbody = GetComponent<Rigidbody>();
        if(transform.parent != null){
            holder = gameObject.transform.parent.gameObject;
            canBePickedUp = false;
            myCollider.enabled = false;
            //myRigidbody.isKinematic = true;
        }
        else{
            holder = null;
            canBePickedUp = true;
            myCollider.enabled = true;
            //myRigidbody.isKinematic = false;
        }

        ammo = ammoCapacity;
        extraAmmo = 999;
        fullAutoClock = 0;
        fullAutoReady = 1 / fireRate;
        shooting = false;
        canShoot = true;
        hammerIsCocked = false;
        //pumpIsReady = false;
        canReload = true;
    }

    private void OnDisable(){
        StopCoroutine(Reload(reloadTime));
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

    private void CollectableBehavior(){
        if(canBePickedUp){
            transform.Rotate(Vector3.up * (90 * Time.deltaTime));
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
            if(ammo - (int)projectileToCast.cost >= 0){
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
    
    IEnumerator Reload(float reloadTime){
        yield return new WaitForSeconds(reloadTime);
        while (ammo < ammoCapacity && extraAmmo > 0){
            extraAmmo--;
            ammo++;
        }
        holder.transform.GetComponent<CharacterWeaponSystem>().WeaponReloaded();
    }

    public void PickedUp(GameObject character){
        holder = character;
        canBePickedUp = false;
        myCollider.enabled = false;
    }

    public void Dropped(){
        holder = null;
        myCollider.enabled = true;
        StartCoroutine(PickUpDelay());
        IEnumerator PickUpDelay(){
            yield return new WaitForSeconds(1f);
            canBePickedUp = true;
        }
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

    public bool CanBePickedUp(){
        return canBePickedUp;
    }

    private void CastProjectile(){
        ObjectPooler.Instance.SpawnFromPool(projectileToCast.projectileName, castPoint.position, castPoint.rotation, this.gameObject);
    }
}
