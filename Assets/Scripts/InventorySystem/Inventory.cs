using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour{
    [SerializeField] private List<ItemSlot> inventorySlots = new List<ItemSlot>();

    public bool AddToInventory(ItemScriptableObject _item){
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

    public void ClearInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            inventorySlots[i] = new ItemSlot();
        }
    }

    public void DropAllInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            while(inventorySlots[i].stackSize > 0){
                Instantiate(inventorySlots[i].item.itemModel, this.gameObject.transform.position, this.gameObject.transform.rotation);
                inventorySlots[i].DropItem();
            }
        }
    }
}
