using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterWeaponSystem : MonoBehaviour{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform gunPosition;

    private void Start() {
        SubscribeToEvents();
    }

    public void SetGunPosition(){
        gunPosition = characterEvents.characterObject.GetComponent<CharacterItemsDisplay>().gunPosition.transform;
    }

    private void SubscribeToEvents(){
        //INPUT EVENTS
        playerInputHandler.OnCharacterCockHammer += OnCockHammer;
        playerInputHandler.OnCharacterPressTrigger += OnPressTrigger;
        playerInputHandler.OnCharacterReload += OnReload;
        playerInputHandler.OnCharacterDropWeapon += OnDropWeapon;
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
                characterStats.isArmed = false;
                characterEvents.PlayerDroppedWeapon();
            }
        }
    }

    public void PickUpWeapon(GameObject _weapon){
        if(!characterStats.isArmed){
            weapon = _weapon.GetComponent<Weapon>();
            
            weapon.transform.parent = this.transform;
            weapon.transform.rotation = this.transform.rotation;
            weapon.transform.position = gunPosition.transform.position;

            weapon.PickedUp(this.gameObject);
            characterStats.isArmed = true;
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
