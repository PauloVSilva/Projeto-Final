using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]

public class WeaponScriptableObject : ScriptableObject{
    [SerializeField] public GameObject weaponModel;
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
