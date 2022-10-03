using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]

public class ItemScriptableObject : ScriptableObject{
    public string itemName;
    public Sprite itemSprite;
    public int maxStackSize;
    public GameObject itemModel;
    public int maxAge;
    public bool canBeStored;
    public float pickUpRadius;
    public float rotationSpeed;
}
