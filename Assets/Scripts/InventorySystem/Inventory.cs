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

    public ItemScriptableObject GetItemOnSlot(int index){
        if(index >= inventorySlots.Count){
            return null;
        }
        return inventorySlots[index].item;
    }

    public virtual void ClearInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            inventorySlots[i] = new ItemSlot();
        }
    }

    public virtual void DropItem(int index, Transform dropPoint){
        Debug.Log("DropItem on slot " + index);
        if(GetItemOnSlot(index) != null){
            if(ObjectPooler.Instance.SpawnFromPool(GetItemOnSlot(index).itemModel, dropPoint.position, dropPoint.rotation) == null){
                Debug.LogWarning("Object Pooler couldn't Spawn " + GetItemOnSlot(index).itemModel + ". Item will be instantiated instead.");
                Instantiate(GetItemOnSlot(index).itemModel, dropPoint.position, dropPoint.rotation);
            }
            inventorySlots[index].DropItem();
        }
    }

    public virtual void DropAllInventory(){
        for(int i = 0; i < inventorySlots.Count; i++){
            while(inventorySlots[i].stackSize > 0){
                if(ObjectPooler.Instance.SpawnFromPool(inventorySlots[i].item.itemModel, this.transform.position, this.transform.rotation) == null){
                    Debug.LogWarning("Object Pooler couldn't Spawn " + inventorySlots[i].item.itemModel + ". Item will be instantiated instead.");
                    Instantiate(inventorySlots[i].item.itemModel, this.gameObject.transform.position, this.gameObject.transform.rotation);
                }
                inventorySlots[i].DropItem();
            }
        }
    }
}
