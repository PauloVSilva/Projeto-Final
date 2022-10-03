using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]

public class WeaponScriptableObject : ItemScriptableObject{
    public ActionType actionType;
    public ChamberRefillType chamberRefillType;
    public ReloadType reloadType;
    public Size size;
    public int ammoCapacity;
    public float fireRate;
    public float weight;
    public float reloadTime;
}
