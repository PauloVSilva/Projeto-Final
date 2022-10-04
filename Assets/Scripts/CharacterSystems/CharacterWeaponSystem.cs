using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterWeaponSystem : MonoBehaviour{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform gunPosition;

    private void Start(){
        characterManager = GetComponent<CharacterManager>();
        SubscribeToEvents();
    }

    public void SetGunPosition(){
        gunPosition = characterManager.characterObject.GetComponent<CharacterItemsDisplay>().gunPosition.transform;
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
        characterManager.PlayerDroppedWeapon();
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
        characterManager.PlayerPickedUpWeapon(weapon);
    }

    public void WeaponFired(){
        characterManager.PlayerShotWeapon(weapon);
    }

    public void WeaponReloaded(){
        characterManager.PlayerReloadedWeapon(weapon);
    }

    public Weapon GetWeapon(){
        return weapon;
    }

}
