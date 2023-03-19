using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInventory : Inventory{
    private CharacterManager characterManager;
    private CharacterWeaponSystem characterWeaponSystem;
    private PlayerInputHandler playerInputHandler;
    [SerializeField] private ItemSlot weaponSlot = new ItemSlot();

    private void Awake(){
        InitializeComponents();
        SubscribeToEvents();
    }

    private void InitializeComponents(){
        characterManager = GetComponent<CharacterManager>();
        characterWeaponSystem = GetComponent<CharacterWeaponSystem>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    private void SubscribeToEvents(){
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

    public void DropWeapon()
    {
        if(weaponSlot.stackSize > 0)
        {
            characterWeaponSystem.DropWeapon();
            weaponSlot.DropItem();
        }
    }

    public void PickWeapon(GameObject _weapon){
        if(characterWeaponSystem.PickUpWeapon(_weapon))
        {
            AddToWeaponSlot(_weapon.GetComponent<Weapon>().item);
        }
    }

    public bool AddToWeaponSlot(ItemScriptableObject _item){
        if(weaponSlot.stackSize == 0)
        {
            weaponSlot.AddToSlot(_item);
            return true;
        }
        return false;
    }

    public override void ClearInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            inventorySlots[i] = new ItemSlot();
        }
        if(weaponSlot.stackSize > 0){
            characterWeaponSystem.DestroyWeapon();
            weaponSlot = new ItemSlot();
        }
    }

    public override void DropAllInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            while(inventorySlots[i].stackSize > 0){
                GameObject droppedItem = ObjectPooler.Instance.SpawnFromPool(inventorySlots[i].item.itemModel, this.transform.position, this.transform.rotation);
                if(droppedItem == null){
                    Debug.LogWarning("Object Pooler couldn't Spawn " + inventorySlots[i].item.itemModel + ". Item will be instantiated instead.");
                    Instantiate(inventorySlots[i].item.itemModel, this.gameObject.transform.position, this.gameObject.transform.rotation);
                }
                if(droppedItem.GetComponent<Coin>() != null){
                    characterManager.DecreaseScore(droppedItem.GetComponent<Coin>().Value);
                }
                inventorySlots[i].DropItem();
            }
        }


        if(weaponSlot.stackSize > 0){
            characterWeaponSystem.DropWeapon();
            weaponSlot.DropItem();
        }
    }
}