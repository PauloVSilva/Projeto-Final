using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterWeaponSystem : MonoBehaviour{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [field: SerializeField] public Weapon CharacterWeapon { get; private set; }
    [SerializeField] private Transform gunPosition;

    private void Start(){
        InitiazeComponents();
        SubscribeToEvents();
    }


    private void InitiazeComponents()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    private void SubscribeToEvents()
    {
        playerInputHandler.OnCharacterPressTrigger += OnPressTrigger;
    }


    public void SetGunPosition(Transform _gunPosition)
    {
        gunPosition = _gunPosition;
    }

    public void OnPressTrigger(InputAction.CallbackContext context)
    {
        CharacterWeapon?.OnPressTrigger(context);
    }

    public void DropWeapon(){
        CharacterWeapon.transform.parent = null;
        CharacterWeapon.Dropped();
        CharacterWeapon = null;
        characterManager.InvokeOnPlayerDroppedWeapon();
    }

    public void DestroyWeapon(){
        Destroy(CharacterWeapon.transform.gameObject);
        CharacterWeapon = null;
    }

    public bool PickUpWeapon(GameObject _weapon)
    {
        if (gunPosition == null) return false;

        CharacterWeapon = _weapon.GetComponent<Weapon>();
        
        CharacterWeapon.transform.rotation = gunPosition.transform.rotation;
        CharacterWeapon.transform.position = gunPosition.transform.position;
        CharacterWeapon.transform.parent = transform;

        CharacterWeapon.PickedUp(gameObject);

        characterManager.InvokeOnPlayerPickedUpWeapon(CharacterWeapon); //this method calls a method that invokes an event. Pretty spaghetti I know

        return true;
    }

    public void WeaponFired()
    {
        characterManager.InvokeOnPlayerShotWeapon(CharacterWeapon);
    }

    public void WeaponReloaded()
    {
        characterManager.InvokeOnPlayerReloadedWeapon(CharacterWeapon);
    }
}
