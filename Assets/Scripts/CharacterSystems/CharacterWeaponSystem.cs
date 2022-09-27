using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterWeaponSystem : MonoBehaviour{
    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform gunPosition;

    private void Start(){
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
    }

    public void OnCockHammer(InputAction.CallbackContext context){
        weapon?.OnCockHammer(context);
    }

    public void OnPressTrigger(InputAction.CallbackContext context){
        weapon?.OnPressTrigger(context);
    }

    public void OnReload(InputAction.CallbackContext context){
        weapon?.OnReload(context);
    }

    public void DropWeapon(){
        weapon.transform.parent = null;
        weapon.Dropped();
        weapon = null;
        characterEvents.PlayerDroppedWeapon();
    }

    public void DestroyWeapon(){
        Destroy(weapon.transform.gameObject);
        weapon = null;
    }

    public void PickUpWeapon(GameObject _weapon){
        weapon = _weapon.GetComponent<Weapon>();
        
        weapon.transform.parent = this.transform;
        weapon.transform.rotation = this.transform.rotation;
        weapon.transform.position = gunPosition.transform.position;

        weapon.PickedUp(this.gameObject);
        characterEvents.PlayerPickedUpWeapon(weapon);
    }

    public void WeaponFired(){
        characterEvents.PlayerShotWeapon(weapon);
    }

    public void WeaponReloaded(){
        characterEvents.PlayerReloadedWeapon(weapon);
    }

    public Weapon GetWeapon(){
        return weapon;
    }

}
