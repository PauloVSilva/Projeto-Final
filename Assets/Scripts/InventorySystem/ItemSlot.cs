using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot{
    public ItemScriptableObject item;
    public int stackSize;

    public ItemSlot(){
        item = null;
        stackSize = 0;
    }

    public void AddToSlot(ItemScriptableObject _itemToAdd){
        item = _itemToAdd;
        stackSize++;
    }

    public void AddToStack(){
        stackSize++;
    }

    public void DropItem(){
        if(stackSize > 0){
            //if(!ObjectPooler.instance.SpawnFromPool(item.itemModel, this.transform.position, this.transform.rotation)){
            //    Debug.LogWarning("Something went wrong. Object Pooler couldn't Spawn " + item.itemModel);
            //}
            stackSize--;
        }
        if(stackSize == 0){
            item = null;
        }
    }
}
