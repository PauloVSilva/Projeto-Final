using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour{
    [SerializeField] protected List<ItemSlot> inventorySlots = new List<ItemSlot>();

    public virtual bool AddToInventory(ItemScriptableObject _item){
        for(int i = 0; i < inventorySlots.Count; i++){
            if(inventorySlots[i].item == _item && inventorySlots[i].stackSize < _item.maxStackSize){
                inventorySlots[i].AddToStack();
                return true;
            }
        }
        for(int i = 0; i < inventorySlots.Count; i++){
            if(inventorySlots[i].item == null){
                inventorySlots[i].AddToSlot(_item);
                return true;
            }
        }
        return false;
    }

    public virtual void ClearInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            inventorySlots[i] = new ItemSlot();
        }
    }

    public virtual void DropAllInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            while(inventorySlots[i].stackSize > 0){
                Instantiate(inventorySlots[i].item.itemModel, this.gameObject.transform.position, this.gameObject.transform.rotation);
                inventorySlots[i].DropItem();
            }
        }
    }
}
