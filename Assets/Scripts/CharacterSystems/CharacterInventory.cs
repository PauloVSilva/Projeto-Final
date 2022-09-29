using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInventory : Inventory{
    [SerializeField] private CharacterWeaponSystem characterWeaponSystem;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private ItemSlot weaponSlot = new ItemSlot();

    private void Start(){
        SubscribeToEvents();
    }

    private void SubscribeToEvents(){
        //INPUT EVENTS
        playerInputHandler.OnCharacterDropItem += OnDropItem;
    }

    public bool IsArmed(){
        if(weaponSlot.stackSize > 0){
            return true;
        }
        return false;
    }

    public void OnDropItem(InputAction.CallbackContext context){
        if(context.performed){
            if(weaponSlot.stackSize > 0){
                characterWeaponSystem.DropWeapon();
                weaponSlot.DropItem();
            }
        }
    }

    public void PickWeapon(GameObject _weapon){
        characterWeaponSystem.PickUpWeapon(_weapon);
        AddToWeaponSlot(_weapon.GetComponent<Weapon>().item);
    }

    public bool AddToWeaponSlot(ItemScriptableObject _item){
        if(weaponSlot.stackSize == 0){
            weaponSlot.AddToSlot(_item);
            return true;
        }
        return false;
    }

    public override void ClearInventory(){
        if(weaponSlot.stackSize > 0){
            characterWeaponSystem.DestroyWeapon();
            weaponSlot = new ItemSlot();
        }
        for(int i = 0; i < inventorySlots.Count; i++){
            inventorySlots[i] = new ItemSlot();
        }
    }

    public override void DropAllInventory(){
        if(weaponSlot.stackSize > 0){
            //Instantiate(weaponSlot.item.itemModel, this.gameObject.transform.position, this.gameObject.transform.rotation);
            characterWeaponSystem.DropWeapon();
            weaponSlot.DropItem();
        }
        for(int i = 0; i < inventorySlots.Count; i++){
            while(inventorySlots[i].stackSize > 0){
                if(!ObjectPooler.instance.SpawnFromPool(inventorySlots[i].item.itemModel, this.transform.position, this.transform.rotation, this.gameObject)){
                    Debug.LogWarning("Something went wrong. Object Pooler couldn't Spawn " + inventorySlots[i].item.itemModel);
                }

                //Instantiate(inventorySlots[i].item.itemModel, this.gameObject.transform.position, this.gameObject.transform.rotation);
                inventorySlots[i].DropItem();
            }
        }
    }
}