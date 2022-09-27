using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot{
    public ItemScriptableObject item;
    [SerializeField] public Sprite itemSprite;
    [SerializeField] public int stackSize;

    public ItemSlot(){
        item = null;
        itemSprite = null;
        stackSize = 0;
    }

    public void AddToSlot(ItemScriptableObject _itemToAdd){
        item = _itemToAdd;
        itemSprite = item.itemSprite;
        stackSize++;
    }

    public void AddToStack(){
        stackSize++;
    }

    public void DropItem(){
        if(stackSize > 0){
            stackSize--;
        }
        if(stackSize == 0){
            item = null;
            itemSprite = null;
        }
    }
}
