using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]

public class WeaponScriptableObject : ScriptableObject{
    [SerializeField] public enum ActionType{manual, semiAuto, fullAuto}
    [SerializeField] public enum ChamberReloadType{pump, revolver}
    [SerializeField] public enum Size{handGun, longGun}

    [SerializeField] public GameObject characterModel;
    [SerializeField] public Sprite sprite;
    [SerializeField] public string weaponName;
    [SerializeField] public ActionType actionType;
    [SerializeField] public ChamberReloadType chamberReloadType;
    [SerializeField] public Size size;
    [SerializeField] public int ammoCapacity;
    [SerializeField] public float fireRate;
    [SerializeField] public float weight;
    [SerializeField] public float reloadTime;
}
