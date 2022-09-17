using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]

public class WeaponScriptableObject : ScriptableObject{
    public enum ActionType{manual, semiAuto, fullAuto}
    public enum ChamberReloadType{pump, revolver}
    public enum Size{handGun, longGun}

    public Sprite sprite;
    public string weaponName;
    public ActionType actionType;
    public ChamberReloadType chamberReloadType;
    public Size size;
    public int ammoCapacity;
    public float fireRate;
    public float weight;
    public float reloadTime;
}
