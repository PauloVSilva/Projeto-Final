using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterWeaponSystem : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform gunPosition;
    [SerializeField] private bool isArmed;

    private void Start() {
        InitializeVariables();
        SubscribeToEvents();
    }

    private void InitializeVariables(){
        characterStats = gameObject.transform.parent.GetComponent<CharacterStats>();
        characterEvents = gameObject.transform.parent.GetComponent<CharacterEvents>();
        gunPosition = GetComponent<CharacterItemsDisplay>().gunPosition.transform;
        isArmed = false;
    }

    private void SubscribeToEvents(){
        //INPUT EVENTS
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterCockHammer += OnCockHammer;
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterPressTrigger += OnPressTrigger;
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterReload += OnReload;
        gameObject.transform.parent.GetComponent<PlayerInputHandler>().OnCharacterDropWeapon += OnDropWeapon;
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        if(weapon != null){
            weapon.OnCockHammer(context);
        }
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        if(weapon != null){
            weapon.OnPressTrigger(context);
        }
    }

    public void OnReload(InputAction.CallbackContext context){
        if(weapon != null){
            weapon.OnReload(context);
        }
    }

    public void OnDropWeapon(InputAction.CallbackContext context){
        if(context.performed){
            if(weapon != null){
                weapon.transform.parent = null;
                weapon.Dropped();
                weapon = null;
                isArmed = false;
                characterEvents.PlayerDroppedWeapon();
            }
        }
    }

    public void PickUpWeapon(GameObject _weapon){
        if(!isArmed){
            weapon = _weapon.GetComponent<Weapon>();
            
            weapon.transform.parent = this.transform;
            weapon.transform.rotation = this.transform.rotation;
            weapon.transform.position = gunPosition.transform.position;

            weapon.PickedUp(this.gameObject);
            isArmed = true;
            characterEvents.PlayerPickedUpWeapon(weapon);
        }
    }

    public void WeaponFired(){
        characterEvents.PlayerShotWeapon(weapon);
    }

    public void WeaponReloaded(){
        characterEvents.PlayerReloadedWeapon(weapon);
    }

    public Weapon GetWeapon(){
        if(weapon != null){
            return weapon;
        }
        else{
            return null;
        }
    }

}
